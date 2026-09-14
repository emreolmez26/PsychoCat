using UnityEngine;
using UnityEngine.AI;

public class CatAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Patrol Ayarları")]
    public float patrolRadius = 10f;
    public float waitTime = 3f;
    private float timer;

    [Header("Ses Algılama (Investigate)")]
    public float hearingRadius = 15f;
    public LayerMask soundLayer;
    private bool isInvestigating = false;

    [Header("Hack Sistemi (Sabotaj)")]
    public Transform hackTarget; // Şalterin konumu
    public Light roomLight; // Kapatılacak ışık
    public bool startHack = false; // Unity'den test etmek için tetikleyici
    private bool isHacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = waitTime;
    }

    void Update()
    {
        // Eğer hack komutu geldiyse her şeyi bırakıp şaltere gider
        if (startHack)
        {
            agent.SetDestination(hackTarget.position);
            isHacking = true;

            // Şalterin yanına ulaştıysa ışığı kapat
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                if (roomLight != null) roomLight.enabled = false;
                startHack = false; // Görev bitti
                isHacking = false;
                timer = waitTime; // Bekleme süresini sıfırla
            }
            return;
        }

        ListenForSounds();

        if (!isInvestigating && !isHacking)
        {
            PatrolBehavior();
        }
    }

    void ListenForSounds()
    {
        Collider[] heardSounds = Physics.OverlapSphere(transform.position, hearingRadius, soundLayer);

        if (heardSounds.Length > 0)
        {
            Vector3 soundLocation = heardSounds[0].transform.position;
            agent.SetDestination(soundLocation);
            isInvestigating = true;
        }
        else
        {
            if (isInvestigating && agent.remainingDistance <= agent.stoppingDistance)
            {
                isInvestigating = false;
                timer = waitTime;
            }
        }
    }

    void PatrolBehavior()
    {
        timer += Time.deltaTime;

        if (timer >= waitTime && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newDestination = RandomNavSphere(transform.position, patrolRadius, -1);
            agent.SetDestination(newDestination);
            timer = 0;
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}