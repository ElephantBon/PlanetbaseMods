using Planetbase;
using System;
using System.Linq;
using System.Reflection;
using Module = Planetbase.Module;

namespace StorageGuru.Patches
{
    internal class WhereTheDeadBodiesPatch
    {
        private const string ModName = "WhereTheDeadBodies";

        private static bool? isLoaded;

        // Cache
        private static Assembly assembly;
        private static Type typeCorpse; 
        private static Type typeRemains;
        
        public static bool IsLoaded()
        {
            if(isLoaded == null) { 
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == ModName);
                isLoaded = assembly != null;
            }
            return isLoaded.Value;
        }


        internal static bool IsModuleMorgue(Module module)
        {
            return module.getModuleType().ToString() == ModName+".Objects.ModuleTypeMorgue";
        }
        internal static bool IsCorpse(Resource resource)
        {
            return resource != null && resource.getResourceType().ToString() == ModName+".Objects.Corpse";
        }
        internal static bool IsCorpse(ResourceType type)
        {
            if(typeCorpse == null) { 
                typeCorpse = assembly.GetType(ModName+".Objects.Corpse");
                if(typeCorpse == null) 
                    StorageGuru.ModEntry.Logger.Log("cannot find "+nameof(typeCorpse));
            }
            return typeCorpse.IsInstanceOfType(type);
        }

        internal static bool IsRemains(ResourceType type)
        {
            if(typeRemains == null) { 
                typeRemains = assembly.GetType(ModName+".Objects.Remains");
                if(typeRemains == null)
                    StorageGuru.ModEntry.Logger.Log("cannot find "+nameof(typeRemains));
            }
            return typeRemains.IsInstanceOfType(type);
        }
    }
}
