using PsychoCat.Interaction;
using UnityEngine;

namespace PsychoCat.Systems.DayFlow
{
    [DisallowMultipleComponent]
    public sealed class PrototypeEndDayInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DayFlowController dayFlow;

        public string InteractionPrompt => dayFlow == null || !dayFlow.IsDayComplete
            ? "Finish required tasks first"
            : dayFlow.CanAdvanceDay ? "End Day" : "No more prototype days";

        public void Interact(GameObject interactor)
        {
            if (dayFlow != null)
                dayFlow.TryAdvanceDay();
        }
    }
}
