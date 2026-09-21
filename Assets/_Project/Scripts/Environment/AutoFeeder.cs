using System.Collections;
using UnityEngine;
using PsychoCat.Interaction; // Projenin etkileşim altyapısı

public class AutoFeeder : MonoBehaviour, IInteractable
{
    public enum FeederState
    {
        Normal,
        Sabotaged
    }

    [Header("State Settings")]
    [SerializeField] private FeederState currentState = FeederState.Normal;

    [Header("Dispense Points & Visuals")]
    [Tooltip("Mamanın döküleceği nokta/kap")]
    [SerializeField] private Transform foodDispensePoint;
    [Tooltip("Normal modda çıkan tek mama yığını / tanesi")]
    [SerializeField] private GameObject normalFoodVisual;
    [Tooltip("Sabotaj modunda etrafa taşan dev mama yığını")]
    [SerializeField] private GameObject overflowFoodVisual;

    [Header("Sabotage Parameters")]
    [SerializeField] private float sabotageDispenseInterval = 0.2f;
    [SerializeField] private int maxSpillCount = 15;

    [Header("UI Prompt")]
    [SerializeField] private string normalPrompt = "Mama Kabını İncele";
    [SerializeField] private string sabotagedPrompt = "Sabote Edilmiş Mama Kabını İncele";

    public string InteractionPrompt => currentState == FeederState.Normal ? normalPrompt : sabotagedPrompt;

    private bool isDispensing = false;

    private void Start()
    {
        UpdateVisuals();
    }

    // Oyuncu E ile incelediğinde / tıkladığında
    public void Interact(GameObject interactor)
    {
        if (currentState == FeederState.Normal)
        {
            Debug.Log("[AutoFeeder] Mama kabı normal çalışıyor. Tek porsiyon verildi.");
            DispenseNormalFood();
        }
        else
        {
            Debug.Log("[AutoFeeder] UYARI: Mama kabı sabote edilmiş! Sistem kontrol dışı.");
            TriggerSabotageEffect();
        }
    }

    public void DispenseNormalFood()
    {
        if (normalFoodVisual != null)
        {
            normalFoodVisual.SetActive(true);
        }
    }

    // Görev/Hikaye akışında kedinin veya olayın çağıracağı fonksiyon
    public void SetSabotaged()
    {
        currentState = FeederState.Sabotaged;
        UpdateVisuals();
        TriggerSabotageEffect();
    }

    public void SetNormal()
    {
        currentState = FeederState.Normal;
        UpdateVisuals();
    }

    private void TriggerSabotageEffect()
    {
        if (!isDispensing)
        {
            StartCoroutine(SabotageSpillRoutine());
        }
    }

    private IEnumerator SabotageSpillRoutine()
    {
        isDispensing = true;

        if (normalFoodVisual != null) normalFoodVisual.SetActive(false);
        if (overflowFoodVisual != null) overflowFoodVisual.SetActive(true);

        // Prototip: Konsola art arda taşma uyarısı basma veya partikül fırlatma
        for (int i = 0; i < maxSpillCount; i++)
        {
            Debug.Log($"[AutoFeeder] MAMALAR ETRAFA SAÇILIYOR! ({i + 1}/{maxSpillCount})");
            yield return new WaitForSeconds(sabotageDispenseInterval);
        }

        isDispensing = false;
    }

    private void UpdateVisuals()
    {
        if (normalFoodVisual != null) normalFoodVisual.SetActive(currentState == FeederState.Normal);
        if (overflowFoodVisual != null) overflowFoodVisual.SetActive(currentState == FeederState.Sabotaged);
    }

    // Inspector üzerinden test edebilmek için ContextMenu
    [ContextMenu("Test Sabotage")]
    public void TestSabotage() => SetSabotaged();

    [ContextMenu("Test Normal")]
    public void TestNormal() => SetNormal();
}