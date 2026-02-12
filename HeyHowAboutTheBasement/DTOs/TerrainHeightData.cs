using UnityEngine;
using Module = Planetbase.Module;

namespace HeyHowAboutTheBasement.DTOs
{
    public class TerrainHeightData
    {
        public Module module1;
        public Module module2;
        public TerrainData terrainData;
        public int num4;
        public int num5;
        public float[,] floats;

        public TerrainHeightData(Module module1, Module module2, TerrainData terrainData, int num4, int num5, float[,] floats)
        {
            this.module1 = module1;
            this.module2 = module2;
            this.terrainData = terrainData;
            this.num4 = num4;
            this.num5 = num5;
            this.floats = floats.Clone() as float[,];
        }
    }
}
