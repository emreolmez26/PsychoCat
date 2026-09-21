using UnityEngine;
using PsychoCat.Interaction;

public class FlashlightItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string promptMessage = "Feneri Al";
    public string InteractionPrompt => promptMessage;

    public void Interact(GameObject interactor)
    {
        if (interactor != null && interactor.TryGetComponent(out FlashlightController controller))
        {
            controller.CollectFlashlight();
            gameObject.SetActive(false); // Masadaki feneri sahneden kaldır
        }
    }
}