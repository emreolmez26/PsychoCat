using UnityEngine;
using PsychoCat.Interaction; // Projenin etkileşim namespace'i

public class SmartHomePanel : MonoBehaviour, IInteractable
{
    [Header("Connected Devices")]
    [SerializeField] private RobotVacuum targetVacuum;

    [Header("UI Prompt")]
    [SerializeField] private string interactionPrompt = "Robot Süpürgeyi Aç/Kapat";
    public string InteractionPrompt => interactionPrompt;

    public void Interact(GameObject interactor)
    {
        ToggleVacuum();
    }

    public void ToggleVacuum()
    {
        if (targetVacuum != null)
        {
            targetVacuum.ToggleVacuum();
            Debug.Log($"[SmartHomePanel] Robot süpürge durumu: {targetVacuum.IsRunning()}");
        }
        else
        {
            Debug.LogWarning("[SmartHomePanel] Target Vacuum atanmamış!");
        }
    }
}