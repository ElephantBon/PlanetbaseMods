using HarshWorld.Disasters.RogueBot;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace HarshWorld
{ 
    public class RogueBots : Disaster
    {
        private bool mFirstUpdate = true;

        private bool mInProgress;

        private float mTimeToNext;

        private float mTime;

        private float mTimeLastCheck;

        private float mCheckInterval = 10f;

        private bool mLoggingEnabled = true;

        private bool mStarted = false;

        public RogueBots()
        {
            decideNextTime();
        }

        public override void destroy()
        {
        }

        private void decideNextTime()
        {
            mTimeToNext = Random.Range(HarshWorld.settings.MinimumDurationBetweenRogueBots, HarshWorld.settings.MaximumDurationBetweenRogueBots) * HarshWorld.RealTimeFactor;
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
                mTimeLastCheck += timeStep;
                if (!mStarted || mTimeLastCheck >= mCheckInterval * HarshWorld.RealTimeFactor) 
                {
                    if (!updateBots())
                    {
                        endEarly = true;
                    }
                    
                    mTimeLastCheck -= mCheckInterval * HarshWorld.RealTimeFactor;
                }
                if (mTime > HarshWorld.settings.RogueBotsDuration * HarshWorld.RealTimeFactor || endEarly)
                {
                    restoreBots();
                    onEnd();
                }
            }
            else
            {
                updateDetection(mTimeToNext, timeStep);
                mTimeToNext -= timeStep;
                if (mTimeToNext < 0f)
                {
                    if(HarshWorld.settings.EnableRogueBots && Random.Range(0f, 100f) < HarshWorld.settings.DisasterProbability)
                    {
                        trigger();
                    }
                    decideNextTime();
                }
            }
            mFirstUpdate = false;
        }

        private bool updateBots()
        {
            if(mStarted)
            {
                // End disaster if all rogue bots are destroyed
                var bots = CoreUtils.GetMember<Character, List<Character>>("mCharacters").Where(c => c.getSpecialization() is RogueBot).ToArray();
                if(bots.Length == 0)
                {
                    log("All rogue bots destroyed. Ending rogue bots disaster.");
                    return false;
                }
            }
            else
            {
                // Turn bots to rogue bots
                var bots = CoreUtils.GetMember<Character, List<Character>>("mCharacters").Where(c => c is Bot).ToArray();
                if(bots.Length == 0)
                {
                    log("No bots found to turn rogue. Ending rogue bots disaster.");
                    return false;
                }

                ArrayHelper.Shuffle(bots);

                int rogueCount = Mathf.CeilToInt(bots.Length * HarshWorld.settings.RogueBotsPercentage / 100f);
                for(int i = 0;  i < rogueCount; i++) 
                {
                    RogueBot.goRogue(bots[i] as Bot);
                }

                mStarted = true;
            }

            return true;
        }

        private void restoreBots()
        {
            var bots = CoreUtils.GetMember<Character, List<Character>>("mCharacters").Where(c => c.getSpecialization() is RogueBot).ToArray();
            foreach(var bot in bots)
            {
                RogueBot.restore(bot);
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
            Singleton<MessageLog>.getInstance().addMessage(new Message("Bots go rogue", ResourceList.StaticIcons.Bot, 1));
            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        private void onEnd()
        {
            log("Rogue bots disaster ended.");

            mInProgress = false;
            mStarted = false;

            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        public override void trigger()
        {
            if (mInProgress)
            {
                return;
            }

            mTime = 0f;
            mTimeLastCheck = 0f;
            mInProgress = true;
            mStarted = false;
            onStart();
        }

        public void serialize(XmlNode rootNode, string name)
        {
            XmlNode parent = Serialization.createNode(rootNode, name);
            Serialization.serializeBool(parent, "rogue-bots-in-progress", mInProgress);
            Serialization.serializeFloat(parent, "time-to-next-rogue-bots", mTimeToNext);
            Serialization.serializeBool(parent, "rogue-bots-started", mStarted);
            Serialization.serializeFloat(parent, "time", mTime);
        }

        public void deserialize(XmlNode node)
        {
            if (node != null)
            {
                mInProgress = Serialization.deserializeBool(node["rogue-bots-in-progress"]);
                mTimeToNext = Serialization.deserializeFloat(node["time-to-next-rogue-bots"]);
                mStarted = Serialization.deserializeBool(node["rogue-bots-started"]);
                mTime = Serialization.deserializeFloat(node["time"]);
                mTimeLastCheck = mInProgress ? mTime % 1f : 0f;
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
