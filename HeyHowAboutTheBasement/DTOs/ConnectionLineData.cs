using UnityEngine;
using Module = Planetbase.Module;


namespace HeyHowAboutTheBasement.DTOs
{
    public class ConnectionLineData
    {
        public Vector3 Position;
        public Vector3 Normalized;
        public float Radius;
        public float Magnitude;

        public ConnectionLineData(Module module1, Module module2)
        {
            float size = Mathf.Min(Mathf.Min(module1.getRadius(), module2.getRadius()), Module.ValidSizes[2] * 0.5f);
            var radius = size * 0.5f;
            Vector3 vector2 = calculateConnectionPoint(module1, module2.getPosition(), radius);
            Vector3 vector3 = calculateConnectionPoint(module2, module1.getPosition(), radius);
            Vector3 position = module1.getPosition();
            Vector3 position2 = module2.getPosition();
            Vector3 vector = position2 - position;
            position2 = vector3 - vector;

            Position = vector2 + vector;
            Radius = radius * 1.6f;
            Magnitude = (position2 - position).magnitude;
            Normalized = (position2 - position).normalized;
        }

        public static Vector3 calculateConnectionPoint(Module module, Vector3 connectionPosition, float connectionRadius)
        {
            float radius = module.getRadius();
            float num = Mathf.Sqrt(radius * radius - connectionRadius * connectionRadius);
            Vector3 vector = connectionPosition - module.getPosition();
            vector.y = 0f;
            vector.Normalize();
            return module.getPosition() + vector * num;
        }
    }
}
