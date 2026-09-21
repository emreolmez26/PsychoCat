using System.Collections.Generic;
using UnityEngine;

public class PowerOutageManager : MonoBehaviour
{
    public static PowerOutageManager Instance { get; private set; }

    [Header("House Lights")]
    [Tooltip("Evdeki kapatılacak tüm oda Spot/Point ışıkları")]
    [SerializeField] private List<Light> houseLights = new List<Light>();

    [Header("Atmosphere / Night Settings")]
    [SerializeField] private Light directionalSun;
    [SerializeField] private Color nightAmbientColor = new Color(0.02f, 0.02f, 0.05f);
    [Tooltip("Varsa mum ışıkları veya acil durum cılız ışıkları")]
    [SerializeField] private List<GameObject> emergencyCandleLights = new List<GameObject>();

    [Header("State")]
    [SerializeField] private bool isPowerCut = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [ContextMenu("Trigger Power Cut")]
    public void CutPower()
    {
        isPowerCut = true;

        // 1. Ev ışıklarını kapat
        foreach (var l in houseLights)
        {
            if (l != null) l.enabled = false;
        }

        // 2. Ay ışığı / Dış ışığı kıs
        if (directionalSun != null)
        {
            directionalSun.intensity = 0.05f;
            directionalSun.color = new Color(0.3f, 0.4f, 0.7f); // Soğuk gece mavisi
        }

        // 3. Ortam ışığını (Ambient) karart
        RenderSettings.ambientLight = nightAmbientColor;

        // 4. Mumları / acil durum ışıklarını yak
        foreach (var candle in emergencyCandleLights)
        {
            if (candle != null) candle.SetActive(true);
        }

        Debug.Log("<color=red>[PowerOutage] Elektrikler kesildi! Ev karanlığa gömüldü.</color>");
    }

    [ContextMenu("Restore Power")]
    public void RestorePower()
    {
        isPowerCut = false;

        foreach (var l in houseLights)
        {
            if (l != null) l.enabled = true;
        }

        if (directionalSun != null)
        {
            directionalSun.intensity = 1.0f;
            directionalSun.color = Color.white;
        }

        foreach (var candle in emergencyCandleLights)
        {
            if (candle != null) candle.SetActive(false);
        }

        Debug.Log("<color=green>[PowerOutage] Elektrikler geri geldi.</color>");
    }

    public bool IsPowerCut() => isPowerCut;
}