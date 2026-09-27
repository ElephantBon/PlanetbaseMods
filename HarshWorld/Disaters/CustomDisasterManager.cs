using Planetbase;
using System.Xml;

namespace HarshWorld
{ 
    public class CustomDisasterManager : Singleton<CustomDisasterManager>
    {
        private Disaster[] mDisasters = new Disaster[3];

        private CropDisease mCropDisease;
        private FireHazard mFireHazard;
        private MeteorRain mMeteorRain;


        public CustomDisasterManager()
        {
            mMeteorRain = new MeteorRain();
            mCropDisease = new CropDisease();
            mFireHazard = new FireHazard();
            mDisasters[0] = mCropDisease;
            mDisasters[1] = mFireHazard;
            mDisasters[2] = mMeteorRain;
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

        public CropDisease getCropDisease()
        {
            return mCropDisease;
        }

        public FireHazard getFireHazard()
        {
            return mFireHazard;
        }

        public MeteorRain getMeteorRain()
        {
            return mMeteorRain;
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
        }

        public void serialize(XmlNode rootNode)
        {
            mCropDisease.serialize(rootNode, "crop-disease");
            mFireHazard.serialize(rootNode, "fire-hazard");
            mMeteorRain.serialize(rootNode, "meteor-rain");
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
