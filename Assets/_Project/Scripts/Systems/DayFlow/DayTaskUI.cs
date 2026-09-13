using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PsychoCat.Systems.DayFlow
{
    [DisallowMultipleComponent]
    public sealed class DayTaskUI : MonoBehaviour
    {
        [SerializeField] private DayFlowController dayFlow;
        [SerializeField] private Text taskText;

        private void OnEnable()
        {
            if (dayFlow == null || taskText == null)
            {
                Debug.LogError("Assign Day Flow and task Text to DayTaskUI.", this);
                enabled = false;
                return;
            }
            dayFlow.DayStarted += OnDayChanged;
            dayFlow.DayCompleted += OnDayChanged;
            dayFlow.TaskStateChanged += OnTaskChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (dayFlow == null)
                return;
            dayFlow.DayStarted -= OnDayChanged;
            dayFlow.DayCompleted -= OnDayChanged;
            dayFlow.TaskStateChanged -= OnTaskChanged;
        }

        private void OnDayChanged(int dayNumber) => Refresh();
        private void OnTaskChanged(DailyTask task) => Refresh();

        private void Refresh()
        {
            var text = new StringBuilder();
            if (dayFlow.CurrentDayNumber == 0)
            {
                taskText.text = "";
                return;
            }
            text.Append("DAY ").Append(dayFlow.CurrentDayNumber).AppendLine().AppendLine();
            foreach (DailyTask task in dayFlow.ActiveTasks)
            {
                text.Append(task.State == DailyTaskState.Completed ? "[x] " : "[ ] ");
                text.Append(task.DisplayName);
                if (!task.IsRequired)
                    text.Append(" (optional)");
                text.AppendLine();
            }
            if (dayFlow.IsDayComplete)
            {
                text.AppendLine().AppendLine("DAY COMPLETE");
                text.Append(dayFlow.CanAdvanceDay ? "Use End Day to continue." : "End of prototype days.");
            }
            taskText.text = text.ToString();
        }
    }
}
