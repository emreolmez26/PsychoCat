using PsychoCat.Interaction;
using UnityEngine;

namespace PsychoCat.Systems.PlayerNeeds
{
    [DisallowMultipleComponent]
    public sealed class PrototypeRestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private PlayerNeedsController playerNeeds;
        [SerializeField, Min(0f)] private float sleepinessRestored = 75f;

        public string InteractionPrompt => "Rest";

        public void Interact(GameObject interactor)
        {
            if (playerNeeds != null)
                playerNeeds.Rest(sleepinessRestored);
        }
    }
}
