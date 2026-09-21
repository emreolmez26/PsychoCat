using UnityEngine;

[RequireComponent(typeof(Light))]
public class CandleFlicker : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.7f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float flickerSpeed = 0.08f;

    private Light candleLight;
    private float timer;

    private void Awake()
    {
        candleLight = GetComponent<Light>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Perlin noise ile yumuşak ve doğal bir alev titreşimi
            candleLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * 8f, 0f));
            timer = flickerSpeed;
        }
    }
}