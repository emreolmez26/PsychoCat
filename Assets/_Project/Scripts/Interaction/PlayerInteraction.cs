using UnityEngine;
using UnityEngine.InputSystem;

namespace PsychoCat.Interaction
{
    [DisallowMultipleComponent]
    public sealed class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField, Min(0f)] private float interactionDistance = 2.5f;
        [SerializeField] private LayerMask interactionLayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private InteractionPromptUI promptUI;
        [SerializeField] private PlayerPickup playerPickup;

        private InputAction interactInput;
        private IInteractable currentTarget;

        private void OnEnable()
        {
            if (playerCamera == null || interactAction == null || interactAction.action == null)
            {
                Debug.LogError("Assign the FPS Camera and Player/Interact to PlayerInteraction.", this);
                enabled = false;
                return;
            }

            // Match the controller's input ownership without changing the shared asset.
            interactInput = interactAction.action.Clone();
            interactInput.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            interactInput.Enable();
        }

        private void LateUpdate()
        {
            // Target after movement and look have updated the camera for this frame.
            if (!Application.isFocused || Cursor.lockState != CursorLockMode.Locked ||
                playerCamera == null || !playerCamera.isActiveAndEnabled)
            {
                SetTarget(null);
                return;
            }

            if (playerPickup != null && playerPickup.IsHolding)
            {
                currentTarget = null;
                if (promptUI != null)
                    promptUI.SetPrompt("Drop | LMB - Throw");
                playerPickup.HandleHeldInput(interactInput.WasPressedThisFrame());
                // Consume this frame even if dropping/throwing emptied the hand.
                if (!playerPickup.IsHolding)
                    SetTarget(null);
                return;
            }

            SetTarget(FindTarget());

            // The initial press works even when the asset has a Hold interaction.
            if (currentTarget != null && interactInput.WasPressedThisFrame())
            {
                currentTarget.Interact(gameObject);
                if (playerPickup != null && playerPickup.IsHolding)
                    SetTarget(null);
            }
        }

        private IInteractable FindTarget()
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            ray.origin = playerCamera.transform.position;
            if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance,
                    interactionLayerMask, QueryTriggerInteraction.Ignore))
                return null;

            // Only the first solid hit is considered, so walls block interaction.
            IInteractable target = hit.collider.GetComponentInParent<IInteractable>();
            if (target is Behaviour behaviour && !behaviour.isActiveAndEnabled)
                return null;

            return target;
        }

        private void SetTarget(IInteractable target)
        {
            currentTarget = target;
            if (promptUI != null)
                promptUI.SetPrompt(currentTarget?.InteractionPrompt);
        }

        private void OnDisable()
        {
            interactInput?.Dispose();
            interactInput = null;
            SetTarget(null);
        }
    }
}
