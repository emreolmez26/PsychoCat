using UnityEngine;

namespace PsychoCat.Environment
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CollisionSoundEvent : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float minimumImpactSpeed = 2f;
        [SerializeField, Min(0.1f)] private float lifetime = 1.5f;
        [SerializeField, Min(0f)] private float cooldown = 0.25f;
        private float nextAllowedTime;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude < minimumImpactSpeed || Time.time < nextAllowedTime)
                return;

            int soundLayer = LayerMask.NameToLayer("Sound");
            if (soundLayer < 0)
                return;

            nextAllowedTime = Time.time + cooldown;
            var sound = new GameObject("Collision Sound Event");
            sound.layer = soundLayer;
            sound.transform.position = collision.contactCount > 0
                ? collision.GetContact(0).point : transform.position;
            // A small marker lets CatAI's hearing radius determine audibility.
            // A trigger cannot block player movement or cause another impact.
            var marker = sound.AddComponent<SphereCollider>();
            marker.radius = 0.1f;
            marker.isTrigger = true;
            Destroy(sound, lifetime);
        }
    }
}
