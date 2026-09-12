using UnityEngine;

namespace PsychoCat.Interaction
{
    [DisallowMultipleComponent]
    public sealed class PrototypeInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionPrompt = "Test Interaction";

        public string InteractionPrompt => interactionPrompt;

        public void Interact(GameObject interactor)
        {
            Debug.Log("Prototype interactable activated.", this);
        }
    }
}
