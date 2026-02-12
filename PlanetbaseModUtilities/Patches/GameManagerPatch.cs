using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    [HarmonyPatch(typeof(GameManager), "onNewGameState")]
    public class GameManagerPatch
    {
        public delegate void GameStateChangedDelegate(GameState gameState);
        public static GameStateChangedDelegate OnGameStateChanged;
        public static bool IsUpdating { get; private set; }

        /// <summary>
        /// Add hook at end of onNewGameState to listen for state changes
        /// </summary>
        [HarmonyPatch("onNewGameState")]
        [HarmonyPostfix]
        public static void onNewGameState(GameState gameState)
        {
            OnGameStateChanged?.Invoke(gameState);
        }

        [HarmonyPatch("fixedUpdate")]
        [HarmonyPostfix]
        public static void fixedUpdate(GameManager __instance, float timeStep)
        {
            IsUpdating = CoreUtils.GetMember<GameManager, GameManager.State>("mState", __instance) == GameManager.State.Updating;
        }
    }
}
