using Planetbase;

namespace HarshWorld.Disasters.RogueBot
{
    internal class RogueConstructor : RogueBot
    {
        private static RogueConstructor instance;

        public static RogueConstructor getInstance()
        {
            if(instance == null)
            {
                instance = new RogueConstructor();
            }
            return instance;
        }

        public RogueConstructor()
        {
            mIcon = ResourceUtil.loadIconColor("Specializations/icon_bot_constructor");
            mModel = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabBotConstructor");
        }
    }
}
