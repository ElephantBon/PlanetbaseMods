using HarmonyLib;
using HeyHowAboutTheBasement.DTOs;
using Planetbase;
using System;
using UnityEngine;
using Module = Planetbase.Module;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using HeyHowAboutTheBasement.Models;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Connection), nameof(Connection.canLink), new Type[] { typeof(Module), typeof(Module), typeof(Vector3), typeof(Vector3) })]
    internal class canLinkPatch
    {
        public static bool Prefix(ref bool __result, Module m1, Module m2, Vector3 position1, Vector3 position2)
        {
            // Tunnel construction can only connect to non-dead-end module
            Module checkDeadEndModule = null;
            if(m1.getModuleType() is ModuleTypeTunnelConstruction)
                checkDeadEndModule = m2;
            else
            if(m2.getModuleType() is ModuleTypeTunnelConstruction)
                checkDeadEndModule = m1;

            if(checkDeadEndModule != null
            && checkDeadEndModule.hasFlag(ModuleType.FlagDeadEnd)) {
                __result = false;
                return false;
            }

            float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();

            #region Tunnel
            // Entrance can link entrance
            // Entrance without tunnel connection can link tunnel construction
            var type1 = m1.getModuleType();
            var type2 = m2.getModuleType();
            bool maybeCanLink = (type1 is ModuleTypeTunnelEntrance && type2 is ModuleTypeTunnelEntrance);
            if(maybeCanLink)
                Debug.Log("HeyHowAboutTheBasement: two entrances.");

            if(!maybeCanLink) {
                Module moduleEntrance = null;

                // One of the modules is entrance
                if(type1 is ModuleTypeTunnelEntrance && type2 is ModuleTypeTunnelConstruction)
                    moduleEntrance = m1;
                else
                if(type2 is ModuleTypeTunnelEntrance && type1 is ModuleTypeTunnelConstruction)
                    moduleEntrance = m2;

                if(moduleEntrance != null) {
                    // Check entrance has tunnel connection
                    //Debug.Log("HeyHowAboutTheBasement: entrance linking construction.");
                    maybeCanLink = true;
                    foreach(var construction in moduleEntrance.getLinks())
                        if(construction is Module module && module.getModuleType() is ModuleTypeTunnelEntrance) {
                            maybeCanLink = false; // The entrance is already linked to another entrance, means the tunnel is completed
                            //Debug.Log("HeyHowAboutTheBasement: the entrance has tunnel connection.");
                            break;
                        }
                }
            }

            if(maybeCanLink) { 
                // Tunnel entrance can connect each other regardless distance, 

                // Check if there are any building on connection
                float num = Mathf.Min(Mathf.Min(m1.getRadius(), m2.getRadius()), Module.ValidSizes[2] * 0.5f);
                var mRadius = num * 0.5f;
                Vector3 vector = position2 - position1;
                vector.y = 0f;
                vector.Normalize();
                Vector3 vector2 = ConnectionLineData.calculateConnectionPoint(m1, m2.getPosition(), mRadius);
                Vector3 vector3 = ConnectionLineData.calculateConnectionPoint(m2, m1.getPosition(), mRadius);

                position1 = vector2 + vector;
                position2 = vector3 - vector;
                var radius = mRadius * 1.6f;
                float magnitude = (position2 - position1).magnitude;
                Vector3 normalized = (position2 - position1).normalized;
                const float minDist = 3f;
                for(float n = radius; n < magnitude; n += radius) {
                    var position = position1 + normalized * n;

                    // Must be mountain level
                    Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(position));
                    int layerMask = 256;
                    if(Physics.Raycast(ray, out var hitInfo, 150f, layerMask)) {
                        float heightDiff = hitInfo.point.y - floorHeight;
                        if(heightDiff <= 3.0f) {
                            // not at mountain level
                            __result = false;
                            return false;
                        }
                    }

                    RaycastHit[] array2 = Physics.SphereCastAll(position + Vector3.up * 20f, radius, Vector3.down, 40f, 4198400);
                    if(array2 == null)
                        continue;

                    for(int k = 0; k < array2.Length; k++) {
                        RaycastHit raycastHit = array2[k];
                        GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                        var dictionary = CoreUtils.GetMember<Construction, Dictionary<GameObject, Construction>>("mConstructionDictionary");
                        Construction construction = dictionary[gameObject];
                        if(construction == null || construction == m1 || construction == m2)
                            continue;

                        if(construction is Connection connection1
                        && (connection1.getModule1() == m1 || connection1.getModule1() == m2)
                        && (connection1.getModule2() == m1 || connection1.getModule2() == m2)) {
                            // Found existing tunnel connection of the two modules
                            __result = false;
                            return false;
                        }

                        float distToConstruction = (position - construction.getPosition()).magnitude - mRadius - construction.getRadius();
                        if(distToConstruction < minDist) {
                            // Check if it's connection in mountain
                            if(construction is Connection connection2
                            && connection2.getModule1().getModuleType() is ModuleTypeTunnelEntrance
                            && connection2.getModule2().getModuleType() is ModuleTypeTunnelEntrance)
                                continue; // Although it's tunnel connection, but still need to check other obstacles
                            else {
                                // Can't build
                                __result = false;
                                return false;
                            }
                        }
                    }
                }


                __result = true;
                return false;
            }
            #endregion

            #region Moving walkway
            if(m1.getModuleType() is ModuleTypeMovingWalkwayController && m2.getModuleType() is ModuleTypeMovingWalkwayController) {
                // Check if any of the two modules is already connected to another MovingWalkwayController, if so, can't link
                if((WalkwayModel.IsLinked(m1) || WalkwayModel.IsLinked(m2)) && !WalkwayModel.IsLinked(m1, m2)) {
                    __result = false; 
                    return false; 
                }

                // Check if there are any building on connection
                float num = Mathf.Min(Mathf.Min(m1.getRadius(), m2.getRadius()), Module.ValidSizes[1] * 0.5f);
                var mRadius = num * 0.5f;
                Vector3 vector = position2 - position1;
                vector.y = 0f;
                vector.Normalize();
                Vector3 vector2 = ConnectionLineData.calculateConnectionPoint(m1, m2.getPosition(), mRadius);
                Vector3 vector3 = ConnectionLineData.calculateConnectionPoint(m2, m1.getPosition(), mRadius);

                position1 = vector2 + vector;
                position2 = vector3 - vector;
                var radius = mRadius * 1.6f;
                float magnitude = (position2 - position1).magnitude;
                Vector3 normalized = (position2 - position1).normalized;
                const float minDist = 3f;
                for(float n = radius; n < magnitude; n += radius) {
                    var position = position1 + normalized * n;

                    // Must be floor level
                    Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(position));
                    int layerMask = 256;
                    if(Physics.Raycast(ray, out var hitInfo, 150f, layerMask)) {
                        float heightDiff = hitInfo.point.y - floorHeight;
                        if(heightDiff > 0.1f || heightDiff < -0.1f) {
                            // not at floor level
                            __result = false;
                            return false;
                        }
                    }

                    RaycastHit[] array2 = Physics.SphereCastAll(position + Vector3.up * 20f, radius, Vector3.down, 40f, 4198400);
                    if(array2 == null)
                        continue;

                    for(int k = 0; k < array2.Length; k++) {
                        RaycastHit raycastHit = array2[k];
                        GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                        var dictionary = CoreUtils.GetMember<Construction, Dictionary<GameObject, Construction>>("mConstructionDictionary");
                        Construction construction = dictionary[gameObject];
                        if(construction == null || construction == m1 || construction == m2)
                            continue;

                        if(construction is Connection connection1
                        && connection1.getModule1() == m1
                        && connection1.getModule2() == m2) {
                            // Found existing tunnel connection of the two modules
                            __result = false;
                            return false;
                        }

                        float distToConstruction = (position - construction.getPosition()).magnitude - mRadius - construction.getRadius();
                        if(distToConstruction < minDist) {
                            // Check if it's connection in mountain
                            if(construction is Connection connection2
                            && (connection2.getModule1().getModuleType() is ModuleTypeMovingWalkwayController
                            && connection2.getModule2().getModuleType() is ModuleTypeMovingWalkwayController))
                                continue; // Although it's tunnel connection, but still need to check other obstacles
                            else {
                                // Can't build
                                __result = false;
                                return false;
                            }
                        }
                    }
                }

                __result = true;
                return false;
            }
            #endregion

            // Original
            return true;
            
        }
    }
}
