using UnityEngine;
using PsychoCat.Interaction;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Toggle Light";
    public string InteractionPrompt => interactionPrompt;

    [Header("Lamba Referansı")]
    [SerializeField] private Light targetLight;

    [Header("Başlangıç Durumu")]
    [SerializeField] private bool startsOn = true;

    private void Start()
    {
        if (targetLight != null)
        {
            targetLight.enabled = startsOn;
        }
    }

    public void Interact(GameObject interactor)
    {
        ToggleLight();
    }

    public void ToggleLight()
    {
        if (targetLight != null)
        {
            targetLight.enabled = !targetLight.enabled;
        }
    }
}