using PsychoCat.Interaction;
using UnityEngine;

namespace PsychoCat.Systems.DayFlow
{
    [DisallowMultipleComponent]
    public sealed class PrototypeTaskInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DayFlowController dayFlow;
        [SerializeField] private string taskId;

        public string InteractionPrompt
        {
            get
            {
                if (dayFlow == null || !dayFlow.TryGetTask(taskId, out DailyTask task))
                    return "No task here today";
                return task.DisplayName + (task.State == DailyTaskState.Completed ? " (Completed)" : "");
            }
        }

        public void Interact(GameObject interactor)
        {
            if (dayFlow != null && dayFlow.TryGetTask(taskId, out _))
                dayFlow.CompleteTask(taskId);
        }
    }
}
