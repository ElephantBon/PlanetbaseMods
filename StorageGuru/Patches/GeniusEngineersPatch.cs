using Planetbase;
using System;
using System.Linq;
using System.Reflection;

namespace StorageGuru.Patches
{
    internal class GeniusEngineersPatch
    {
        private const string ModName = "GeniusEngineers";

        private static bool? isLoaded;

        // Cache
        private static Assembly assembly;
        private static Type[] types = new Type[4];

        internal static bool IsLoaded()
        {
            if(isLoaded == null) {
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == ModName);
                isLoaded = assembly != null;
            }
            return isLoaded.Value;
        }

        internal static bool IsExtraBotLife(ResourceType type)
        {
            var i = 0;
            if(types[i] == null) {
                types[i] = assembly.GetType(ModName + ".Objects.ResourceExtraBotLife");
                if(types[i] == null)
                    StorageGuru.ModEntry.Logger.Log("cannot find target type.");
            }
            return types[i].IsInstanceOfType(type);
        }
        internal static bool IsExtraPowerStorage(ResourceType type)
        {
            var i = 1;
            if(types[i] == null) {
                types[i] = assembly.GetType(ModName + ".Objects.ResourceExtraPowerStorage");
                if(types[i] == null)
                    StorageGuru.ModEntry.Logger.Log("cannot find target type.");
            }
            return types[i].IsInstanceOfType(type);
        }
        internal static bool IsExtraVegetableLife(ResourceType type)
        {
            var i = 2;
            if(types[i] == null) {
                types[i] = assembly.GetType(ModName + ".Objects.ResourceExtraVegetableLife");
                if(types[i] == null)
                    StorageGuru.ModEntry.Logger.Log("cannot find target type.");
            }
            return types[i].IsInstanceOfType(type);
        }
        internal static bool IsExtraWaterStorage(ResourceType type)
        {
            var i = 3;
            if(types[i] == null) {
                types[i] = assembly.GetType(ModName + ".Objects.ResourceExtraWaterStorage");
                if(types[i] == null)
                    StorageGuru.ModEntry.Logger.Log("cannot find target type.");
            }
            return types[i].IsInstanceOfType(type);
        }
    }
}
