using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [Header("Açı Ayarları")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float speed = 3f;

    private Transform hingeTransform;
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotateCoroutine;
    private Camera mainCam;

    private void Awake()
    {
        hingeTransform = transform.parent != null ? transform.parent : transform;
    }

    private void Start()
    {
        mainCam = Camera.main;
        closedRotation = hingeTransform.localRotation;
        openRotation = Quaternion.Euler(hingeTransform.localEulerAngles + new Vector3(0f, openAngle, 0f));
    }

    private void Update()
    {
        // Yeni Input System üzerinden fare sol tık kontrolü
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(hingeTransform))
                {
                    ToggleDoor();
                }
            }
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        rotateCoroutine = StartCoroutine(AnimateRotation(targetRotation));
    }

    private IEnumerator AnimateRotation(Quaternion targetRot)
    {
        while (Quaternion.Angle(hingeTransform.localRotation, targetRot) > 0.1f)
        {
            hingeTransform.localRotation = Quaternion.Slerp(hingeTransform.localRotation, targetRot, Time.deltaTime * speed);
            yield return null;
        }

        hingeTransform.localRotation = targetRot;
    }
}