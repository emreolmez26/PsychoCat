using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Açı Ayarları")]
    [Tooltip("Kapı açıkken menteşenin Y eksenindeki açısı")]
    [SerializeField] private float openAngle = 90f;
    [Tooltip("Açılma/kapanma animasyon hızı")]
    [SerializeField] private float speed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotateCoroutine;

    private void Start()
    {
        // Kapının başlangıçtaki kapalı rotasyonunu hafızaya al
        closedRotation = transform.localRotation;
        // Kapalı rotasyonun üzerine belirlenen açıyı ekle
        openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0f, openAngle, 0f));
    }

    /// <summary>
    /// Kapı durumunu tersine çevirir (açıksa kapatır, kapalıysa açar).
    /// </summary>
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
        while (Quaternion.Angle(transform.localRotation, targetRot) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * speed);
            yield return null;
        }

        transform.localRotation = targetRot;
    }

    // Prototip aşamasında Play modundayken kapıya fareyle tıklayarak test etmek için:
    private void OnMouseDown()
    {
        ToggleDoor();
    }
}