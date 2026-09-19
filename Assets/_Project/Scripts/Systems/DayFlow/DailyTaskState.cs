namespace PsychoCat.Systems.DayFlow
{
    public enum DailyTaskState
    {
        Pending,
        Completed
    }

    public sealed class DailyTask
    {
        public string Id { get; }
        public string DisplayName { get; }
        public bool IsRequired { get; }
        public DailyTaskState State { get; private set; }

        internal DailyTask(DailyTaskDefinition definition)
        {
            Id = definition.Id;
            DisplayName = definition.DisplayName;
            IsRequired = definition.IsRequired;
            State = DailyTaskState.Pending;
        }

        internal void Complete()
        {
            State = DailyTaskState.Completed;
        }
    }
}
