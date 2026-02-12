using PlanetbaseModUtilities;

namespace NuclearPlant.Objects
{
    public class NuclearGeneratorPowered : NuclearGenerator
    {
        public NuclearGeneratorPowered()
        {
            mPowerGeneration = 480000;
            mWaterGeneration = -6000;
        }

        public static new void RegisterString()
        {
            StringUtils.RegisterString("component_nuclear_generator_powered", Name);
        }
    }
}
