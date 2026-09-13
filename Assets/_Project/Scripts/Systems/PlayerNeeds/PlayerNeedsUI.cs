using UnityEngine;
using UnityEngine.UI;

namespace PsychoCat.Systems.PlayerNeeds
{
    [DisallowMultipleComponent]
    public sealed class PlayerNeedsUI : MonoBehaviour
    {
        [SerializeField] private PlayerNeedsController playerNeeds;
        [SerializeField] private Text needsText;

        private void OnEnable()
        {
            if (playerNeeds == null || needsText == null)
            {
                Debug.LogError("Assign Player Needs and needs Text to PlayerNeedsUI.", this);
                enabled = false;
                return;
            }
            playerNeeds.HungerChanged += OnNeedChanged;
            playerNeeds.SleepinessChanged += OnNeedChanged;
            Refresh();
        }

        // All scene Awakes have finished, regardless of object initialization order.
        private void Start() => Refresh();

        private void OnDisable()
        {
            if (playerNeeds == null)
                return;
            playerNeeds.HungerChanged -= OnNeedChanged;
            playerNeeds.SleepinessChanged -= OnNeedChanged;
        }

        private void OnNeedChanged(float value) => Refresh();

        private void Refresh()
        {
            if (playerNeeds == null || needsText == null)
                return;
            needsText.text = $"HUNGER  {Mathf.FloorToInt(playerNeeds.CurrentHunger)}%" +
                (playerNeeds.IsHungerCritical ? "  [CRITICAL]" : "") +
                $"\nSLEEPINESS  {Mathf.FloorToInt(playerNeeds.CurrentSleepiness)}%" +
                (playerNeeds.IsSleepinessCritical ? "  [CRITICAL]" : "");
        }
    }
}
