using UnityEngine;
using UnityEngine.UI;

namespace PsychoCat.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private Text promptText;
        private string currentPrompt;

        private void Awake()
        {
            if (promptText == null)
            {
                Debug.LogError("Assign the prototype prompt Text to InteractionPromptUI.", this);
                enabled = false;
                return;
            }

            promptText.enabled = false;
        }

        public void SetPrompt(string prompt)
        {
            if (promptText == null)
                return;

            bool visible = isActiveAndEnabled && !string.IsNullOrEmpty(prompt);
            if (visible && currentPrompt != prompt)
                promptText.text = "E - " + prompt;

            currentPrompt = visible ? prompt : null;
            promptText.enabled = visible;
        }

        private void OnDisable()
        {
            SetPrompt(null);
        }
    }
}
