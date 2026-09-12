using UnityEngine;

namespace PsychoCat.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PickupItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionPrompt = "Pick Up";
        private Rigidbody body;

        public string InteractionPrompt => interactionPrompt;
        public Rigidbody Body => body;
        internal PlayerPickup Holder { get; set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        public void Interact(GameObject interactor)
        {
            if (isActiveAndEnabled && interactor != null &&
                interactor.TryGetComponent(out PlayerPickup pickup))
                pickup.TryPickup(this);
        }

        private void OnDisable()
        {
            if (Holder != null)
                Holder.ReleaseDisabledItem(this);
        }
    }
}
