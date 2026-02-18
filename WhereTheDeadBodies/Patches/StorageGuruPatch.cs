using System;
using System.Linq;

namespace WhereTheDeadBodies.Patches
{
    internal class StorageGuruPatch
    {
        private static bool? isLoaded;
        public static bool IsLoaded()
        {
            if(isLoaded == null)
                isLoaded = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "StorageGuru");
            return isLoaded.Value;
        }
    }
}
