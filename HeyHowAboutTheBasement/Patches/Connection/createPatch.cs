using HarmonyLib;
using Planetbase;
using System;
using System.Xml;
using UnityEngine;
using Module = Planetbase.Module;
using PlanetbaseModUtilities;
using HeyHowAboutTheBasement.DTOs;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Connection), nameof(Connection.create), new Type[] { typeof(Module), typeof(Module) })]
    internal class createPatch
    {
        public static bool Prefix(Module module1, Module module2)
        {
            if(module1.getModuleType() is ModuleTypeTunnelEntrance
            && module2.getModuleType() is ModuleTypeTunnelEntrance) 
                HeyHowAboutTheBasement.creatingConnectionTunnelEntrance = true;

            return true;
        }

        public static void Postfix(Module module1, Module module2)
        {
            if(module1.getModuleType() is ModuleTypeTunnelEntrance
            && module2.getModuleType() is ModuleTypeTunnelEntrance)
                HeyHowAboutTheBasement.creatingConnectionTunnelEntrance = false;
        }
    }


    [HarmonyPatch(typeof(Connection), nameof(Connection.create), new Type[] { typeof(Module), typeof(Module), typeof(XmlNode) })]
    internal class deserializeCreatePatch
    {
        public static bool Prefix(Module module1, Module module2)
        {
            // Store heights before flatten
            if(module1.getModuleType() is ModuleTypeTunnelEntrance
            && module2.getModuleType() is ModuleTypeTunnelEntrance) {

                float num = Mathf.Min(Mathf.Min(module1.getRadius(), module2.getRadius()), Module.ValidSizes[2] * 0.5f);
                var mRadius = num * 0.5f;
                Vector3 position = module1.getPosition();
                Vector3 position2 = module2.getPosition();
                Vector3 vector = position2 - position;
                vector.y = 0f;
                vector.Normalize();
                Vector3 vector2 = ConnectionLineData.calculateConnectionPoint(module1, module2.getPosition(), mRadius);
                Vector3 vector3 = ConnectionLineData.calculateConnectionPoint(module2, module1.getPosition(), mRadius);

                TerrainGenerator terrainGenerator = Singleton<TerrainGenerator>.getInstance();
                var mTerrainGrid = CoreUtils.GetMember<TerrainGenerator, Terrain[,]>("mTerrainGrid", terrainGenerator);

                position = vector2 + vector;
                position2 = vector3 - vector;
                var radius = mRadius * 1.6f;
                float magnitude = (position2 - position).magnitude;
                Vector3 normalized = (position2 - position).normalized;
                for(float n = 0f; n < magnitude; n += radius * 0.75f) {
                    var nPosition = position + normalized * n;
                    int num1 = (int)((nPosition.x - radius) / 500f);
                    int num2 = (int)((nPosition.z - radius) / 500f);
                    int num3 = (int)((nPosition.x + radius) / 500f);
                    int num4 = (int)((nPosition.z + radius) / 500f);

                    SaveHeightsData(module1, module2, mTerrainGrid[num1, num2], nPosition, radius);
                    if(num1 != num3)
                        SaveHeightsData(module1, module2, mTerrainGrid[num3, num2], nPosition, radius);
                    if(num2 != num4)
                        SaveHeightsData(module1, module2, mTerrainGrid[num1, num4], nPosition, radius);
                    if(num1 != num3 && num2 != num4)
                        SaveHeightsData(module1, module2, mTerrainGrid[num3, num4], nPosition, radius);
                }
            }

            return true;
        }

        private static void SaveHeightsData(Module module1, Module module2, Terrain terrain, Vector3 position, float radius)
        {
            TerrainData terrainData = terrain.terrainData;
            position -= terrain.transform.position;
            float num2 = 514f * radius / 500f;
            int num3 = Mathf.RoundToInt(num2 * 2f);
            int num4 = Mathf.RoundToInt(257f * position.x / 500f - num2);
            int num5 = Mathf.RoundToInt(257f * position.z / 500f - num2);
            int num7 = Mathf.Clamp(num4 + num3, 0, 257);
            int num8 = Mathf.Clamp(num5 + num3, 0, 257);
            num4 = Mathf.Clamp(num4, 0, 256);
            num5 = Mathf.Clamp(num5, 0, 256);
            int num9 = num7 - num4;
            int num10 = num8 - num5;
            if(num9 <= 0 || num10 <= 0) {
                return;
            }

            float[,] heights = terrainData.GetHeights(num4, num5, num9, num10);
            HeyHowAboutTheBasement.terrainHeightDatas.Add(new TerrainHeightData(module1, module2, terrainData, num4, num5, heights));
        }

    }
}
