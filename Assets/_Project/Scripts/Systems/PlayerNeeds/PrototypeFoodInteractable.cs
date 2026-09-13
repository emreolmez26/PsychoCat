using PsychoCat.Interaction;
using UnityEngine;

namespace PsychoCat.Systems.PlayerNeeds
{
    [DisallowMultipleComponent]
    public sealed class PrototypeFoodInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private PlayerNeedsController playerNeeds;
        [SerializeField, Min(0f)] private float hungerRestored = 35f;

        public string InteractionPrompt => "Eat Cereal";

        public void Interact(GameObject interactor)
        {
            if (playerNeeds != null)
                playerNeeds.Eat(hungerRestored);
        }
    }
}
