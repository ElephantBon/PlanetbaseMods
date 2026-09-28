using Planetbase;

namespace HarshWorld.Disasters.RogueBot
{
    internal class RogueDriller : RogueBot
    {
        private static RogueDriller instance;

        public static RogueDriller getInstance()
        {
            if(instance == null)
            {
                instance = new RogueDriller();
            }
            return instance;
        }

        public RogueDriller()
        {
            mIcon = ResourceUtil.loadIconColor("Specializations/icon_bot_driller");
            mModel = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabBotDriller");
        }
    }
}
