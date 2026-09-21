using PsychoCat.Interaction;
using PsychoCat.Systems.PlayerNeeds;
using PsychoCat.Systems.DayFlow;
using UnityEngine;

namespace PsychoCat.Day1
{
    public enum Day1TaskAction { Placeholder, ExistingInteraction, Eat, ResolveFeeder }

    [DisallowMultipleComponent]
    public sealed class Day1TaskInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Day1EventCoordinator coordinator;
        [SerializeField] private string taskId;
        [SerializeField] private string prompt;
        [SerializeField] private Day1TaskAction action;
        [SerializeField] private MonoBehaviour target;
        [SerializeField] private bool repeatAfterCompletion;
        [SerializeField, Min(0f)] private float hungerRestored = 35f;

        public string TaskId => taskId;
        private bool CanRepeat => repeatAfterCompletion && action == Day1TaskAction.ExistingInteraction &&
            coordinator != null && coordinator.isActiveAndEnabled && coordinator.DayFlow != null &&
            coordinator.DayFlow.TryGetTask(taskId, out DailyTask task) && task.State == DailyTaskState.Completed;

        public string InteractionPrompt => CanRepeat && target is IInteractable interaction
            ? interaction.InteractionPrompt : coordinator != null && coordinator.IsTaskUnlocked(taskId)
            ? prompt : "Complete the current task first";

        public void Interact(GameObject interactor)
        {
            // A completed panel task must not disable the panel's normal toggle behavior.
            if (CanRepeat && target is IInteractable repeatable)
            {
                repeatable.Interact(interactor);
                return;
            }
            if (coordinator == null || !coordinator.IsTaskUnlocked(taskId)) return;
            switch (action)
            {
                case Day1TaskAction.ExistingInteraction:
                    if (!(target is IInteractable interaction)) return;
                    interaction.Interact(interactor);
                    break;
                case Day1TaskAction.Eat:
                    if (!(target is PlayerNeedsController needs)) return;
                    needs.Eat(hungerRestored);
                    break;
                case Day1TaskAction.ResolveFeeder:
                    if (!(target is AutoFeeder feeder)) return;
                    feeder.SetNormal();
                    break;
            }
            coordinator.CompleteTask(taskId);
        }
    }
}
