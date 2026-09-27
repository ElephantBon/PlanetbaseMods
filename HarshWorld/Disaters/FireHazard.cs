using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace HarshWorld
{
    /// <summary>
    /// When fire hazard starts, pick one random interior module to set fire.
    /// In every update cycle, try to spread fire to nearby modules and connections.
    /// Fire gets stronger when consuming oxygen in the module, and weaker when oxygen is low, and eventually extinguishes when oxygen is too low.
    /// Fire damages modules, connections, components, bots, and colonists.
    /// </summary>
    public class FireHazard : Disaster
    {
        private bool mFirstUpdate = true;

        private bool mInProgress;

        private float mTimeToNext;

        private float mTime;

        private float mTimeLastSpread;

        private bool mLoggingEnabled = false;

        private bool mFireStarted = false;

        private const int MaxVFXcountPerConstruction = 5;

        private const float FireSpreadInterval = 1f * HarshWorld.RealTimeFactor;

        private const float CharacterDamageFactor = 10f;

        private const float OxygenConsumptionFactor = 2f;

        private const float OxygenThresholdToExtinguish = 0.15f / 0.23f;

        private const float OxygenThresholdToSmoldering = 0.10f / 0.23f;

        private const float ProbabilityDecreaseFireIntensity = 0.2f;

        private const float MinimumConditionOfConstruction = 0.15f; // Condition too low will break the dome and leak oxygen

        private Dictionary<Construction, List<GameObject>> mDictVFX = new Dictionary<Construction, List<GameObject>>();

        public FireHazard()
        {
            decideNextTime();
        }

        public override void destroy()
        {
        }

        private void decideNextTime()
        {
            mTimeToNext = Random.Range(HarshWorld.settings.MinimumDurationBetweenFireHazards, HarshWorld.settings.MaximumDurationBetweenFireHazards) * HarshWorld.RealTimeFactor;
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
                while (mTimeLastSpread >= FireSpreadInterval) 
                {
                    if (!trySpreadFire())
                    {
                        endEarly = true;
                        break;
                    }
                    
                    mTimeLastSpread -= FireSpreadInterval;
                }
                if (mTime > HarshWorld.settings.FireHazardDuration * HarshWorld.RealTimeFactor || endEarly)
                {
                    onEnd();
                }
                else
                {
                    updateDamage(timeStep);
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

        private bool trySpreadFire()
        {
            // Get all interior modules including connections
            var modules = new List<Module>(BuildableUtils.GetAllModules().Where(m => m.getLocation() == Location.Interior));
            if(modules.Count == 0)
            {
                log("No interior modules found to spread fire.");
                return false;
            }

            ArrayHelper.Shuffle(modules);

            if (mFireStarted)
            {
                // End fire hazard if there are no more modules with fire
                if (mDictVFX.Count == 0)
                {
                    log("No more modules with fire, ending fire hazard.");
                    return false;
                }

                foreach (var construction in mDictVFX.Keys.ToArray())
                {
                    // Spread fire to nearby modules
                    if(!construction.isLocked())
                    { 
                        foreach (var link in construction.getLinks())
                        {
                            if(link.getLocation() == Location.Exterior
                            || link.isLocked()
                            || (Random.Range(0f, 100f) > HarshWorld.settings.FireSpreadProbability))
                            {
                                continue;
                            }

                            if (link is Module && !mDictVFX.ContainsKey((Module)link))
                            {   
                                increaseFireIntensity((Module)link);
                            }
                            else
                            if(link is Connection && !mDictVFX.ContainsKey((Connection)link))
                            {
                                increaseFireIntensity((Connection)link);
                            }
                        }
                    }

                    // Update fire intensity based on oxygen level
                    Indicator oxygen = CoreUtils.GetMember<Construction, Indicator>("mOxygenIndicator", construction);
                    if(oxygen != null)
                    {
                        log("Oxygen level in " + construction.getName() + ": " + oxygen.getValue());
                        if (oxygen.getValue() < OxygenThresholdToSmoldering)
                        {
                            // Smoldering fire, decrease intensity
                            if(Random.Range(0f, 1f) < ProbabilityDecreaseFireIntensity)
                            {
                                decreaseFireIntensity(construction);
                            }
                        }
                        else
                        if (oxygen.getValue() > OxygenThresholdToExtinguish)
                        {
                            // Normal fire, increase intensity
                            increaseFireIntensity(construction);
                        }
                    }

                    // Recycle components randomly in the construction
                    foreach (var component in construction.getComponents().ToArray())
                    {
                        if (Random.Range(0f, 1f) < HarshWorld.settings.FireBurnDownProbability * 0.01f)
                        {
                            component.recycle();
                            component.destroy();
                        }
                    }
                }
            }
            else
            {
                // Select first one component to set fire
                increaseFireIntensity(modules[0]);
            }

            mFireStarted = true;
            return true;
        }

        private void increaseFireIntensity(Construction construction)
        {
            // Increase fire intensity by adding more VFX
            if (!mDictVFX.ContainsKey(construction))
            {
                mDictVFX[construction] = new List<GameObject>();
            }

            var vfxList = mDictVFX[construction];
            if (vfxList.Count < MaxVFXcountPerConstruction)
            {
                var vfx = VfxHelper.AttachFireVfx(construction.getGameObject());
                var r = construction.getRadius() * 0.9f;

                // Set random position for the VFX
                if (construction is Connection)
                {
                    // For connections, set the VFX position to a random point along the connection's length
                    var connection = construction as Connection;
                    var m1 = connection.getModule1();
                    var m2 = connection.getModule2();
                    var p1 = m1.getPosition();
                    var p2 = m2.getPosition();
                    var dir = (p2 - p1).normalized;
                    p1 = p1 + dir * (m1.getRadius() + r);
                    p2 = p2 - dir * (m2.getRadius() + r);
                    var basePoint = Vector3.Lerp(p1, p2, Random.value);
                    var randomCircle = Random.insideUnitCircle * r;
                    var offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
                    vfx.transform.position = basePoint + offset;
                }
                else
                {
                    // For modules, set the VFX position to a random point within the module's radius
                    vfx.transform.localPosition = new Vector3(Random.Range(-r, r), 0, Random.Range(-r, r));
                }

                vfxList.Add(vfx);

                if(construction is Connection) { }
            }
        }

        private void decreaseFireIntensity(Construction construction)
        {
            // Decrease fire intensity by removing VFX
            if (mDictVFX.ContainsKey(construction))
            {
                var vfxList = mDictVFX[construction];
                if (vfxList.Count > 0)
                {
                    var vfx = vfxList[vfxList.Count - 1];
                    GameObject.Destroy(vfx);
                    vfxList.RemoveAt(vfxList.Count - 1);

                    if(vfxList.Count == 0)
                    {
                        mDictVFX.Remove(construction);
                    }
                }
            }
        }

        private void updateDamage(float timeStep)
        {
            foreach(var kvp in mDictVFX)
            {
                var construction = kvp.Key;
                var vfxList = kvp.Value;
                var damage = vfxList.Count * HarshWorld.settings.FireDamageStrength * timeStep * 0.01f;
                var mCharacters = CoreUtils.GetMember<Character, List<Character>>("mCharacters").ToArray();
                var mResources = CoreUtils.GetMember<Resource, List<Resource>>("mResources").ToArray();

                // Damage construction but never destory it to make it walkable
                {
                    Indicator condition = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", construction);
                    if (condition != null)
                    {
                        if (condition.getValue() - damage > MinimumConditionOfConstruction)
                        {
                            condition.decrease(damage);
                        }
                    }
                }

                // Damage resources in the construction
                var r = construction.getRadius();
                foreach (var resource in mResources)
                {
                    if(!resource.isEmbedded() 
                    && resource.getResourceContainer() == null
                    && resource.getContainer() == null 
                    && resource.getLocation() == Location.Interior 
                    && (resource.getPosition() - construction.getPosition()).magnitude <= r)
                    {
                        Indicator condition = CoreUtils.GetMember<Resource, Indicator>("mConditionIndicator", resource);
                        if (condition != null)
                        {
                            condition.decrease(damage);
                        }
                    }
                }

                // Damage colonists and bots in the construction
                foreach (var character in mCharacters)
                {
                    if (character.getCurrentConstruction() == construction)
                    {
                        character.decayIndicator(CharacterIndicator.Health, damage * CharacterDamageFactor);
                        character.decayIndicator(CharacterIndicator.Condition, damage * CharacterDamageFactor);
                    }
                }

                // Consume oxygen
                {
                    Indicator oxygen = CoreUtils.GetMember<Construction, Indicator>("mOxygenIndicator", construction);
                    if (oxygen != null)
                    {
                        oxygen.decrease(damage * OxygenConsumptionFactor);
                    }
                }
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
            Singleton<MessageLog>.getInstance().addMessage(new Message("Fire hazard spreading", AssetManager.IconFire, 1));
            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        private void onEnd()
        {
            log("Fire hazard ended");

            mInProgress = false;
            mFireStarted = false;

            // Remove vfx
            foreach(var vfxList in mDictVFX.Values)
            {
                foreach(var vfx in vfxList)
                {
                    if (vfx != null)
                    {
                        GameObject.Destroy(vfx);
                    }
                }
            }   
            mDictVFX.Clear();

            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        public override void trigger()
        {
            if(mInProgress)
            {
                return;
            }

            mTime = 0f;
            mTimeLastSpread = 0f;
            mInProgress = true;
            mFireStarted = false;
            onStart();
        }

        public void serialize(XmlNode rootNode, string name)
        {
            XmlNode parent = Serialization.createNode(rootNode, name);
            Serialization.serializeBool(parent, "fire-hazard-in-progress", mInProgress);
            Serialization.serializeFloat(parent, "time-to-next-fire-hazard", mTimeToNext);
            Serialization.serializeBool(parent, "fire-hazard-started", mFireStarted);
            Serialization.serializeFloat(parent, "time", mTime);
        }

        public void deserialize(XmlNode node)
        {
            if(mDictVFX != null)
            {
                mDictVFX.Clear();
            }

            if (node != null)
            {
                mInProgress = Serialization.deserializeBool(node["fire-hazard-in-progress"]);
                mTimeToNext = Serialization.deserializeFloat(node["time-to-next-fire-hazard"]);
                mFireStarted = Serialization.deserializeBool(node["fire-hazard-started"]);
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
