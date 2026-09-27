using Planetbase;
using System.Xml;

namespace HarshWorld
{ 
    public class CustomDisasterManager : Singleton<CustomDisasterManager>
    {
        private Disaster[] mDisasters = new Disaster[1];

        private MeteorRain mMeteorRain;

        public CustomDisasterManager()
        {
            mMeteorRain = new MeteorRain();
            mDisasters[0] = mMeteorRain;
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
            mMeteorRain.deserialize(rootNode["meteor-rain"]);
        }

        public void serialize(XmlNode rootNode)
        {
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
