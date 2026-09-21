using System.Text;
using PsychoCat.Systems.DayFlow;
using UnityEngine;
using UnityEngine.UI;

namespace PsychoCat.Day1
{
    public sealed class Day1FlowUI : MonoBehaviour
    {
        [SerializeField] private Day1EventCoordinator coordinator;
        [SerializeField] private Text taskText;

        private void OnEnable()
        {
            if (coordinator == null || taskText == null) { enabled = false; return; }
            coordinator.FlowChanged += Refresh;
            Refresh();
        }
        private void Start() => Refresh();
        private void OnDisable()
        {
            if (coordinator != null) coordinator.FlowChanged -= Refresh;
        }
        private void Refresh()
        {
            if (coordinator == null || taskText == null || coordinator.DayFlow == null) return;
            var flow = coordinator.DayFlow;
            var text = new StringBuilder();
            text.Append("DAY ").Append(flow.CurrentDayNumber).AppendLine().AppendLine();
            if (flow.CurrentDayNumber == 1)
            {
                foreach (DailyTask task in flow.ActiveTasks)
                {
                    text.Append(task.State == DailyTaskState.Completed ? "[x] " :
                        coordinator.IsTaskUnlocked(task.Id) ? "[>] " : "[-] ");
                    text.Append(task.DisplayName);
                    if (!task.IsRequired) text.Append(" (optional)");
                    text.AppendLine();
                }
                if (flow.IsDayComplete) text.AppendLine().AppendLine("DAY COMPLETE - Return to bed");
            }
            text.AppendLine().Append(coordinator.Status);
            taskText.text = text.ToString();
        }
    }
}
