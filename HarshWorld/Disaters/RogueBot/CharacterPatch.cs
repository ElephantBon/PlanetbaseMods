using HarmonyLib;
using Planetbase;

namespace HarshWorld.Disasters.RogueBot
{
    [HarmonyPatch(typeof(Character))]
    internal class CharacterPatch
    {
        [HarmonyPatch("isEnemy")]
        [HarmonyPrefix]
        public static bool isEnemyPatch(Character __instance, ref bool __result, Character character)
        {
            var mSpecialization = __instance.getSpecialization();            
            if (mSpecialization is RogueBot || character.getSpecialization() is RogueBot)
            {
                __result = mSpecialization != character.getSpecialization();
                return false;
            }
            return true;
        }
    }
}
