using UnityEngine;
using UnityEngine.InputSystem;

namespace PsychoCat.Player
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-10)]
    public sealed class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField, Min(0f), Tooltip("Degrees of rotation per pixel of mouse movement.")]
        private float mouseSensitivity = 0.1f;
        [SerializeField, Range(1f, 89f)] private float maxVerticalLookAngle = 85f;

        private InputAction lookInput;
        private float pitch;
        private bool skipLookFrame;

        private void OnEnable()
        {
            if (playerCamera == null || playerCamera.parent != transform ||
                lookAction == null || lookAction.action == null)
            {
                Debug.LogError("Assign a direct child Camera and Player/Look to PlayerLook on the Player root.", this);
                enabled = false;
                return;
            }

            lookInput = lookAction.action.Clone();
            lookInput.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            lookInput.Enable();
            pitch = Mathf.Clamp(Mathf.DeltaAngle(0f, playerCamera.localEulerAngles.x),
                -maxVerticalLookAngle, maxVerticalLookAngle);
            playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            SetCursorLocked(Application.isFocused);
        }

        private void Update()
        {
            if (!Application.isFocused)
                return;

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetCursorLocked(false);
                return;
            }

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                    SetCursorLocked(true);
                return;
            }

            if (skipLookFrame)
            {
                skipLookFrame = false;
                return;
            }

            // Mouse delta is already displacement for this frame; do not multiply by deltaTime.
            Vector2 delta = lookInput.ReadValue<Vector2>() * mouseSensitivity;
            transform.Rotate(Vector3.up, delta.x, Space.World);
            pitch = Mathf.Clamp(pitch - delta.y, -maxVerticalLookAngle, maxVerticalLookAngle);
            playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
            skipLookFrame = true;
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused)
                SetCursorLocked(false);
        }

        private void OnDisable()
        {
            lookInput?.Dispose();
            lookInput = null;
            SetCursorLocked(false);
        }

        private void OnValidate()
        {
            mouseSensitivity = Mathf.Max(0f, mouseSensitivity);
            maxVerticalLookAngle = Mathf.Clamp(maxVerticalLookAngle, 1f, 89f);
        }
    }
}
