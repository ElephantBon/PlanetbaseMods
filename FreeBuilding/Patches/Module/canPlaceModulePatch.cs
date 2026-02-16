using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace FreeBuilding.Patches
{
    [HarmonyPatch(typeof(Module), nameof(Module.canPlaceModule))]
    public class canPlaceModulePatch
    {
        public static bool Prefix(Module __instance, ref bool __result, Vector3 position, Vector3 normal, float size)
        {
            if(FreeBuilding.freeModeEnabled) {
                __result = CustomCanPlaceModule(__instance, position, normal, size);
                return false;
            }
            else {
                return true;    // Vanilla replacement method
            }
        }

        private static bool CustomCanPlaceModule(Module __instance, Vector3 position, Vector3 normal, float size)
        { 
            float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
            float heightDiff = position.y - floorHeight;

            bool isMine = __instance.hasFlag(ModuleType.FlagMine);
            if(isMine) {
                if(heightDiff < 1f || heightDiff > 3f) {
                    // mine must be a little elevated
                    return false;
                }
            }
            else if(heightDiff > 0.1f || heightDiff < -0.1f) {
                // not at floor level
                return false;
            }

            // here we're approximating the circumference of the structure with 8 points
            // and will check that all these points are level with the floor
            float reducedRadius = size * 0.75f;
            float angledReducedRadius = reducedRadius * 1.41421354f * 0.5f;
            Vector3[] circumference = new Vector3[]
            {
                position + new Vector3(reducedRadius, 0f, 0f),
                position + new Vector3(-reducedRadius, 0f, 0f),
                position + new Vector3(0f, 0f, reducedRadius),
                position + new Vector3(0f, 0f, -reducedRadius),
                position + new Vector3(angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(angledReducedRadius, 0f, -angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, -angledReducedRadius)
            };

            if(isMine) {
                // above we verified that it is a bit elevated
                // now make sure that at least one point is near level ground
                bool isValid = false;
                for(int i = 0; i < circumference.Length; i++) {
                    Vector3 floor;
                    PhysicsUtil.findFloor(circumference[i], out floor, 256);
                    if(floor.y < floorHeight + 1f || floor.y > floorHeight - 1f) {
                        isValid = true;
                        break;
                    }
                }

                if(!isValid) {
                    return false;
                }
            }
            else {
                // Make sure all points are near level ground
                for(int j = 0; j < circumference.Length; j++) {
                    Vector3 floor;
                    PhysicsUtil.findFloor(circumference[j], out floor, 256);
                    if(floor.y > floorHeight + 2f || floor.y < floorHeight - 1f) {
                        return false;
                    }
                }
            }

            //position.y = floorHeight;

            // Can only be 375 units away from center of map, but allow placement up to 420 with a warning
            var totalSize = CoreUtils.GetMember<TerrainGenerator, float>("TotalSize");
            Vector2 mapCenter = new Vector2(totalSize, totalSize) * 0.5f;
            float distToCenter = (mapCenter - new Vector2(position.x, position.z)).magnitude;
            FreeBuilding.placingBeyondBorder = (distToCenter > 375f);
            if(distToCenter > 450f) {
                return false;
            }

            // anyPotentialLinks limits connection to 20 (on top of some other less relevant checks)
            if(Construction.getCount() > 1 && !CoreUtils.InvokeMethod<Module, bool>("anyPotentialLinks", __instance, position)) {
                return false;
            }

            RaycastHit[] array2 = Physics.SphereCastAll(position + Vector3.up * 20f, size * 0.5f + FreeBuilding.settings.MinimumDistance, Vector3.down, 40f, 4198400);
            if(array2 != null) {
                for(int k = 0; k < array2.Length; k++) {
                    RaycastHit raycastHit = array2[k];
                    GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    Construction construction = Construction.find(gameObject);
                    if(construction != null) {
                        //if (construction is Connection)
                        //{
                        //    return false;
                        //}
                        float distToConstruction = (position - construction.getPosition()).magnitude - __instance.getRadius() - construction.getRadius();
                        if(distToConstruction < FreeBuilding.settings.MinimumDistance) {
                            return false;
                        }
                    }
                    else {
                        Debug.LogWarning("Not hitting construction: " + gameObject.name);
                    }
                }
            }

            // Check that it's away from the ship
            if(Physics.CheckSphere(position, size * 0.5f + FreeBuilding.settings.MinimumDistance + 1f, 65536)) {
                return false;
            }

            // Check that it doesn't overlap materials
            //if (Physics.CheckSphere(position, radius * 0.5f + 2f, 1024))
            //{
            //    return false;
            //}

            // This is to rotate the mine. We're setting the mine as auto-rotate instead
            //if (isMine)
            //{
            //    Vector3 vector3 = new Vector3(normal.x, 0f, normal.z);
            //    Vector3 normalized = vector3.normalized;
            //    if (Vector3.Dot(this.mObject.transform.forward, normalized) < 0.8660254f)
            //    {
            //        this.mObject.transform.forward = normalized;
            //    }
            //}

            return true;
        }
    }

}
