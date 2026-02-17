using Planetbase;
using System;

namespace HeyHowAboutTheBasement.Models
{
    internal class WalkwayModel
    {
        public const string NAME_CONTROLLER = "Moving Walkway Controller";

        public static bool IsOnFunctionalWalkway(Character character)
        {
            return IsWalkwayEnabled(character.getCurrentConstruction());
        }

        public static bool IsWalkway(Construction construction)
        {
            if(construction != null && construction is Connection connection && connection.isEnabled()) {
                if(connection.getModule1().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule2().getModuleType() is ModuleTypeMovingWalkwayController)
                    return true;
            }
            return false;
        }

        public static bool IsWalkwayEnabled(Construction construction)
        {
            if(construction != null && construction is Connection connection && connection.isEnabled()) {
                if(connection.getModule1().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule2().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule1().isEnabled()
                && connection.getModule2().isEnabled())
                    return true;
            }
            return false;
        }

        public static void DestroyWalkwayController(Module module)
        {
            if(module.getModuleType() is ModuleTypeMovingWalkwayController) {
                var obj = module.getGameObject().transform.Find(NAME_CONTROLLER);
                if(obj != null) {
                    UnityEngine.Object.Destroy(obj.gameObject);
                }
            }
        }

        /// <summary> To test center of rotation of the imported shape </summary>
        public static void DebugRotateController(Module module)
        {
            if(module.getModuleType() is ModuleTypeMovingWalkwayController) {
                var obj = module.getGameObject().transform.Find(NAME_CONTROLLER);
                if(obj != null) {
                    obj.Rotate(0, 1, 0);
                }
            }
        }

        internal static Module GetLinked(Module module)
        {
            if(module.getModuleType() is ModuleTypeMovingWalkwayController) {
                for(int i=0; i<module.getLinkCount(); i++)
                    if(module.getLinkedModule(i).getModuleType() is ModuleTypeMovingWalkwayController)
                        return module.getLinkedModule(i);
            }
            return null;
        }

        internal static bool IsLinked(Module module)
        {
            return GetLinked(module) != null;
        }

        internal static bool IsLinked(Module m1, Module m2)
        {
            return GetLinked(m1) == m2;
        }
    }
}
