using System;
using System.Collections.Generic;
using UnityEngine;

namespace PsychoCat.Systems.DayFlow
{
    [Serializable]
    public sealed class DayDefinition
    {
        [SerializeField, Min(1)] private int dayNumber = 1;
        [SerializeField] private DailyTaskDefinition[] tasks = Array.Empty<DailyTaskDefinition>();

        public int DayNumber => dayNumber;
        public IReadOnlyList<DailyTaskDefinition> Tasks => tasks;
    }
}
