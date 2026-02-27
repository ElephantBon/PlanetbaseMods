using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StorageGuru.Patches
{
    internal class NuclearPlantPatch
    {
        private const string ModName = "NuclearPlant";

        private static bool? isLoaded;

        // Cache
        private static Assembly assembly;
        private static Type typePower;

        internal static bool IsLoaded()
        {
            if(isLoaded == null) {
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == ModName);
                isLoaded = assembly != null;
            }
            return isLoaded.Value;
        }

        internal static bool IsPower(ResourceType type)
        {
            if(typePower == null) {
                typePower = assembly.GetType(ModName+".Objects.ResourcePower");
                if(typePower == null)
                    StorageGuru.ModEntry.Logger.Log("cannot find "+nameof(typePower));
            }
            return typePower.IsInstanceOfType(type);
        }
    }
}
