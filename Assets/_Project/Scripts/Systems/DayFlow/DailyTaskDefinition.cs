using System;
using UnityEngine;

namespace PsychoCat.Systems.DayFlow
{
    [Serializable]
    public sealed class DailyTaskDefinition
    {
        [SerializeField] private string taskId;
        [SerializeField] private string displayName;
        [SerializeField] private bool required = true;

        public string Id => taskId;
        public string DisplayName => displayName;
        public bool IsRequired => required;
    }
}
