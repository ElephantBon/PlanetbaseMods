using Planetbase;
using System.Collections.Generic;

namespace HarshWorld.Disasters.RogueBot
{
    public class RogueBotAi : BaseAi
    {
        private static RogueBotAi mRogueBotAi;

        public RogueBotAi()
        {
            mIdleRules = new List<AiRule>();
            mTargetRules = new List<AiTargetRule>();
            mIdleRules.Add(new AiRuleGoAttackColonistMelee());
            mIdleRules.Add(new AiRuleGoInterior());
            mIdleRules.Add(new AiRuleWanderInterior());
            mTargetRules.Add(new AiRuleAirlockInteraction());
            mTargetRules.Add(new AiRuleCombat());
            mTargetRules.Add(new AiRuleSetIdle());
        }

        public static RogueBotAi getInstance()
        {
            if (mRogueBotAi == null)
            {
                mRogueBotAi = new RogueBotAi();
            }
            return mRogueBotAi;
        }
    }
}
