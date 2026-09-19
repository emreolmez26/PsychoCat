using System;
using UnityEngine;

namespace PsychoCat.Systems.PlayerNeeds
{
    [DisallowMultipleComponent]
    public sealed class PlayerNeedsController : MonoBehaviour
    {
        [SerializeField, Range(0f, 100f)] private float startingHunger = 35f;
        [SerializeField, Range(0f, 100f)] private float startingSleepiness = 25f;
        [SerializeField, Min(0f), Tooltip("Need points per gameplay second.")]
        private float hungerIncreaseRate = 0.5f;
        [SerializeField, Min(0f), Tooltip("Need points per gameplay second.")]
        private float sleepinessIncreaseRate = 0.25f;
        [SerializeField, Range(0f, 100f)] private float criticalHungerThreshold = 80f;
        [SerializeField, Range(0f, 100f)] private float criticalSleepinessThreshold = 80f;

        private float hunger;
        private float sleepiness;

        public float CurrentHunger => hunger;
        public float CurrentSleepiness => sleepiness;
        public bool IsHungerCritical => hunger >= criticalHungerThreshold;
        public bool IsSleepinessCritical => sleepiness >= criticalSleepinessThreshold;

        public event Action<float> HungerChanged;
        public event Action<float> SleepinessChanged;
        public event Action HungerCritical;
        public event Action SleepinessCritical;

        private void Awake()
        {
            ValidateTuning();
            hunger = startingHunger;
            sleepiness = startingSleepiness;
        }

        private void OnValidate() => ValidateTuning();

        private void Update() => Tick(Time.deltaTime);

        private void Tick(float seconds)
        {
            if (!IsFiniteNonNegative(seconds))
                return;
            ChangeNeed(ref hunger, hunger + seconds * hungerIncreaseRate,
                criticalHungerThreshold, HungerChanged, HungerCritical);
            ChangeNeed(ref sleepiness, sleepiness + seconds * sleepinessIncreaseRate,
                criticalSleepinessThreshold, SleepinessChanged, SleepinessCritical);
        }

        public void Eat(float amount)
        {
            if (IsFiniteNonNegative(amount))
                ChangeNeed(ref hunger, hunger - amount, criticalHungerThreshold, HungerChanged, HungerCritical);
        }

        public void Rest(float amount)
        {
            if (IsFiniteNonNegative(amount))
                ChangeNeed(ref sleepiness, sleepiness - amount, criticalSleepinessThreshold, SleepinessChanged, SleepinessCritical);
        }

        private static void ChangeNeed(ref float current, float next, float threshold,
            Action<float> changed, Action critical)
        {
            next = Mathf.Clamp(next, 0f, 100f);
            if (current == next)
                return;
            bool enteredCritical = current < threshold && next >= threshold;
            current = next;
            changed?.Invoke(next);
            if (enteredCritical)
                critical?.Invoke();
        }

        private static bool IsFiniteNonNegative(float value) =>
            !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f;

        private void ValidateTuning()
        {
            startingHunger = ClampTuning(startingHunger, 35f);
            startingSleepiness = ClampTuning(startingSleepiness, 25f);
            criticalHungerThreshold = ClampTuning(criticalHungerThreshold, 80f);
            criticalSleepinessThreshold = ClampTuning(criticalSleepinessThreshold, 80f);
            hungerIncreaseRate = IsFiniteNonNegative(hungerIncreaseRate) ? hungerIncreaseRate : 0f;
            sleepinessIncreaseRate = IsFiniteNonNegative(sleepinessIncreaseRate) ? sleepinessIncreaseRate : 0f;
        }

        private static float ClampTuning(float value, float fallback) =>
            float.IsNaN(value) || float.IsInfinity(value) ? fallback : Mathf.Clamp(value, 0f, 100f);
    }
}
