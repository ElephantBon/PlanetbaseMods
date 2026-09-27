using Planetbase;
using PlanetbaseModUtilities;
using DateTime = System.DateTime;
using System.Xml;
using UnityEngine;

namespace HarshWorld
{ 
    public class MeteorRain : Disaster
    {
        private bool mFirstUpdate = true;

        private bool mInProgress;

        private float mTimeToNext;

        private float mTime;

        private float mMeteorSpawnTimer;

        private long mLastLoggedSystemSecond = -1L;

        private const float MeteorSpawnInterval = 0.05f;

        private bool mLoggingEnabled = false;

        public MeteorRain()
        {
            decideNextTime();
        }

        public override void destroy()
        {
        }

        private void decideNextTime()
        {
            mTimeToNext = Random.Range(HarshWorld.settings.MinimumDurationBetweenMeteorRains, HarshWorld.settings.MaximumDurationBetweenMeteorRains) * HarshWorld.RealTimeFactor;
            if (PlanetManager.getCurrentPlanet().getMeteorRisk() == Planet.Quantity.Low)
            {
                mTimeToNext *= 2f;
            }
            GameplayModifier gameplayModifier = Singleton<ChallengeManager>.getInstance().getGameplayModifier(GameplayModifierType.DisasterFrequency);
            if (gameplayModifier != null)
            {
                mTimeToNext *= 1f / Mathf.Clamp(gameplayModifier.getFloat(), 0.1f, 100f);
            }
        }

        public override void onTimeScaleChanged(float timeScale, bool paused)
        {
        }

        public override void update(float timeStep)
        {
            if (ModBase.ModEntry != null && mLoggingEnabled)
            {
                DateTime now = DateTime.Now;
                long currentSystemSecond = now.Ticks / 10000000L;
                if (currentSystemSecond != mLastLoggedSystemSecond)
                {
                    ModBase.ModEntry.Logger.Log($"[{now:HH:mm:ss}] MeteorRain.update timeStep={timeStep:F4}, mTime={mTime:F4}, mTimeToNext={mTimeToNext:F4}");
                    mLastLoggedSystemSecond = currentSystemSecond;
                }
            }

            if (PlanetManager.getCurrentPlanet().getMeteorRisk() != 0)
            {
                if (mInProgress)
                {
                    Singleton<MusicManager>.getInstance().onTension();
                    if (mFirstUpdate)
                    {
                        onStart();
                    }
                    mTime += timeStep;
                    mMeteorSpawnTimer += timeStep;
                    while (mMeteorSpawnTimer >= MeteorSpawnInterval)
                    {
                        Singleton<MeteorManager>.getInstance().spawnMeteor();
                        mMeteorSpawnTimer -= MeteorSpawnInterval;
                    }
                    if (mTime > HarshWorld.settings.MeteorRainDuration * HarshWorld.RealTimeFactor)
                    {
                        mInProgress = false;
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
            }
            mFirstUpdate = false;
        }

        private void updateDetection(float timeLeft, float timeStep)
        {
            if (timeLeft > 300f && timeLeft - timeStep <= 300f && Random.value <= Singleton<Colony>.getInstance().getDisasterInterceptionChance())
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message("Meteor rain detected", TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeTelescope>().getIcon(), 4));
            }
            if (timeLeft > 120f && timeLeft - timeStep <= 120f && Random.value <= Singleton<Colony>.getInstance().getDisasterInterceptionChance())
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message("Meteor rain incoming", TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeTelescope>().getIcon(), 4));
            }
        }

        public override bool isInProgress()
        {
            return mInProgress;
        }

        private void onStart()
        {
            Singleton<TimeManager>.getInstance().setNormalSpeed();
            Singleton<MessageLog>.getInstance().addMessage(new Message("Meteor rain now", ResourceList.StaticIcons.Meteor, 1));
            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        private void onEnd()
        {
            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        public override void trigger()
        {
            if (mInProgress)
            {
                return;
            }

            mTime = 0f;
            mMeteorSpawnTimer = 0f;
            mInProgress = true;
            onStart();
        }

        public void serialize(XmlNode rootNode, string name)
        {
            XmlNode parent = Serialization.createNode(rootNode, name);
            Serialization.serializeBool(parent, "meteor-rain-in-progress", mInProgress);
            Serialization.serializeFloat(parent, "time-to-next-meteor-rain", mTimeToNext);
            Serialization.serializeFloat(parent, "time", mTime);
        }

        public void deserialize(XmlNode node)
        {
            if (PlanetManager.getCurrentPlanet().getMeteorRisk() != 0 && node != null)
            {
                mInProgress = Serialization.deserializeBool(node["meteor-rain-in-progress"]);
                mTimeToNext = Serialization.deserializeFloat(node["time-to-next-meteor-rain"]);
                mTime = Serialization.deserializeFloat(node["time"]);
                mMeteorSpawnTimer = mInProgress ? mTime % MeteorSpawnInterval : 0f;
            }
        }

        public override float getIntensity()
        {
            return 0;
        }
    }
}
