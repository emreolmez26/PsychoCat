using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PsychoCat.Interaction
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-20)]
    public sealed class PlayerPickup : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private InputActionReference throwAction;
        [SerializeField, Min(0f), Tooltip("Throw impulse in Newton-seconds.")]
        private float throwForce = 8f;

        private InputAction throwInput;
        private PickupItem heldItem;
        private Rigidbody heldBody;
        private Transform originalParent;
        private Rigidbody pendingDetachBody;
        private Transform pendingDetachParent;
        private bool originalGravity;
        private bool originalKinematic;
        private RigidbodyInterpolation originalInterpolation;
        private CollisionDetectionMode originalCollisionMode;
        private Collider[] playerColliders;
        private bool hadControlAtFrameStart;
        private readonly List<CollisionPair> collisionPairs = new List<CollisionPair>();

        private struct CollisionPair
        {
            public Collider Item;
            public Collider Player;
            public bool WasIgnored;
        }

        public bool IsHolding => heldItem != null && heldBody != null;

        private void Awake()
        {
            playerColliders = GetComponentsInChildren<Collider>(true);
        }

        private void OnEnable()
        {
            if (holdPoint == null || playerCamera == null ||
                holdPoint.parent != playerCamera.transform ||
                throwAction == null || throwAction.action == null)
            {
                Debug.LogError("Assign the FPS Camera, its HoldPoint child and throw action to PlayerPickup.", this);
                enabled = false;
                return;
            }

            throwInput = throwAction.action.Clone();
            throwInput.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            throwInput.Enable();
            hadControlAtFrameStart = false;
        }

        private void Update()
        {
            // Hierarchy changes are safe here, after any OnDisable callbacks finish.
            if (pendingDetachBody != null)
                pendingDetachBody.transform.SetParent(pendingDetachParent, true);
            pendingDetachBody = null;
            pendingDetachParent = null;

            // Run before PlayerLook: a click that relocks the cursor must not throw.
            hadControlAtFrameStart = Application.isFocused && Cursor.lockState == CursorLockMode.Locked;
            if ((heldItem != null || heldBody != null || collisionPairs.Count > 0) &&
                (!IsHolding || holdPoint == null || !heldItem.isActiveAndEnabled))
                Drop();
        }

        public bool TryPickup(PickupItem item)
        {
            if (!isActiveAndEnabled || IsHolding || pendingDetachBody != null || holdPoint == null || item == null ||
                !item.isActiveAndEnabled || item.Body == null || item.Holder != null)
                return false;

            heldItem = item;
            heldBody = item.Body;
            item.Holder = this;
            originalParent = heldBody.transform.parent;
            originalGravity = heldBody.useGravity;
            originalKinematic = heldBody.isKinematic;
            originalInterpolation = heldBody.interpolation;
            originalCollisionMode = heldBody.collisionDetectionMode;

            if (!heldBody.isKinematic)
            {
                heldBody.linearVelocity = Vector3.zero;
                heldBody.angularVelocity = Vector3.zero;
            }
            heldBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            heldBody.isKinematic = true;
            heldBody.useGravity = false;
            heldBody.interpolation = RigidbodyInterpolation.None;
            IgnorePlayerCollisions();
            heldBody.transform.SetParent(holdPoint, true);
            heldBody.transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);
            return true;
        }

        public void HandleHeldInput(bool dropPressed)
        {
            if (!IsHolding || !Application.isFocused || Cursor.lockState != CursorLockMode.Locked)
                return;

            if (dropPressed)
                Drop();
            else if (throwInput != null && throwInput.WasPressedThisFrame())
                Throw();
        }

        public void Drop()
        {
            Release();
        }

        public void Throw()
        {
            if (!IsHolding || !hadControlAtFrameStart || !Application.isFocused ||
                Cursor.lockState != CursorLockMode.Locked || playerCamera == null)
                return;

            Rigidbody body = Release();
            if (body != null && !body.isKinematic)
                body.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        }

        internal void ReleaseDisabledItem(PickupItem item)
        {
            if (heldItem == item)
                Release(false);
        }

        private Rigidbody Release(bool detachImmediately = true)
        {
            Rigidbody body = heldBody;
            if (heldItem != null)
                heldItem.Holder = null;
            heldItem = null;
            heldBody = null;

            if (body != null)
            {
                if (detachImmediately)
                    body.transform.SetParent(originalParent, true);
                else
                {
                    // Never reparent inside hierarchy deactivation. During shutdown
                    // no further Update runs; during gameplay detach on the next one.
                    pendingDetachBody = body;
                    pendingDetachParent = originalParent;
                }
                body.isKinematic = originalKinematic;
                body.useGravity = originalGravity;
                body.interpolation = originalInterpolation;
                body.collisionDetectionMode = originalCollisionMode;
                if (!body.isKinematic)
                {
                    body.linearVelocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                    body.WakeUp();
                }
            }

            foreach (CollisionPair pair in collisionPairs)
            {
                if (pair.Item != null && pair.Player != null)
                    Physics.IgnoreCollision(pair.Item, pair.Player, pair.WasIgnored);
            }
            collisionPairs.Clear();
            originalParent = null;
            return body;
        }

        private void IgnorePlayerCollisions()
        {
            foreach (Collider itemCollider in heldBody.GetComponentsInChildren<Collider>(true))
            {
                if (itemCollider.attachedRigidbody != heldBody)
                    continue;
                foreach (Collider playerCollider in playerColliders)
                {
                    if (playerCollider == null || playerCollider == itemCollider)
                        continue;
                    collisionPairs.Add(new CollisionPair
                    {
                        Item = itemCollider,
                        Player = playerCollider,
                        WasIgnored = Physics.GetIgnoreCollision(itemCollider, playerCollider)
                    });
                    Physics.IgnoreCollision(itemCollider, playerCollider, true);
                }
            }
        }

        private void OnDisable()
        {
            Release(false);
            throwInput?.Dispose();
            throwInput = null;
            hadControlAtFrameStart = false;
        }
    }
}
