using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotVacuum : MonoBehaviour
{
    [Header("Wander Settings")]
    [Tooltip("Süpürgenin kendi etrafında rastgele hedef arayacağı yarıçap (metre)")]
    [SerializeField] private float wanderRadius = 6f;
    [Tooltip("Hedefe ulaştığında bekleyeceği süre")]
    [SerializeField] private float waitTimeAtPoint = 1.0f;
    [SerializeField] private float stopDistance = 0.5f;

    [Header("State")]
    [SerializeField] private bool isRunning = true;

    private NavMeshAgent agent;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (isRunning && agent != null && agent.isOnNavMesh)
        {
            SetNewRandomDestination();
        }
    }

    private void Update()
    {
        // Agent aktif değilse veya durdurulduysa işlem yapma
        if (!isRunning || agent == null || !agent.isOnNavMesh) return;

        // Hedefe vardı mı kontrolü
        if (!agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    SetNewRandomDestination();
                }
            }
        }
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Seçilen rastgele koordinata en yakın geçerli NavMesh noktasını bul
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    // Ana Panelden çağrılacak kontrol metodları
    public void ToggleVacuum()
    {
        SetVacuumState(!isRunning);
    }

    public void SetVacuumState(bool state)
    {
        isRunning = state;
        if (agent == null || !agent.isOnNavMesh) return;

        if (isRunning)
        {
            agent.isStopped = false;
            SetNewRandomDestination();
        }
        else
        {
            agent.isStopped = true;
        }
    }

    public bool IsRunning() => isRunning;

    // Editörde süpürgenin dolaşma alanını görsel olarak görmek için (opsiyonel)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}