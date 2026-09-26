using System.Collections.Generic;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace HarshWorld
{
    internal interface ICustomScheduledDisaster
    {
        string Id { get; }
        bool IsInProgress { get; }
        float MinScheduleDelay { get; }
        float MaxScheduleDelay { get; }
        bool TryStart();
        void UpdateInProgress(float timeStep);
    }

    public class CustomDisasterManager
    {
        private class ScheduledDisasterState
        {
            public readonly ICustomScheduledDisaster Disaster;
            public float TimeUntilNext = -1f;

            public ScheduledDisasterState(ICustomScheduledDisaster disaster)
            {
                Disaster = disaster;
            }

            public void ResetSchedule()
            {
                TimeUntilNext = UnityEngine.Random.Range(Disaster.MinScheduleDelay, Disaster.MaxScheduleDelay);

                if (ModBase.ModEntry != null)
                {
                    ModBase.ModEntry.Logger.Log("[CustomDisasterManager] ResetSchedule: " + Disaster.Id + " in " + TimeUntilNext.ToString("F2") + "s");
                }
            }
        }

        private static CustomDisasterManager _instance;

        private readonly List<ScheduledDisasterState> _orderedDisasters = new List<ScheduledDisasterState>();
        private readonly Dictionary<string, ScheduledDisasterState> _disastersById = new Dictionary<string, ScheduledDisasterState>();

        public static CustomDisasterManager GetOrCreate()
        {
            if (_instance == null)
            {
                _instance = new CustomDisasterManager();
            }
            return _instance;
        }

        private CustomDisasterManager()
        {
            Register(MeteorRainController.GetOrCreate());
        }

        private void Register(ICustomScheduledDisaster disaster)
        {
            ScheduledDisasterState state = new ScheduledDisasterState(disaster);
            _orderedDisasters.Add(state);
            _disastersById[disaster.Id] = state;
        }

        public void Tick(float timeStep)
        {
            if (!GameManagerPatch.IsUpdating)
            {
                ResetSchedules();
                return;
            }

            bool anyCustomInProgress = false;
            for (int i = 0; i < _orderedDisasters.Count; i++)
            {
                ScheduledDisasterState state = _orderedDisasters[i];
                state.Disaster.UpdateInProgress(timeStep);
                if (state.Disaster.IsInProgress)
                {
                    anyCustomInProgress = true;
                }
            }

            if (anyCustomInProgress || IsVanillaDisasterInProgress())
            {
                return;
            }

            for (int i = 0; i < _orderedDisasters.Count; i++)
            {
                ScheduledDisasterState state = _orderedDisasters[i];
                if (state.TimeUntilNext < 0f)
                {
                    state.ResetSchedule();
                }

                state.TimeUntilNext -= timeStep;
                if (state.TimeUntilNext <= 0f)
                {
                    bool started = state.Disaster.TryStart();
                    state.ResetSchedule();
                    if (started)
                    {
                        break;
                    }
                }
            }
        }

        public bool TryTrigger(string disasterId)
        {
            ScheduledDisasterState state;
            if (!_disastersById.TryGetValue(disasterId, out state))
            {
                return false;
            }

            if (!CanStartNow())
            {
                return false;
            }

            bool started = state.Disaster.TryStart();
            if (started)
            {
                state.ResetSchedule();
            }
            return started;
        }

        private bool CanStartNow()
        {
            if (!GameManagerPatch.IsUpdating)
            {
                return false;
            }

            if (IsVanillaDisasterInProgress())
            {
                return false;
            }

            for (int i = 0; i < _orderedDisasters.Count; i++)
            {
                if (_orderedDisasters[i].Disaster.IsInProgress)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsVanillaDisasterInProgress()
        {
            DisasterManager disasterManager = Singleton<DisasterManager>.getInstance();
            return disasterManager != null && disasterManager.anyInProgress();
        }

        private void ResetSchedules()
        {
            for (int i = 0; i < _orderedDisasters.Count; i++)
            {
                _orderedDisasters[i].TimeUntilNext = -1f;
            }
        }
    }
}
