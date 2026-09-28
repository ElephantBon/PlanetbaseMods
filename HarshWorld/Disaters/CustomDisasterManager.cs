using Planetbase;
using System.Xml;

namespace HarshWorld
{ 
    public class CustomDisasterManager : Singleton<CustomDisasterManager>
    {
        private Disaster[] mDisasters = new Disaster[4];

        private CropDisease mCropDisease;
        private FireHazard mFireHazard;
        private MeteorRain mMeteorRain;
        private RogueBots mRogueBots;


        public CustomDisasterManager()
        {
            mCropDisease = new CropDisease();
            mFireHazard = new FireHazard();
            mMeteorRain = new MeteorRain();
            mRogueBots = new RogueBots();
            mDisasters[0] = mCropDisease;
            mDisasters[1] = mFireHazard;
            mDisasters[2] = mMeteorRain;
            mDisasters[3] = mRogueBots;
        }

        public void update(float timeStep)
        {
            for (int i = 0; i < mDisasters.Length; i++)
            {
                mDisasters[i].update(timeStep);
            }
        }

        public override void destroy()
        {
            base.destroy();
            for (int i = 0; i < mDisasters.Length; i++)
            {
                mDisasters[i].destroy();
            }
        }

        public Disaster[] getDisasters()
        {
            return (Disaster[])mDisasters.Clone();
        }

        public bool anyInProgress()
        {
            for (int i = 0; i < mDisasters.Length; i++)
            {
                if (mDisasters[i].isInProgress())
                    return true;
            }
            return false;
        }

        public Disaster getMeteorRainInProgress()
        {
            if (mMeteorRain.isInProgress())
            {
                return mMeteorRain;
            }
            return null;
        }

        public void deserialize(XmlNode rootNode)
        {
            mCropDisease.deserialize(rootNode["crop-disease"]);
            mFireHazard.deserialize(rootNode["fire-hazard"]);
            mMeteorRain.deserialize(rootNode["meteor-rain"]);
            mRogueBots.deserialize(rootNode["rogue-bots"]);
        }

        public void serialize(XmlNode rootNode)
        {
            mCropDisease.serialize(rootNode, "crop-disease");
            mFireHazard.serialize(rootNode, "fire-hazard");
            mMeteorRain.serialize(rootNode, "meteor-rain");
            mRogueBots.serialize(rootNode, "rogue-bots");
        }

        public void onTimeScaleChanged(float timeScale, bool paused)
        {
            for (int i = 0; i < mDisasters.Length; i++)
            {
                mDisasters[i].onTimeScaleChanged(timeScale, paused);
            }
        }
    }
}
