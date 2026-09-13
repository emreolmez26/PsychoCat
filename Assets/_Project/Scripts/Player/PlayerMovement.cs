using UnityEngine;
using UnityEngine.InputSystem;

namespace PsychoCat.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference sprintAction;
        [SerializeField, Min(0f)] private float walkSpeed = 3f;
        [SerializeField, Min(0f)] private float sprintSpeed = 5f;
        [SerializeField, Tooltip("Downward acceleration in metres per second squared.")]
        private float gravity = -20f;

        private const float GroundedVelocity = -2f;
        private CharacterController controller;
        private InputAction moveInput;
        private InputAction sprintInput;
        private float verticalVelocity;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (moveAction == null || moveAction.action == null ||
                sprintAction == null || sprintAction.action == null)
            {
                Debug.LogError("Assign Player/Move and Player/Sprint to PlayerMovement.", this);
                enabled = false;
                return;
            }

            // Own the action lifetime without changing the shared project's input asset.
            moveInput = moveAction.action.Clone();
            sprintInput = sprintAction.action.Clone();
            moveInput.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            sprintInput.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            sprintInput.wantsInitialStateCheck = true;
            moveInput.Enable();
            sprintInput.Enable();
            verticalVelocity = 0f;
            IsGrounded = false;
        }

        private void Update()
        {
            if (!controller.enabled)
            {
                verticalVelocity = 0f;
                IsGrounded = false;
                return;
            }

            bool hasControl = Application.isFocused && Cursor.lockState == CursorLockMode.Locked;
            Vector2 input = hasControl ? Vector2.ClampMagnitude(moveInput.ReadValue<Vector2>(), 1f) : Vector2.zero;
            float speed = hasControl && sprintInput.IsPressed() ? sprintSpeed : walkSpeed;
            Vector3 velocity = (transform.right * input.x + transform.forward * input.y) * speed;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = GroundedVelocity;

            verticalVelocity += gravity * Time.deltaTime;
            velocity.y = verticalVelocity;
            CollisionFlags collisions = controller.Move(velocity * Time.deltaTime);
            IsGrounded = (collisions & CollisionFlags.Below) != 0;

            // Reset after contact so a landing cannot retain accumulated falling speed.
            if (IsGrounded && verticalVelocity < 0f)
                verticalVelocity = GroundedVelocity;
        }

        private void OnDisable()
        {
            moveInput?.Dispose();
            sprintInput?.Dispose();
            moveInput = null;
            sprintInput = null;
            verticalVelocity = 0f;
            IsGrounded = false;
        }

        private void OnValidate()
        {
            walkSpeed = Mathf.Max(0f, walkSpeed);
            sprintSpeed = Mathf.Max(walkSpeed, sprintSpeed);
            gravity = Mathf.Min(-0.01f, gravity);
        }
    }
}
