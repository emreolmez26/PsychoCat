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
    public Transform hackTarget;
    public Light roomLight;
    public bool startHack = false;
    private bool isHacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = waitTime;
    }

    void Update()
    {
        // Inspector üzerinden manuel test etmeye devam edebilmen için
        if (startHack)
        {
            TriggerHack(hackTarget, roomLight);
            startHack = false;
        }

        if (isHacking)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Null check: Eğer ışık atanmamışsa oyun çökmez, sadece hata vermeden geçer
                if (roomLight != null) roomLight.enabled = false;

                isHacking = false;
                timer = waitTime;
            }
            return;
        }

        ListenForSounds();

        if (!isInvestigating && !isHacking)
        {
            PatrolBehavior();
        }
    }

    // Emre'nin istediği, dışarıdan (başka scriptlerden) çağrılabilir Public Hack Metodu
    public void TriggerHack(Transform targetSwitch, Light targetLight)
    {
        if (targetSwitch == null) return; // Null check: Hedef yoksa kodu hiç çalıştırma

        hackTarget = targetSwitch;
        roomLight = targetLight;
        isHacking = true;
        agent.SetDestination(hackTarget.position);
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

            // Eğer NavMesh başarısız olursa kedi olduğu yerde kalır, saçma bir yere gitmez
            if (newDestination != transform.position)
            {
                agent.SetDestination(newDestination);
            }
            timer = 0;
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;

        // Emre'nin istediği kontrol: Geçerli bir nokta bulunursa orayı, bulunamazsa kedinin şu anki konumunu döndür
        if (NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask))
        {
            return navHit.position;
        }

        return origin;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}