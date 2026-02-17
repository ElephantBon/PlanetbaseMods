using HarmonyLib;
using HeyHowAboutTheBasement.Models;
using Planetbase;
using UnityEngine;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Connection), nameof(Connection.onBuilt))]
    internal class onBuiltPatch
    {
        public static void Postfix(Connection __instance)
        {
            if(WalkwayModel.IsWalkway(__instance)) 
                addWalkwayPath(__instance);
        }

        private static void addWalkwayPath(Connection instance)
        {
            var prefabPathFloor = ResourceList.getInstance().PrefabPathFloor[0];
            var mObject = instance.getGameObject();
            var distance = Mathf.Abs(Vector3.Distance(instance.getModule1().getPosition(), instance.getModule2().getPosition()));
            var pathLength = 3f;
            int num = (int)Mathf.Ceil(distance / pathLength);     
            var offset = distance / 2.0f - pathLength / 2.0f + (num * pathLength - distance);
            for(int i=0; i<num; i++) {
                GameObject obj = Object.Instantiate<GameObject>(prefabPathFloor);                
                obj.transform.SetParent(mObject.transform, false);
                obj.transform.localPosition = new Vector3(0, 0.2f, -offset + i * 3f);
                obj.transform.localScale = new Vector3(0.6f, 1f, 1f);                
                obj.disablePhysics();
            }
            
            Vector3 dir = instance.getModule1().getPosition() - instance.getModule2().getPosition();
            var rotation = Quaternion.LookRotation(dir, Vector3.up);
            addWalkwayController(instance.getModule1(), rotation);
            addWalkwayController(instance.getModule2(), rotation);
        }

        private static void addWalkwayController(Module module, Quaternion rotation)
        {
            GameObject obj = Object.Instantiate<GameObject>(ContentManager.PrefabMovingWalkwayController);
            obj.name = WalkwayModel.NAME_CONTROLLER;
            obj.transform.SetParent(module.getGameObject().transform, false);
            obj.transform.rotation = rotation;
            obj.disablePhysics();
        }
    }
}
