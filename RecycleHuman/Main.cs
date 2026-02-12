using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace RecycleHuman
{
    public class Main : ModBase
    {
        public const float moraleDecayPercentage = 0.10f; // Percentage of all colonists' morale decrease by 10% of their current morale when a human is recycled

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
