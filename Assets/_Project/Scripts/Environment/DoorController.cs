using System.Collections;
using UnityEngine;
using PsychoCat.Interaction;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Open/Close Door";
    public string InteractionPrompt => interactionPrompt;

    [Header("Açı Ayarları")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float speed = 3f;

    private Transform hingeTransform;
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotateCoroutine;

    private void Awake()
    {
        hingeTransform = transform.parent != null ? transform.parent : transform;
    }

    private void Start()
    {
        closedRotation = hingeTransform.localRotation;
        openRotation = Quaternion.Euler(hingeTransform.localEulerAngles + new Vector3(0f, openAngle, 0f));
    }

    public void Interact(GameObject interactor)
    {
        ToggleDoor();
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