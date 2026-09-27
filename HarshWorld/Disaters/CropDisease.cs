using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace HarshWorld
{ 
    public class CropDisease : Disaster
    {
        private bool mFirstUpdate = true;

        private bool mInProgress;

        private float mTimeToNext;

        private float mTime;

        private float mTimeLastSpread;

        private bool mAffected;

        private bool mLoggingEnabled = false;

        private Dictionary<ConstructionComponent, GameObject> mDictVFX = new Dictionary<ConstructionComponent, GameObject>();

        public CropDisease()
        {
            decideNextTime();
        }

        public override void destroy()
        {
        }

        private void decideNextTime()
        {
            mTimeToNext = Random.Range(Main.settings.MinimumDurationBetweenCropDiseases, Main.settings.MaximumDurationBetweenCropDiseases) * 2;
        }

        public override void onTimeScaleChanged(float timeScale, bool paused)
        {
        }

        public override void update(float timeStep)
        {
            if (mInProgress)
            {
                Singleton<MusicManager>.getInstance().onTension();
                if (mFirstUpdate)
                {
                    onStart();
                }
                var endEarly = false;
                mTime += timeStep;
                mTimeLastSpread += timeStep;                
                while (mTimeLastSpread >= Main.settings.CropDiseaseSpreadInterval) 
                {
                    if (!spreadDisease())
                    {
                        endEarly = true;
                        break;
                    }
                    
                    mTimeLastSpread -= Main.settings.CropDiseaseSpreadInterval;
                }
                if (mTime > Main.settings.CropDiseaseDuration * 2 || endEarly)
                {
                    onEnd();
                }
            }
            else
            {
                updateDetection(mTimeToNext, timeStep);
                mTimeToNext -= timeStep;
                if (mTimeToNext < 0f)
                {
                    trigger();
                    decideNextTime();
                }
            }
            mFirstUpdate = false;
        }

        private List<ConstructionComponent> getAllPlantPads()
        {
            var plants = new List<ConstructionComponent>(BuildableUtils.GetAllComponents());
            for (int i = plants.Count - 1; i >= 0; i--)
            {
                var componentType = plants[i].getComponentType();
                if (!(componentType is VegetablePad || componentType is VegetablePadStarchy || componentType is MedicinalPad))
                {
                    plants.RemoveAt(i);
                }
            }
            return plants;
        }

        private bool spreadDisease()
        {
            log("mTime: " + mTime + ", mTimeLastSpread: " + mTimeLastSpread + ", mTimeToNext: " + mTimeToNext);
            var plants = getAllPlantPads();

            if (plants.Count == 0)
            {
                log("No plants found to infect. Ending crop disease disaster.");
                return false;
            }

            // End disaster if all inffected plants are removed or recovered
            if(mAffected)
            {
                bool anyDeadPlant = false;
                foreach(var plant in plants)
                {
                    Indicator condition = CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", plant);
                    if (condition != null && condition.getValue() <= 0.5f)
                    {
                        anyDeadPlant = true;
                        break;
                    }
                }
                
                if (!anyDeadPlant)
                {
                    log("All infected plants have been removed or recovered. Ending crop disease disaster.");
                    return false;
                }
            }

            // Remove vfx for plants that are no longer affected
            foreach(var plant in mDictVFX.Keys.ToArray())
            {
                Indicator condition = CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", plant);
                if (condition != null && condition.getValue() > 0.5f)
                {
                    log($"Removing disease VFX from plant {plant.getName()}.");
                    GameObject vfx = mDictVFX[plant];
                    if (vfx != null)
                    {
                        GameObject.Destroy(vfx);
                    }
                    mDictVFX.Remove(plant);
                }
            }

            // Only healthy plants can be affected by the disease
            plants = plants.Where(p =>
            {
                Indicator condition = CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", p);
                return condition != null && condition.getValue() > 0.75f;
            }).ToList();

            if(plants.Count == 0)
            {
                return true;
            }

            ShuffleArray(plants.ToArray());

            int affectedCount = Mathf.CeilToInt(plants.Count * Main.settings.CropDiseaseSpreadPercentage / 100f);
            affectedCount = Mathf.Clamp(affectedCount, 1, plants.Count);

            // Random ranged between 1 and affectedCount to make the disease spread more unpredictable
            affectedCount = Random.Range(1, affectedCount + 1);

            log("Spreading crop disease to " + affectedCount + " plants out of " + plants.Count + " total plants.");
            for (int i = 0; i < affectedCount; i++)
            {
                int index = Random.Range(0, plants.Count);
                ConstructionComponent plant = plants[index];
                plants.RemoveAt(index);
                Indicator condition = CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", plant);
                if (condition != null)
                {
                    condition.decrease(condition.getValue());
                    if(!mDictVFX.ContainsKey(plant))
                    {
                        log($"Attaching disease VFX to plant {plant.getName()}.");
                        mDictVFX[plant] = VfxHelper.AttachDiseaseVfx(plant.getGameObject());
                    }
                }
            }

            mAffected = true;
            return true;
        }

        void ShuffleArray<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);

                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        private void updateDetection(float timeLeft, float timeStep)
        {
        }

        public override bool isInProgress()
        {
            return mInProgress;
        }

        private void onStart()
        {
            Singleton<TimeManager>.getInstance().setNormalSpeed();
            Singleton<MessageLog>.getInstance().addMessage(new Message("Crop disease spreading", TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeBioDome>().getIcon(), 1));
            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        private void onEnd()
        {
            log("Crop disease disaster ended.");

            mInProgress = false;
            mAffected = false;

            // Remove vfx
            foreach(var vfx in mDictVFX.Values)
            {
                if (vfx != null)
                {
                    GameObject.Destroy(vfx);
                }
            }   
            mDictVFX.Clear();

            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        public override void trigger()
        {
            mTime = 0f;
            mTimeLastSpread = 0f;
            mInProgress = true;
            mAffected = false;
            onStart();
        }

        public void serialize(XmlNode rootNode, string name)
        {
            XmlNode parent = Serialization.createNode(rootNode, name);
            Serialization.serializeBool(parent, "crop-disease-in-progress", mInProgress);
            Serialization.serializeFloat(parent, "time-to-next-crop-disease", mTimeToNext);
            Serialization.serializeBool(parent, "crop-disease-affected", mAffected);
            Serialization.serializeFloat(parent, "time", mTime);
        }

        public void deserialize(XmlNode node)
        {
            if (node != null)
            {
                mInProgress = Serialization.deserializeBool(node["crop-disease-in-progress"]);
                mTimeToNext = Serialization.deserializeFloat(node["time-to-next-crop-disease"]);
                mAffected = Serialization.deserializeBool(node["crop-disease-affected"]);
                mTime = Serialization.deserializeFloat(node["time"]);
                mTimeLastSpread = mInProgress ? mTime % 1f : 0f;
            }
        }

        public override float getIntensity()
        {
            return 0;
        }

        private void log(string message)
        {
            if (ModBase.ModEntry != null && mLoggingEnabled)
            {
                ModBase.ModEntry.Logger.Log(message);
            }
        }
    }
}
