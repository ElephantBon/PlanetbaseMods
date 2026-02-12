using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;


namespace ColonistLimit
{
    public class Main : ModBase
    {
        public static RefInt colonistLimit = new RefInt(0);

        public static new void Init(ModEntry modEntry)
        {
            InitializeMod(new Main(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
        }
    }
}
