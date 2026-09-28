using Planetbase;

namespace HarshWorld.Disasters.RogueBot
{
    public class AiRuleGoAttackColonistMelee : AiRule
    {
        public override bool update(Character character)
        {
            if (character.hasStatusFlag(2))
            {
                Colonist colonist = Colonist.findNearestStanding(character);
                if (colonist != null)
                {
                    Target target = new Target(colonist);
                    target.setRadius(1f);
                    return AiRule.goTarget(character, target);
                }
            }
            return false;
        }
    }

}
