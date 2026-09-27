using Planetbase;
using System.Xml;

namespace HarshWorld
{ 
    public class CustomDisasterManager : Singleton<CustomDisasterManager>
    {
        private Disaster[] mDisasters = new Disaster[2];

        private MeteorRain mMeteorRain;
        private CropDisease mCropDisease;


        public CustomDisasterManager()
        {
            mMeteorRain = new MeteorRain();
            mCropDisease = new CropDisease();
            mDisasters[0] = mMeteorRain;
            mDisasters[1] = mCropDisease;
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

        public MeteorRain getMeteorRain()
        {
            return mMeteorRain;
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
            mMeteorRain.deserialize(rootNode["meteor-rain"]);
        }

        public void serialize(XmlNode rootNode)
        {
            mCropDisease.serialize(rootNode, "crop-disease");
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
