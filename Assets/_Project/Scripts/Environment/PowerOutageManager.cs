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

    private readonly Dictionary<Light, bool> previousHouseLights = new Dictionary<Light, bool>();
    private readonly Dictionary<GameObject, bool> previousCandles = new Dictionary<GameObject, bool>();
    private Light previousSun;
    private float previousSunIntensity;
    private Color previousSunColor;
    private UnityEngine.Rendering.AmbientMode previousAmbientMode;
    private Color previousAmbientLight;
    private Color previousAmbientSky;
    private Color previousAmbientEquator;
    private Color previousAmbientGround;
    private bool hasPowerSnapshot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [ContextMenu("Trigger Power Cut")]
    public void CutPower()
    {
        // Repeated cuts must not replace the original state with the outage state.
        if (hasPowerSnapshot) return;

        previousHouseLights.Clear();
        foreach (var light in houseLights)
            if (light != null) previousHouseLights[light] = light.enabled;
        previousCandles.Clear();
        foreach (var candle in emergencyCandleLights)
            if (candle != null) previousCandles[candle] = candle.activeSelf;

        previousSun = directionalSun;
        if (previousSun != null)
        {
            previousSunIntensity = previousSun.intensity;
            previousSunColor = previousSun.color;
        }
        previousAmbientMode = RenderSettings.ambientMode;
        previousAmbientLight = RenderSettings.ambientLight;
        previousAmbientSky = RenderSettings.ambientSkyColor;
        previousAmbientEquator = RenderSettings.ambientEquatorColor;
        previousAmbientGround = RenderSettings.ambientGroundColor;
        hasPowerSnapshot = true;
        isPowerCut = true;

        // 1. Ev ışıklarını kapat
        foreach (var l in houseLights)
        {
            if (l != null) l.enabled = false;
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;

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
        if (!hasPowerSnapshot) return;
        isPowerCut = false;

        foreach (var entry in previousHouseLights)
        {
            if (entry.Key != null) entry.Key.enabled = entry.Value;
        }

        if (previousSun != null)
        {
            previousSun.intensity = previousSunIntensity;
            previousSun.color = previousSunColor;
        }

        RenderSettings.ambientMode = previousAmbientMode;
        RenderSettings.ambientLight = previousAmbientLight;
        RenderSettings.ambientSkyColor = previousAmbientSky;
        RenderSettings.ambientEquatorColor = previousAmbientEquator;
        RenderSettings.ambientGroundColor = previousAmbientGround;

        foreach (var entry in previousCandles)
        {
            if (entry.Key != null) entry.Key.SetActive(entry.Value);
        }
        previousHouseLights.Clear();
        previousCandles.Clear();
        previousSun = null;
        hasPowerSnapshot = false;

        Debug.Log("<color=green>[PowerOutage] Elektrikler geri geldi.</color>");
    }

    public bool IsPowerCut() => isPowerCut;
}
