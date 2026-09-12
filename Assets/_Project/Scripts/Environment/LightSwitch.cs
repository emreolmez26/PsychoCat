using UnityEngine;
using UnityEngine.InputSystem;

public class LightSwitch : MonoBehaviour
{
    [Header("Lamba Referansı")]
    [SerializeField] private Light targetLight;

    [Header("Başlangıç Durumu")]
    [SerializeField] private bool startsOn = true;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        // Başlangıç durumunu uygula
        if (targetLight != null)
        {
            targetLight.enabled = startsOn;
        }
    }

    private void Update()
    {
        // Yeni Input System ile sol tık kontrolü
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Tıklanan nesne bu şalter mi?
                if (hit.transform == transform)
                {
                    ToggleLight();
                }
            }
        }
    }

    public void ToggleLight()
    {
        if (targetLight != null)
        {
            targetLight.enabled = !targetLight.enabled;
        }
    }
}