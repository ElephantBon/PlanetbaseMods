using HarmonyLib;
using Planetbase;
using System.Collections.Generic;
using UnityEngine;

namespace HarshWorld.Disasters.RogueBot
{
    [HarmonyPatch(typeof(InteractionCombat))]
    internal class InteractionCombatPatch
    {
        private static Dictionary<Character, GameObject> mDictVfx = new Dictionary<Character, GameObject>();

        [HarmonyPatch("updateAttack")]
        [HarmonyPostfix]
        public static void updateAttackPatch(ref bool __result, Character characterA, Character characterB, float timeStep, float distance)
        {
            if (characterA.getSpecialization() is RogueBot && (!mDictVfx.ContainsKey(characterB) || mDictVfx[characterB] == null))
            {
                // Rogue bot attacking, spawn VFX
                if (characterB is Human && !(characterA.getSpecialization() is RogueConstructor))
                {
                    // Spawn blood splatter VFX for human character
                    mDictVfx[characterB] = VfxHelper.AttachBloodVfx(characterB.getGameObject());
                }
                else
                {
                    // Spawn sparks VFX for non-human character
                    mDictVfx[characterB] = VfxHelper.AttachSparksVfx(characterB.getGameObject());
                }
            }
        }

        [HarmonyPatch("getCharacterAnimations")]
        [HarmonyPrefix]
        public static bool getCharacterAnimationsPatch(Character __instance, ref List<CharacterAnimation> __result, Character character)
        {
            var specialization = character.getSpecialization();
            if (specialization is RogueBot)
            {
                var list = new List<CharacterAnimation>();
                if(specialization is RogueCarrier)
                {
                    list.Add(new CharacterAnimation(CharacterAnimationType.Idle));
                    list.Add(new CharacterAnimation(CharacterAnimationType.CarryHeavy));
                }
                else
                if (specialization is RogueConstructor)
                {
                    list.Add(new CharacterAnimation(CharacterAnimationType.Idle));
                    list.Add(new CharacterAnimation(CharacterAnimationType.Build));
                }
                else
                if (specialization is RogueDriller)
                {
                    list.Add(new CharacterAnimation(CharacterAnimationType.Idle));
                    list.Add(new CharacterAnimation(CharacterAnimationType.BeingRepaired));
                }
                __result = list;
                return false;
            }
            return true;
        }
    }
}
