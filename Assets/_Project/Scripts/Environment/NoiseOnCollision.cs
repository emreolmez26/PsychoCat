using UnityEngine;

public class NoiseOnCollision : MonoBehaviour
{
    [Header("Çarpma Ayarları")]
    [SerializeField] private float minVelocityForNoise = 2f;
    [SerializeField] private float noiseRadius = 10f;
    [SerializeField] private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        // Yeterli hızla yere veya duvara çarptıysa
        if (collision.relativeVelocity.magnitude >= minVelocityForNoise)
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // Kedi AI ve dinleyiciler için gürültü uyarısı
            Debug.Log($"[NOISE] {gameObject.name} çarpma sesi yaydı! Yarıçap: {noiseRadius}m");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, noiseRadius);
    }
}