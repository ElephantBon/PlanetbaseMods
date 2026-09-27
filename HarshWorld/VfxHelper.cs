using UnityEngine;

namespace HarshWorld
{
    using UnityEngine;

    public static class VfxHelper
    {
        // Cache shared materials so repeated calls don't leak new Material instances.
        private static Material _flameMaterial;
        private static Material _smokeMaterial;

        /// <summary>
        /// Attaches a layered, realistic fire effect (flame core + smoke + flickering light)
        /// to the specified GameObject. Returns the container object so it can be Destroy()'d
        /// later to extinguish the fire.
        /// </summary>
        public static GameObject AttachFireVfx(GameObject targetObject)
        {
            if (targetObject == null)
            {
                Debug.LogWarning("VfxHelper.AttachFireVfx: targetObject is null.");
                return null;
            }

            // Container that holds flame, smoke and light together.
            GameObject container = new GameObject("Mod_FireVFX");
            container.transform.SetParent(targetObject.transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            GameObject flameObj = BuildFlameLayer(container.transform);
            GameObject smokeObj = BuildSmokeLayer(container.transform);
            Light fireLight = BuildFlickerLight(container.transform);

            // Drive the light flicker from a small MonoBehaviour component.
            FireLightFlicker flicker = container.AddComponent<FireLightFlicker>();
            flicker.Light = fireLight;

            flameObj.GetComponent<ParticleSystem>().Play();
            smokeObj.GetComponent<ParticleSystem>().Play();

            return container;
        }

        /// <summary>
        /// Attaches a slow-spreading sickly green smoke/spore effect to the specified
        /// GameObject to signal crop disease. Unlike fire, this drifts outward in all
        /// directions (sphere shape) rather than upward, and moves slowly to read as
        /// "spreading contamination" rather than active combustion.
        /// Returns the container so it can be Destroy()'d once the plant is cured/removed.
        /// </summary>
        public static GameObject AttachDiseaseVfx(GameObject targetObject)
        {
            if (targetObject == null)
            {
                Debug.LogWarning("VfxHelper.AttachDiseaseVfx: targetObject is null.");
                return null;
            }

            GameObject container = new GameObject("Mod_DiseaseVFX");
            container.transform.SetParent(targetObject.transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            VfxTag tag = container.AddComponent<VfxTag>();
            tag.VfxType = "Disease";

            GameObject sporeObj = BuildDiseaseSporeLayer(container.transform);
            sporeObj.GetComponent<ParticleSystem>().Play();

            return container;
        }

        // ---------------------------------------------------------------
        // Disease spores: slow, sickly green, spreads outward in a sphere
        // (all directions), low speed, long-lived, gentle upward drift.
        // ---------------------------------------------------------------
        private static GameObject BuildDiseaseSporeLayer(Transform parent)
        {
            GameObject sporeObj = new GameObject("DiseaseSpores");
            sporeObj.transform.SetParent(parent);
            sporeObj.transform.localPosition = Vector3.zero;
            sporeObj.transform.localRotation = Quaternion.identity;
            // Flatten the emitter itself so the sphere shape below becomes a squashed
            // ellipsoid - this biases particles to spread outward horizontally instead
            // of in a perfect sphere (which reads as "going up" once gravity is added).
            sporeObj.transform.localScale = new Vector3(1.6f, 0.35f, 1.6f);

            ParticleSystem ps = sporeObj.AddComponent<ParticleSystem>();

            // Main module - very slow and long-lived so it reads as a lingering, creeping haze
            var main = ps.main;
            main.duration = 1f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 9f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.15f); // slow crawl outward
            main.startSize = new ParticleSystem.MinMaxCurve(0.6f, 1.2f);
            main.gravityModifier = 0f; // no vertical bias at all - pure outward spread
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.45f, 0.65f, 0.15f), new Color(0.25f, 0.4f, 0.1f));
            main.maxParticles = 120;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Emission - low rate, this is a slow spread, not a burst
            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 5f;

            // Shape - sphere (squashed flat by the transform scale above) so particles
            // spread outward in a low, horizontal disc around the plant.
            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.4f; // roughly matches a small plant's footprint; scale per-plant if needed

            // Color over lifetime - fades in as a hazy sickly green, then dissipates
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient diseaseGradient = new Gradient();
            diseaseGradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(new Color(0.55f, 0.75f, 0.2f), 0f),
                new GradientColorKey(new Color(0.35f, 0.5f, 0.15f), 0.5f),
                new GradientColorKey(new Color(0.2f, 0.3f, 0.1f), 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.35f, 0.25f),
                new GradientAlphaKey(0.25f, 0.7f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = diseaseGradient;

            // Size over lifetime - spores/smoke puffs slowly expand as they spread
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.4f);
            sizeCurve.AddKey(0.6f, 1f);
            sizeCurve.AddKey(1f, 1.6f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Gentle tumbling for organic haze motion
            var rotationOverLifetime = ps.rotationOverLifetime;
            rotationOverLifetime.enabled = true;
            rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-15f, 15f);

            // Soft turbulence so the cloud doesn't look like it's moving in perfect straight lines
            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.25f;
            noise.frequency = 0.2f;
            noise.scrollSpeed = 0.2f;
            noise.damping = true;
            noise.quality = ParticleSystemNoiseQuality.Medium;

            // Renderer - alpha blended, not additive (this should look like murky haze, not glow)
            var renderer = sporeObj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetSmokeMaterial(); // reuses the alpha-blended smoke material
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }

            return sporeObj;
        }

        // ---------------------------------------------------------------
        // Flame core: fast, bright, additive, short-lived, upward + noisy
        // ---------------------------------------------------------------
        private static GameObject BuildFlameLayer(Transform parent)
        {
            GameObject flameObj = new GameObject("Flame");
            flameObj.transform.SetParent(parent);
            flameObj.transform.localPosition = Vector3.zero;
            flameObj.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // point upward

            ParticleSystem ps = flameObj.AddComponent<ParticleSystem>();

            // Main module
            var main = ps.main;
            main.duration = 1f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.7f, 1.3f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.6f, 1.1f);
            main.gravityModifier = -0.15f; // gentle upward pull
                                           // Narrow, hot start color range - the gradient below does most of the work.
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.95f, 0.8f), new Color(1f, 0.8f, 0.4f));
            main.maxParticles = 200;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Emission
            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 45f;

            // Shape - cone, slightly wider base for a natural flame footprint
            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 12f;
            shape.radius = 0.35f;

            // Color over lifetime: white-hot core -> yellow -> orange -> deep red -> fade
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient flameGradient = new Gradient();
            flameGradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(new Color(1f, 1f, 0.9f), 0f),
                new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0.15f),
                new GradientColorKey(new Color(1f, 0.4f, 0f), 0.4f),
                new GradientColorKey(new Color(0.4f, 0.08f, 0.05f), 0.75f),
                new GradientColorKey(Color.black, 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.85f, 0.5f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = flameGradient;

            // Size over lifetime: quick puff then taper as it rises
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.6f);
            sizeCurve.AddKey(0.2f, 1f);
            sizeCurve.AddKey(1f, 0.15f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Rotation over lifetime for organic tumbling
            var rotationOverLifetime = ps.rotationOverLifetime;
            rotationOverLifetime.enabled = true;
            rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-90f, 90f);

            // Velocity over lifetime for subtle horizontal "dance"
            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);

            // Noise for flicker/turbulence - the single biggest realism lever
            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.5f;
            noise.frequency = 0.8f;
            noise.scrollSpeed = 1.2f;
            noise.damping = true;
            noise.quality = ParticleSystemNoiseQuality.Medium;

            // Renderer
            var renderer = flameObj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetFlameMaterial();
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }

            return flameObj;
        }

        // ---------------------------------------------------------------
        // Smoke layer: slower, larger, low opacity, alpha-blended, longer-lived
        // ---------------------------------------------------------------
        private static GameObject BuildSmokeLayer(Transform parent)
        {
            GameObject smokeObj = new GameObject("Smoke");
            smokeObj.transform.SetParent(parent);
            smokeObj.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            smokeObj.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            ParticleSystem ps = smokeObj.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 1f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 3f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.4f);
            main.gravityModifier = -0.05f;
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.15f, 0.15f, 0.15f, 0.5f), new Color(0.3f, 0.3f, 0.3f, 0.5f));
            main.maxParticles = 80;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 8f;

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 18f;
            shape.radius = 0.25f;

            // Smoke fades in slightly then dissipates
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient smokeGradient = new Gradient();
            smokeGradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 0f),
                new GradientColorKey(new Color(0.35f, 0.35f, 0.35f), 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.35f, 0.2f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = smokeGradient;

            // Smoke expands as it rises (opposite of flame's shrink)
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve smokeSizeCurve = new AnimationCurve();
            smokeSizeCurve.AddKey(0f, 0.5f);
            smokeSizeCurve.AddKey(1f, 2f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, smokeSizeCurve);

            var rotationOverLifetime = ps.rotationOverLifetime;
            rotationOverLifetime.enabled = true;
            rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-30f, 30f);

            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.4f;
            noise.frequency = 0.3f;
            noise.scrollSpeed = 0.5f;
            noise.damping = true;

            var renderer = smokeObj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetSmokeMaterial();
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }

            return smokeObj;
        }

        // ---------------------------------------------------------------
        // Flickering point light to sell the "fire casts light" effect
        // ---------------------------------------------------------------
        private static Light BuildFlickerLight(Transform parent)
        {
            GameObject lightObj = new GameObject("FireLight");
            lightObj.transform.SetParent(parent);
            lightObj.transform.localPosition = new Vector3(0f, 0.5f, 0f);

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.6f, 0.2f);
            light.range = 6f;
            light.intensity = 2f;
            light.shadows = LightShadows.None; // flickering shadows are expensive; enable if needed

            return light;
        }

        // ---------------------------------------------------------------
        // Shader / material helpers with fallback chain (guards against
        // "Particles/Additive" or "Particles/Standard Unlit" being missing
        // from the current render pipeline or being stripped from the build).
        // ---------------------------------------------------------------
        private static Material GetFlameMaterial()
        {
            if (_flameMaterial == null)
            {
                Shader shader = FindBestParticleShader(additive: true);
                _flameMaterial = shader != null ? new Material(shader) : null;
                if (_flameMaterial != null)
                {
                    _flameMaterial.mainTexture = GetSoftParticleTexture();
                }
            }
            return _flameMaterial;
        }

        private static Material GetSmokeMaterial()
        {
            if (_smokeMaterial == null)
            {
                Shader shader = FindBestParticleShader(additive: false);
                _smokeMaterial = shader != null ? new Material(shader) : null;
                if (_smokeMaterial != null)
                {
                    _smokeMaterial.mainTexture = GetSoftParticleTexture();
                }
            }
            return _smokeMaterial;
        }

        private static Texture2D _softParticleTexture;

        /// <summary>
        /// Generates a soft, round, radially-faded texture at runtime so particles read
        /// as soft puffs/glows instead of hard-edged squares (the default look when a
        /// shader has no texture assigned). Cached after first generation.
        /// </summary>
        private static Texture2D GetSoftParticleTexture()
        {
            if (_softParticleTexture != null)
            {
                return _softParticleTexture;
            }

            const int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxDist = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                    // Smooth falloff from opaque center to transparent edge.
                    float alpha = Mathf.Clamp01(1f - dist);
                    alpha = alpha * alpha * (3f - 2f * alpha); // smoothstep for a softer edge
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            tex.Apply();
            _softParticleTexture = tex;
            return _softParticleTexture;
        }

        private static Shader FindBestParticleShader(bool additive)
        {
            Shader shader = additive
                ? Shader.Find("Particles/Additive")
                : Shader.Find("Particles/Alpha Blended");

            if (shader == null)
            {
                // URP fallback
                shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            }
            if (shader == null)
            {
                // Legacy fallback
                shader = additive
                    ? Shader.Find("Legacy Shaders/Particles/Additive")
                    : Shader.Find("Legacy Shaders/Particles/Alpha Blended");
            }
            if (shader == null)
            {
                // Last resort - always present in Built-in RP
                shader = Shader.Find("Sprites/Default");
            }

            if (shader == null)
            {
                Debug.LogWarning("VfxHelper: No suitable particle shader found on this platform/pipeline.");
            }

            return shader;
        }

        public static bool HasVfx(GameObject target, string vfxType)
        {
            foreach (Transform child in target.transform)
            {
                VfxTag t = child.GetComponent<VfxTag>();
                if (t != null && t.VfxType == vfxType) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Small helper component that drives organic light-intensity flicker
    /// using layered Perlin noise rather than pure random jitter.
    /// </summary>
    public class FireLightFlicker : MonoBehaviour
    {
        public Light Light;

        [Tooltip("Base intensity the light flickers around.")]
        public float BaseIntensity = 2f;

        [Tooltip("How much the intensity varies above/below the base.")]
        public float FlickerAmount = 0.6f;

        [Tooltip("How fast the flicker noise evolves.")]
        public float FlickerSpeed = 3f;

        private float _noiseSeed;

        private void Awake()
        {
            _noiseSeed = Random.Range(0f, 1000f);
        }

        private void Update()
        {
            if (Light == null) return;

            float noise = Mathf.PerlinNoise(_noiseSeed, Time.time * FlickerSpeed);
            Light.intensity = BaseIntensity + (noise - 0.5f) * 2f * FlickerAmount;
        }
    }
    public class VfxTag : MonoBehaviour
    {
        public string VfxType; // "Fire", "Disease", etc.
    }

}
