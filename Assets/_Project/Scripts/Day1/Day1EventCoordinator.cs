using System;
using PsychoCat.Player;
using PsychoCat.Systems.DayFlow;
using UnityEngine;

namespace PsychoCat.Day1
{
    [DisallowMultipleComponent]
    public sealed class Day1EventCoordinator : MonoBehaviour
    {
        [SerializeField] private DayFlowController dayFlow;
        [SerializeField] private CatAI cat;
        [SerializeField] private AutoFeeder feeder;
        [SerializeField] private RobotVacuum vacuum;
        [SerializeField] private PowerOutageManager power;
        [SerializeField] private FlashlightItem flashlight;
        [SerializeField] private DoorController[] routeDoors = Array.Empty<DoorController>();
        [SerializeField] private string[] taskOrder =
        {
            "feed_cat", "clean_litter", "player_eat", "stop_vacuum",
            "resolve_feeder", "play_with_cat", "pickup_flashlight"
        };

        private bool initialized;
        private bool inLivingRoom;
        private bool vacuumStarted;
        private bool feederRequested;
        private bool feederApplied;
        private bool powerRequested;
        private bool powerApplied;

        public DayFlowController DayFlow => dayFlow;
        public event Action FlowChanged;
        public string Status => dayFlow == null || dayFlow.CurrentDayNumber == 0 ? "Starting Day 1..."
            : dayFlow.CurrentDayNumber != 1 ? "Day 1 complete. Day 2 is outside this integration."
            : powerApplied ? "Night: collect the flashlight, then return to bed."
            : powerRequested ? "The cat is heading to the power switch..."
            : feederRequested && !feederApplied ? "The cat is heading to the feeder..."
            : Done("player_eat") && !vacuumStarted ? "Go to the living room."
            : "Complete the unlocked task.";

        private void OnEnable()
        {
            if (dayFlow == null || cat == null || feeder == null || vacuum == null || power == null || flashlight == null)
            {
                Debug.LogError("Assign all Day 1 coordinator references.", this);
                enabled = false;
                return;
            }
            dayFlow.DayStarted += OnDayStarted;
            dayFlow.TaskStateChanged += OnTaskChanged;
            cat.SabotageCompleted += OnSabotageCompleted;
            if (dayFlow.CurrentDayNumber == 1)
            {
                if (!initialized) OnDayStarted(1);
                else Synchronize();
            }
        }

        private void OnDisable()
        {
            if (dayFlow != null)
            {
                dayFlow.DayStarted -= OnDayStarted;
                dayFlow.TaskStateChanged -= OnTaskChanged;
            }
            if (cat != null)
            {
                cat.SabotageCompleted -= OnSabotageCompleted;
                // Cancel owned in-flight work so a disabled listener cannot miss completion.
                if ((feederRequested && !feederApplied) || (powerRequested && !powerApplied))
                    cat.SetState(CatState.Patrol);
            }
            if (!feederApplied) feederRequested = false;
            if (!powerApplied) powerRequested = false;
        }

        private void OnDayStarted(int day)
        {
            if (day != 1) { FlowChanged?.Invoke(); return; }
            initialized = true;
            vacuumStarted = feederRequested = feederApplied = powerRequested = powerApplied = false;
            foreach (DoorController door in routeDoors)
                if (door != null) door.Interact(gameObject);
            cat.WakeAndLeadPlayer();
            FlowChanged?.Invoke();
        }

        private bool Done(string id) => dayFlow != null && dayFlow.TryGetTask(id, out DailyTask task) &&
            task.State == DailyTaskState.Completed;

        private bool EarlierRequiredComplete(string id)
        {
            foreach (string earlier in taskOrder)
            {
                if (earlier == id) return true;
                if (!dayFlow.TryGetTask(earlier, out DailyTask task) ||
                    (task.IsRequired && task.State != DailyTaskState.Completed)) return false;
            }
            return false;
        }

        public bool IsTaskUnlocked(string id)
        {
            if (!isActiveAndEnabled || dayFlow == null || dayFlow.CurrentDayNumber != 1 ||
                !dayFlow.TryGetTask(id, out DailyTask task) || task.State == DailyTaskState.Completed ||
                !EarlierRequiredComplete(id)) return false;
            return id == "stop_vacuum" ? vacuumStarted : id == "resolve_feeder" ? feederApplied :
                id == "pickup_flashlight" ? powerApplied : true;
        }

        public bool CompleteTask(string id)
        {
            if (!IsTaskUnlocked(id)) return false;
            if (id == "stop_vacuum" && vacuum.IsRunning()) return false;
            if (id == "pickup_flashlight" && flashlight.gameObject.activeSelf) return false;
            return dayFlow.CompleteTask(id);
        }

        private void OnTaskChanged(DailyTask task) => Synchronize();

        private void Synchronize()
        {
            if (!isActiveAndEnabled || dayFlow.CurrentDayNumber != 1) return;
            if (Done("player_eat") && EarlierRequiredComplete("stop_vacuum") && inLivingRoom && !vacuumStarted)
            {
                vacuumStarted = true;
                vacuum.SetVacuumState(true);
                cat.TriggerVacuumEvent();
            }
            if (Done("stop_vacuum") && EarlierRequiredComplete("resolve_feeder") && !feederRequested)
            {
                feederRequested = true;
                cat.StopVacuumEvent();
                cat.TriggerFeederSabotage();
            }
            if (Done("play_with_cat") && EarlierRequiredComplete("pickup_flashlight") && !powerRequested)
            {
                powerRequested = true;
                cat.TriggerSwitchSabotage();
            }
            FlowChanged?.Invoke();
        }

        private void OnSabotageCompleted(CatSabotageKind kind, Transform target)
        {
            if (dayFlow.CurrentDayNumber != 1) return;
            if (kind == CatSabotageKind.Feeder && target == cat.feederTarget && feederRequested && !feederApplied)
            {
                feederApplied = true;
                feeder.SetSabotaged();
            }
            else if (kind == CatSabotageKind.Power && target == cat.switchTarget && powerRequested && !powerApplied)
            {
                powerApplied = true;
                power.CutPower();
            }
            FlowChanged?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerMovement>() == null) return;
            inLivingRoom = true;
            Synchronize();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PlayerMovement>() != null) inLivingRoom = false;
        }
    }
}
