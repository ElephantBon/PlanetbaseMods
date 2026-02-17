using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;

namespace WhereTheDeadBodies
{
    [HarmonyPatch(typeof(Character), "updateAll")]
    public class updateAllPatch
    {
        public static bool Prefix(float timeStep, int frameIndex)
        {
            var mCharacters = CoreUtils.GetMember<Character, List<Character>>("mCharacters");
            var mPendingDestruction = CoreUtils.GetMember<Character, List<Character>>("mPendingDestruction");

            float timeStep2 = timeStep * 64f;
            int count = mCharacters.Count;
            for(int i = 0; i < count; i++) {
                mCharacters[i].update(timeStep);
            }

            for(int j = 0; j < count; j++) {
                Character character = mCharacters[j];
                if(!character.isDead() && (character.getId() & 0x3F) == frameIndex) {
                    character.tick(timeStep2);
                }
            }

            int count2 = mPendingDestruction.Count;
            for(int k = 0; k < count2; k++) {
                Character character = mPendingDestruction[k];

                Corpse.CreateFromCharacter(character);

                character.destroy();
            }

            mPendingDestruction.Clear();

            // Check
            if(CoreUtils.GetMember<Character, List<Character>>("mPendingDestruction").Count > 0)
                throw new Exception("SetValue Required");

            return false; // Stop vanilla mechanism
        }
    }
}
