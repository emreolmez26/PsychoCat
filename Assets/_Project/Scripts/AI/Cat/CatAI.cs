using UnityEngine;
using UnityEngine.AI;

// Kedinin girebileceği tüm durumların listesi
public enum CatState
{
    Patrol,         // Normal ev içi dolaşma
    LeadPlayer,     // CAT-006: Oyuncuyu uyandırıp mama kabına götürme
    WatchPlayer,    // CAT-007: Durup oyuncuyu izleme
    RidingVacuum,   // CAT-008: Robot süpürgeye binme
    Sabotage        // CAT-009 & 010: Robot süpürge / Şalter bozma
}

public class CatAI : MonoBehaviour
{
    private NavMeshAgent agent;

    // Oyun başladığında kedi standart olarak dolaşma modunda olur
    public CatState currentState = CatState.Patrol;

    [Header("CAT-006: Wake & Lead Ayarları")]
    public Transform player; // Oyuncunun konumu
    public Transform foodBowl; // Mama kabının konumu
    public float maxDistanceToPlayer = 5f; // Oyuncu bu mesafeden uzaklaşırsa kedi durup bekler

    [Header("CAT-007: Teleport Ayarları")]
    public Transform teleportTarget; // Kedinin ışınlanacağı nokta

    [Header("CAT-008: Robot Vacuum Ayarları")]
    public Transform robotVacuum; // Süpürge objesi

    [Header("CAT-009 & 010: Sabotaj Ayarları")]
    public Transform feederTarget; // Akıllı mama makinesinin konumu
    public Transform switchTarget; // Elektrik şalterinin konumu
    private Transform currentSabotageTarget; // O an hangisini bozuyorsa o hedef

    [Header("Patrol Ayarları")]
    public float patrolRadius = 10f;
    public float waitTime = 3f;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = waitTime;
    }

    void Update()
    {
        // Kedinin o anki ruh hali neyse sadece o fonksiyonu çalıştırır
        switch (currentState)
        {
            case CatState.Patrol:
                PatrolBehavior();
                break;
            case CatState.LeadPlayer:
                LeadPlayerBehavior();
                break;
            case CatState.WatchPlayer:
                WatchPlayerBehavior();
                break;
            case CatState.RidingVacuum:
                RideVacuumBehavior();
                break;
            case CatState.Sabotage:
                SabotageBehavior();
                break;
        }
    }

    // CAT-006: WAKE & LEAD BEHAVIOR
    public void WakeAndLeadPlayer()
    {
        currentState = CatState.LeadPlayer;
    }

    void LeadPlayerBehavior()
    {
        if (player == null || foodBowl == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > maxDistanceToPlayer)
        {
            Debug.Log("Oyuncu geride kaldı, durup onu bekliyorum!");
            agent.isStopped = true;
            Vector3 lookPos = player.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);
        }
        else
        {
            Debug.Log("Oyuncu peşimde, mama kabına yürüyorum...");
            agent.isStopped = false;
            agent.SetDestination(foodBowl.position);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                Debug.Log("Mama kabına ulaştım, normal dolaşmaya dönüyorum!");
                currentState = CatState.Patrol;
            }
        }
    }

    // NORMAL DOLAŞMA (PATROL) BEHAVIOR
    void PatrolBehavior()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newDestination = RandomNavSphere(transform.position, patrolRadius, -1);
            if (newDestination != transform.position) agent.SetDestination(newDestination);
            timer = 0;
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, dist, layermask))
            return navHit.position;
        return origin;
    }

    // CAT-007: WATCH & TELEPORT BEHAVIOR
    public void TriggerWatchPlayer()
    {
        currentState = CatState.WatchPlayer;
    }

    void WatchPlayerBehavior()
    {
        if (player == null) return;

        // Kedi olduğu yerde çakılı kalır
        agent.isStopped = true;

        // Yavaşça ve ürpertici bir şekilde oyuncuya döner
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 3f);

        Debug.Log("Gözlerimi oyuncuya diktim, onu izliyorum...");
    }

    [ContextMenu("Işınlanmayı Test Et")]
    public void TeleportOutOfSight()
    {
        if (teleportTarget == null) return;

        Vector3 safePosition = new Vector3(teleportTarget.position.x, 1f, teleportTarget.position.z);
        agent.Warp(safePosition);

        agent.enabled = true;
        agent.isStopped = false;
        currentState = CatState.Patrol;

        Debug.Log("Kedi çaktırmadan ışınlandı ve devriyeye devam ediyor!");
    }

    // CAT-008: ROBOT VACUUM BEHAVIOR
    [ContextMenu("Süpürgeye Bin (CAT-008)")]
    public void TriggerVacuumEvent()
    {
        currentState = CatState.RidingVacuum;
    }

    void RideVacuumBehavior()
    {
        if (robotVacuum == null) return;

        // Kedinin kendi yürüme motorunu kapat
        agent.enabled = false;

        // Kedi tam süpürgenin merkezine kilitlenir 
        transform.position = robotVacuum.position + new Vector3(0, 1.1f, 0);

        // Yüzünü süpürgenin baktığı yöne çevirir
        transform.rotation = robotVacuum.rotation;

        Debug.Log("Süpürgenin üstündeyim, odayı turluyorum!");
    }

    // CAT-009 & 010: SABOTAGE BEHAVIOR
    [ContextMenu("Mama Makinesini Boz (CAT-009)")]
    public void TriggerFeederSabotage()
    {
        currentSabotageTarget = feederTarget;
        currentState = CatState.Sabotage;
    }

    [ContextMenu("Şalteri İndir (CAT-010)")]
    public void TriggerSwitchSabotage()
    {
        currentSabotageTarget = switchTarget;
        currentState = CatState.Sabotage;
    }

    void SabotageBehavior()
    {
        if (currentSabotageTarget == null) return;

        // Süpürgeden yeni inmiş olabilir
        agent.enabled = true;
        agent.isStopped = false;

        agent.SetDestination(currentSabotageTarget.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;

            Vector3 lookPos = currentSabotageTarget.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);

            Debug.Log(currentSabotageTarget.name + " hedefine ulaştım ve bozuyorum! Ortalık karışacak!");
        }
    }
}