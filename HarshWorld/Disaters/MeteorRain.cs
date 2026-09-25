using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Planetbase;
using UnityEngine;

namespace HarshWorld
{
    /// <summary>
    /// Mutes every real Unity AudioSource on a meteor the instant it's spawned,
    /// but only while a rain is in progress. Runs as a Postfix right after
    /// MeteorManager.spawnMeteor() finishes (mMeteors.Add(val) is the last line
    /// of the vanilla method), which is before Unity calls Start() on the new
    /// object, so the per-meteor fly/impact sfx never actually plays.
    /// </summary>
    [HarmonyPatch(typeof(MeteorManager), "spawnMeteor")]
    internal static class MeteorManager_spawnMeteor_MutePatch
    {
        private static void Postfix(MeteorManager __instance)
        {
            if (!MeteorRainController.IsRaining) return;

            var meteors = Traverse.Create(__instance).Field("mMeteors").GetValue<List<GameObject>>();
            if (meteors == null || meteors.Count == 0) return;

            GameObject meteor = meteors[meteors.Count - 1];
            AudioSource[] sources = meteor.GetComponentsInChildren<AudioSource>(true);
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].mute = true;
            }
        }
    }

    /// <summary>
    /// Drives a timed meteor rain: spawns many meteors scattered across the map
    /// over `duration` seconds, and plays a single looping ambience clip instead
    /// of dozens of overlapping per-meteor sfx.
    /// </summary>
    public class MeteorRainController : MonoBehaviour
    {
        public static bool IsRaining { get; private set; }

        // Matches the radius the vanilla "no target" spawn branch uses, so the
        // scatter covers the same full-map area the base game already spawns in.
        private const float ScatterRadius = 700f;

        private static MeteorRainController _instance;

        public static MeteorRainController GetOrCreate()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MeteorRainController");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _instance = go.AddComponent<MeteorRainController>();
            }
            return _instance;
        }

        /// <param name="duration">Total length of the rain, in seconds.</param>
        /// <param name="minInterval">Minimum seconds between spawns.</param>
        /// <param name="maxInterval">Maximum seconds between spawns.</param>
        /// <param name="ambienceClipPath">
        /// Full path to a .wav/.ogg file (e.g. Path.Combine(modEntry.Path, "meteorrain.wav")).
        /// Pass null to skip the ambience track.
        /// </param>
        public void StartRain(float duration = 30f, float minInterval = 0.015f, float maxInterval = 0.045f, string ambienceClipPath = null)
        {
            if (IsRaining)
            {
                Debug.Log("[MeteorRain] Rain already in progress, ignoring.");
                return;
            }
            StartCoroutine(RainRoutine(duration, minInterval, maxInterval, ambienceClipPath));
        }

        private IEnumerator RainRoutine(float duration, float minInterval, float maxInterval, string ambienceClipPath)
        {
            IsRaining = true;

            MeteorManager manager = Singleton<MeteorManager>.getInstance();
            Vector3 center = Singleton<TerrainGenerator>.getInstance().getCenter();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float wait = UnityEngine.Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(wait);
                elapsed += wait;

                Vector3 target = center + new Vector3(
                    UnityEngine.Random.Range(-ScatterRadius, ScatterRadius),
                    0f,
                    UnityEngine.Random.Range(-ScatterRadius, ScatterRadius));

                manager.spawnMeteor(target);
            }

            IsRaining = false;
        }
    }
}