using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace PsychoCat.Systems.DayFlow
{
    [DisallowMultipleComponent]
    public sealed class DayFlowController : MonoBehaviour
    {
        [SerializeField] private DayDefinition[] days = Array.Empty<DayDefinition>();

        private readonly List<DailyTask> activeTasks = new List<DailyTask>();
        private ReadOnlyCollection<DailyTask> taskView;
        private int dayIndex = -1;

        public int CurrentDayNumber => dayIndex < 0 ? 0 : days[dayIndex].DayNumber;
        public IReadOnlyList<DailyTask> ActiveTasks => taskView ?? (taskView = activeTasks.AsReadOnly());
        public bool IsDayComplete { get; private set; }
        public bool CanAdvanceDay => IsDayComplete && dayIndex + 1 < days.Length;

        public event Action<DailyTask> TaskStateChanged;
        public event Action<int> DayStarted;
        public event Action<int> DayCompleted;

        private void Start()
        {
            if (!ValidateDefinitions())
            {
                Debug.LogError("DayFlowController needs ordered, unique day numbers and unique non-empty task IDs/names, with at least one required task per day.", this);
                enabled = false;
                return;
            }

            StartDay(0);
        }

        public bool TryGetTask(string taskId, out DailyTask task)
        {
            task = activeTasks.Find(candidate => string.Equals(candidate.Id, taskId, StringComparison.Ordinal));
            return task != null;
        }

        // Returns true only for a new Pending -> Completed transition.
        public bool CompleteTask(string taskId)
        {
            if (!TryGetTask(taskId, out DailyTask task))
            {
                Debug.LogWarning($"Unknown task ID '{taskId}' for day {CurrentDayNumber}.", this);
                return false;
            }
            if (task.State == DailyTaskState.Completed)
                return false;

            task.Complete();
            bool completedNow = !IsDayComplete && activeTasks.TrueForAll(
                candidate => !candidate.IsRequired || candidate.State == DailyTaskState.Completed);
            if (completedNow)
                IsDayComplete = true;

            TaskStateChanged?.Invoke(task);
            if (completedNow)
                DayCompleted?.Invoke(CurrentDayNumber);
            return true;
        }

        public bool TryAdvanceDay()
        {
            if (!CanAdvanceDay)
                return false;

            StartDay(dayIndex + 1);
            return true;
        }

        private void StartDay(int index)
        {
            dayIndex = index;
            IsDayComplete = false;
            activeTasks.Clear();
            foreach (DailyTaskDefinition definition in days[index].Tasks)
                activeTasks.Add(new DailyTask(definition));
            DayStarted?.Invoke(CurrentDayNumber);
        }

        private bool ValidateDefinitions()
        {
            if (days == null || days.Length == 0)
                return false;

            int previousNumber = 0;
            foreach (DayDefinition day in days)
            {
                if (day == null || day.DayNumber <= previousNumber || day.Tasks == null)
                    return false;
                previousNumber = day.DayNumber;
                var ids = new HashSet<string>(StringComparer.Ordinal);
                bool hasRequiredTask = false;
                foreach (DailyTaskDefinition task in day.Tasks)
                {
                    if (task == null || string.IsNullOrWhiteSpace(task.Id) ||
                        string.IsNullOrWhiteSpace(task.DisplayName) || !ids.Add(task.Id))
                        return false;
                    hasRequiredTask |= task.IsRequired;
                }
                if (!hasRequiredTask)
                    return false;
            }
            return true;
        }
    }
}
