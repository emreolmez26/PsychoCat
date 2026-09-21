using System;
using UnityEngine;
using UnityEngine.AI;

// Explicit values preserve existing serialized Day 1 states.
public enum CatState { Patrol = 0, LeadPlayer = 1, WatchPlayer = 2, RidingVacuum = 3, Sabotage = 4, Investigate = 5 }
public enum CatSabotageKind { Feeder, Power, LegacyHack }

[RequireComponent(typeof(NavMeshAgent))]
public class CatAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public CatState currentState = CatState.Patrol;

    [Header("CAT-006: Wake & Lead")]
    public Transform player;
    public Transform foodBowl;
    public float maxDistanceToPlayer = 5f;
    [Header("CAT-007: Watch & Teleport")]
    public Transform teleportTarget;
    [Header("CAT-008: Robot Vacuum")]
    public Transform robotVacuum;
    [Header("CAT-009 & 010: Sabotage")]
    public Transform feederTarget;
    public Transform switchTarget;
    [Header("Patrol")]
    public float patrolRadius = 10f;
    public float waitTime = 3f;
    [Header("Sound Perception")]
    public float hearingRadius = 15f;
    public LayerMask soundLayer;
    [Header("Week 1 Hack Compatibility")]
    public Transform hackTarget;
    public Light roomLight;
    public bool startHack;
    [SerializeField, Min(0.1f)] private float navMeshRecoveryRadius = 2f;

    public event Action<CatSabotageKind, Transform> SabotageCompleted;

    private CatState activeState;
    private bool stateInitialized;
    private float timer;
    private bool hasDestination;
    private Vector3 requestedDestination;
    private Vector3 destination;
    private Vector3 soundPosition;
    private Transform currentSabotageTarget;
    private CatSabotageKind sabotageKind;
    private CatState afterSabotage;
    private Light legacyHackLight;
    private NavMeshPath path;

    private bool AgentReady => agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh;
    private NavMeshQueryFilter NavigationFilter => new NavMeshQueryFilter
    {
        agentTypeID = agent.agentTypeID,
        areaMask = agent.areaMask
    };

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    private void Update()
    {
        // Inspector/direct-field callers use the same state exit path.
        if (!stateInitialized || currentState != activeState) SetState(currentState);
        if (startHack)
        {
            startHack = false;
            TriggerHack(hackTarget, roomLight);
        }
        if (currentState == CatState.RidingVacuum)
        {
            if (robotVacuum == null) { StopVacuumEvent(); return; }
            transform.SetPositionAndRotation(robotVacuum.position + Vector3.up * 1.1f, robotVacuum.rotation);
            return;
        }
        if (!EnsureAgent()) return;

        // Scripted sequences retain priority over ambient sounds.
        bool heardSound = false;
        if (currentState == CatState.Patrol || currentState == CatState.Investigate)
            heardSound = ListenForSounds();

        switch (currentState)
        {
            case CatState.Patrol:
                timer += Time.deltaTime;
                if (timer >= waitTime && (!hasDestination || ReachedDestination()))
                {
                    MoveTo(transform.position + UnityEngine.Random.insideUnitSphere * patrolRadius, patrolRadius);
                    timer = 0f;
                }
                break;
            case CatState.Investigate:
                if (!MoveTo(soundPosition) || (!heardSound && ReachedDestination()))
                    SetState(CatState.Patrol);
                break;
            case CatState.LeadPlayer:
                LeadPlayerBehavior();
                break;
            case CatState.WatchPlayer:
                agent.isStopped = true;
                Face(player, 3f);
                break;
            case CatState.Sabotage:
                SabotageBehavior();
                break;
        }
    }

    public void SetState(CatState state)
    {
        if (!Enum.IsDefined(typeof(CatState), state)) return;
        if (stateInitialized && activeState == state && currentState == state) return;
        if (AgentReady) { agent.ResetPath(); agent.isStopped = false; }
        hasDestination = false;
        activeState = currentState = state;
        stateInitialized = true;
        timer = 0f;
        if (state != CatState.Sabotage)
        {
            currentSabotageTarget = null;
            legacyHackLight = null;
        }
        if (state == CatState.RidingVacuum)
        {
            if (agent != null) agent.enabled = false;
        }
        else if (EnsureAgent())
            agent.isStopped = state == CatState.WatchPlayer;
    }

    private bool EnsureAgent()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent == null || !isActiveAndEnabled) return false;
        if (AgentReady) return true;
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshRecoveryRadius, NavigationFilter))
            return false;
        // Find a valid polygon before enabling; never Warp a disabled agent.
        agent.enabled = false;
        transform.position = hit.position + Vector3.up * agent.baseOffset;
        agent.enabled = true;
        hasDestination = false;
        return AgentReady;
    }

    private bool MoveTo(Vector3 target, float sampleRadius = 2f)
    {
        if (!EnsureAgent()) return false;
        if (hasDestination && (target - requestedDestination).sqrMagnitude < 0.01f &&
            (agent.hasPath || ReachedDestination()))
        {
            agent.isStopped = false;
            return true;
        }
        if (!NavMesh.SamplePosition(target, out NavMeshHit hit, Mathf.Max(0.1f, sampleRadius), NavigationFilter) ||
            !agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete)
        {
            agent.ResetPath();
            hasDestination = false;
            return false;
        }
        hasDestination = agent.SetPath(path);
        requestedDestination = target;
        destination = hit.position;
        agent.isStopped = false;
        return hasDestination;
    }

    private bool ReachedDestination()
    {
        if (!AgentReady || !hasDestination || agent.pathPending || agent.pathStatus != NavMeshPathStatus.PathComplete)
            return false;
        Vector3 offset = agent.nextPosition - destination;
        offset.y = 0f;
        return offset.magnitude <= Mathf.Max(agent.stoppingDistance, 0.1f) + 0.05f &&
            (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.05f);
    }

    private bool ListenForSounds()
    {
        Collider nearest = null;
        float distance = float.PositiveInfinity;
        foreach (Collider sound in Physics.OverlapSphere(transform.position, Mathf.Max(0f, hearingRadius),
                     soundLayer, QueryTriggerInteraction.Collide))
        {
            float candidate = (sound.transform.position - transform.position).sqrMagnitude;
            if (candidate < distance) { nearest = sound; distance = candidate; }
        }
        if (nearest == null) return false;
        soundPosition = nearest.transform.position;
        SetState(CatState.Investigate);
        return true;
    }

    public void WakeAndLeadPlayer() => SetState(CatState.LeadPlayer);

    private void LeadPlayerBehavior()
    {
        if (player == null || foodBowl == null) { SetState(CatState.Patrol); return; }
        if (Vector3.Distance(transform.position, player.position) > maxDistanceToPlayer)
        {
            agent.isStopped = true;
            Face(player, 5f);
            return;
        }
        if (!MoveTo(foodBowl.position) || ReachedDestination()) SetState(CatState.Patrol);
    }

    public void TriggerWatchPlayer() => SetState(CatState.WatchPlayer);

    private void Face(Transform target, float speed)
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);
    }

    [ContextMenu("Işınlanmayı Test Et")]
    public void TeleportOutOfSight() => TryTeleportOutOfSight();

    public bool TryTeleportOutOfSight()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (!isActiveAndEnabled || agent == null || teleportTarget == null ||
            !NavMesh.SamplePosition(teleportTarget.position, out NavMeshHit hit, navMeshRecoveryRadius, NavigationFilter))
            return false;
        if (!AgentReady)
        {
            agent.enabled = false;
            transform.position = hit.position + Vector3.up * agent.baseOffset;
            agent.enabled = true;
        }
        if (!AgentReady || !agent.Warp(hit.position)) return false;
        SetState(CatState.Patrol);
        agent.ResetPath();
        agent.isStopped = false;
        hasDestination = false;
        return true;
    }

    [ContextMenu("Süpürgeye Bin (CAT-008)")]
    public void TriggerVacuumEvent()
    {
        if (robotVacuum != null) SetState(CatState.RidingVacuum);
    }

    public void StopVacuumEvent() => SetState(CatState.Patrol);

    [ContextMenu("Mama Makinesini Boz (CAT-009)")]
    public void TriggerFeederSabotage() => TriggerFeederSabotage(CatState.Patrol);
    public void TriggerFeederSabotage(CatState nextState) => BeginSabotage(feederTarget, CatSabotageKind.Feeder, nextState);

    [ContextMenu("Şalteri İndir (CAT-010)")]
    public void TriggerSwitchSabotage() => TriggerSwitchSabotage(CatState.Patrol);
    public void TriggerSwitchSabotage(CatState nextState) => BeginSabotage(switchTarget, CatSabotageKind.Power, nextState);

    public void TriggerHack(Transform targetSwitch, Light targetLight)
    {
        if (!BeginSabotage(targetSwitch, CatSabotageKind.LegacyHack, CatState.Patrol)) return;
        hackTarget = targetSwitch;
        roomLight = legacyHackLight = targetLight;
    }

    private bool BeginSabotage(Transform target, CatSabotageKind kind, CatState nextState)
    {
        if (target == null || !isActiveAndEnabled) return false;
        SetState(CatState.Sabotage);
        hasDestination = false;
        currentSabotageTarget = target;
        sabotageKind = kind;
        afterSabotage = nextState == CatState.Sabotage || nextState == CatState.Investigate ||
            !Enum.IsDefined(typeof(CatState), nextState) ? CatState.Patrol : nextState;
        legacyHackLight = null;
        return true;
    }

    private void SabotageBehavior()
    {
        if (currentSabotageTarget == null || !MoveTo(currentSabotageTarget.position))
        {
            SetState(CatState.Patrol);
            return;
        }
        if (!ReachedDestination()) return;
        Transform completedTarget = currentSabotageTarget;
        CatSabotageKind completedKind = sabotageKind;
        if (completedKind == CatSabotageKind.LegacyHack && legacyHackLight != null)
            legacyHackLight.enabled = false;
        // Exit first so subscribers can safely start another sequence.
        SetState(afterSabotage);
        SabotageCompleted?.Invoke(completedKind, completedTarget);
    }

    private void OnDisable()
    {
        if (AgentReady) { agent.ResetPath(); agent.isStopped = true; }
        hasDestination = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
