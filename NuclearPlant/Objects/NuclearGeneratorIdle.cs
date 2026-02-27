using PlanetbaseModUtilities;

namespace NuclearPlant.Objects
{
    public class NuclearGeneratorIdle : NuclearGenerator
    {
        public NuclearGeneratorIdle()
        {
            mPowerGeneration = 0;
            mWaterGeneration = 0;
        }

        public static new void RegisterString()
        {
            StringUtils.RegisterString("component_nuclear_generator_idle", Name);
        }
    }
}
