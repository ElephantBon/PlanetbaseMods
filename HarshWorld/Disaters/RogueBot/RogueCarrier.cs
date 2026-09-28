using Planetbase;

namespace HarshWorld.Disasters.RogueBot
{
    internal class RogueCarrier : RogueBot
    {
        private static RogueCarrier instance;

        public static RogueCarrier getInstance()
        {
            if(instance == null)
            {
                instance = new RogueCarrier();
            }
            return instance;
        }

        public RogueCarrier()
        {
            mIcon = ResourceUtil.loadIconColor("Specializations/icon_bot_carrier");
            mModel = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabBotCarrier");
        }
    }
}
