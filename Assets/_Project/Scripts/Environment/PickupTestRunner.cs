using UnityEngine;
using UnityEngine.InputSystem;
using PsychoCat.Interaction;

public class PickupTestRunner : MonoBehaviour
{
    public PlayerPickup pickup;
    public PickupItem targetItem;

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 1 Tuşu: Cursor kilitle
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("<color=cyan>[TEST]</color> Cursor kilitlendi!");
        }

        // 2 Tuşu: Doğrudan ele al
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            if (pickup != null && targetItem != null)
            {
                pickup.TryPickup(targetItem);
                Debug.Log("<color=green>[TEST]</color> Bardak ele alındı!");
            }
        }

        // ESC: Kilidi aç
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}