using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("Hand Flashlight Object")]
    [Tooltip("Kameranın altındaki Player_Flashlight objesi")]
    [SerializeField] private GameObject handFlashlightRoot;

    private bool hasFlashlight = false;
    private bool isOn = false;

    private void Update()
    {
        if (!hasFlashlight) return;

        // F tuşuna basıldığında aç/kapat
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    public void CollectFlashlight()
    {
        hasFlashlight = true;
        isOn = true;

        if (handFlashlightRoot != null)
        {
            handFlashlightRoot.SetActive(true);
        }

        Debug.Log("[Flashlight] Fener alındı ve açıldı. Açıp kapatmak için: F");
    }

    public void ToggleFlashlight()
    {
        isOn = !isOn;
        if (handFlashlightRoot != null)
        {
            handFlashlightRoot.SetActive(isOn);
        }
    }
}