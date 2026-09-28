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
            noise.strength = 0.15f;
            noise.frequency = 0.12f;
            noise.scrollSpeed = 0.1f;
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
            // IMPORTANT: x, y and z must all share the same MinMaxCurve mode (here,
            // "random between two constants"). Leaving y at its default Constant(0)
            // mode causes a mode-mismatch exception once the module is evaluated.
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
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
        // Blood splatter: one-shot burst of dark red droplets from a point
        // ---------------------------------------------------------------
        private static Material _bloodMaterial;
        private static Texture2D _dropletTexture;

        /// <summary>
        /// Attaches a one-shot blood splatter to the specified GameObject, so the emitter
        /// follows it (e.g. a moving character). Droplets are simulated in world space,
        /// so they fly off and fall naturally rather than being dragged along.
        /// 'localOffset' positions the wound relative to the target's pivot.
        /// Destroys itself automatically after a short period.
        /// </summary>
        public static GameObject AttachBloodVfx(GameObject targetObject, float intensity = 1f)
        {
            if (targetObject == null)
            {
                Debug.LogWarning("VfxHelper.AttachBloodVfx: targetObject is null.");
                return null;
            }
            intensity = Mathf.Max(0.1f, intensity);

            GameObject container = new GameObject("Mod_BloodVFX");
            container.transform.SetParent(targetObject.transform, false);

            BuildBloodDroplets(container.transform, intensity);
            BuildBloodMist(container.transform, intensity);

            Object.Destroy(container, 1.5f);
            return container;
        }

        private static GameObject BuildBloodDroplets(Transform parent, float intensity)
        {
            GameObject obj = new GameObject("BloodDroplets");
            obj.transform.SetParent(parent, false);

            ParticleSystem ps = obj.AddComponent<ParticleSystem>();
            // AddComponent starts playing; stop first so duration can be safely changed.
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.5f;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(3f * intensity, 8f * intensity);
            main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.09f);
            main.gravityModifier = 2.5f; // heavy droplets fall fast
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.45f, 0.0f, 0.0f, 1f), new Color(0.7f, 0.02f, 0.02f, 1f));
            main.maxParticles = 200;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Single burst - no continuous emission
            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
            new ParticleSystem.Burst(0f, (short)Mathf.RoundToInt(35f * intensity))
            });

            // Sphere emits outward from the center in every direction
            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.05f; // near-point source

            // Droplets shrink slightly and fade right at the end of their life
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(1f, 0.6f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient fade = new Gradient();
            fade.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.8f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = fade;

            // Droplets hit the ground/walls and lose most of their energy (no bouncing)
            var collision = ps.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.quality = ParticleSystemCollisionQuality.Low;
            collision.dampen = 0.7f;
            collision.bounce = 0.05f;
            collision.lifetimeLoss = 0.4f;
            collision.radiusScale = 0.5f;

            // Stretched billboards turn round sprites into fast-moving streaks
            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetBloodMaterial();
                renderer.renderMode = ParticleSystemRenderMode.Stretch;
                renderer.velocityScale = 0.03f;
                renderer.lengthScale = 1.5f;
            }

            ps.Play();
            return obj;
        }

        private static GameObject BuildBloodMist(Transform parent, float intensity)
        {
            GameObject obj = new GameObject("BloodMist");
            obj.transform.SetParent(parent, false);

            ParticleSystem ps = obj.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.3f;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f * intensity);
            main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.5f);
            main.gravityModifier = 0.2f;
            main.startColor = new Color(0.5f, 0.02f, 0.02f, 0.35f);
            main.maxParticles = 20;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
            new ParticleSystem.Burst(0f, (short)Mathf.RoundToInt(8f * intensity))
            });

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.05f;

            // Soft puff that expands and fades quickly
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.5f);
            sizeCurve.AddKey(1f, 1.5f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient fade = new Gradient();
            fade.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = fade;

            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetSmokeMaterial(); // soft alpha-blended puff
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }

            ps.Play();
            return obj;
        }

        /// <summary>
        /// Round droplet texture with a much crisper edge than the soft puff texture,
        /// so blood reads as liquid rather than mist.
        /// </summary>
        private static Texture2D GetDropletTexture()
        {
            if (_dropletTexture != null)
            {
                return _dropletTexture;
            }

            const int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxDist = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                    // Solid core, narrow soft rim (only the outer ~20% fades)
                    float alpha = Mathf.Clamp01((1f - dist) / 0.2f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            tex.Apply();
            _dropletTexture = tex;
            return _dropletTexture;
        }

        private static Material GetBloodMaterial()
        {
            if (_bloodMaterial == null)
            {
                Shader shader = FindBestParticleShader(additive: false);
                _bloodMaterial = shader != null ? new Material(shader) : null;
                if (_bloodMaterial != null)
                {
                    _bloodMaterial.mainTexture = GetDropletTexture();
                }
            }
            return _bloodMaterial;
        }

        // ---------------------------------------------------------------
        // Welding sparks: continuous bright streaking sparks + flash + flicker light
        // ---------------------------------------------------------------
        private static Material _sparkMaterial;

        /// <summary>
        /// Attaches a continuous welding-spark effect to the specified GameObject:
        /// fast, bright, bouncing sparks, a pulsing white-blue flash at the weld point,
        /// and a rapidly flickering light. The sparks spray upward from the object's
        /// local up axis. Returns the container so it can be Destroy()'d to stop welding.
        /// 'localOffset' positions the weld point relative to the target's pivot.
        /// </summary>
        public static GameObject AttachSparksVfx(GameObject targetObject)
        {
            if (targetObject == null)
            {
                Debug.LogWarning("VfxHelper.AttachWeldingSparksVfx: targetObject is null.");
                return null;
            }

            GameObject container = new GameObject("Mod_WeldingVFX");
            container.transform.SetParent(targetObject.transform, false);
            container.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            container.transform.localRotation = Quaternion.identity;

            BuildWeldingSparks(container.transform);
            BuildWeldingFlash(container.transform);

            // Harsh, fast, blue-white flicker (reuses the Perlin flicker component)
            GameObject lightObj = new GameObject("WeldLight");
            lightObj.transform.SetParent(container.transform, false);
            lightObj.transform.localPosition = new Vector3(0f, 0.15f, 0f);

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.8f, 0.9f, 1f);
            light.range = 4f;
            light.intensity = 3f;
            light.shadows = LightShadows.None;

            FireLightFlicker flicker = container.AddComponent<FireLightFlicker>();
            flicker.Light = light;
            flicker.BaseIntensity = 3f;
            flicker.FlickerAmount = 2.5f;
            flicker.FlickerSpeed = 25f;

            Object.Destroy(container, 1.5f);
            return container;
        }

        private static GameObject BuildWeldingSparks(Transform parent)
        {
            GameObject obj = new GameObject("WeldSparks");
            obj.transform.SetParent(parent, false);
            obj.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // cone points along local up

            ParticleSystem ps = obj.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.25f;
            main.loop = true;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
            main.gravityModifier = 1.5f;
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 1f, 0.85f), new Color(1f, 0.85f, 0.4f));
            main.maxParticles = 300;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Steady stream plus a small burst every cycle so it crackles in pulses
            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 25f;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
            new ParticleSystem.Burst(0f, (short)12)
            });

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 50f;
            shape.radius = 0.02f; // near-point source

            // White-hot -> yellow -> orange -> red as the spark cools, fading at the end
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient sparkGradient = new Gradient();
            sparkGradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(new Color(1f, 1f, 0.9f), 0f),
                new GradientColorKey(new Color(1f, 0.85f, 0.3f), 0.3f),
                new GradientColorKey(new Color(1f, 0.4f, 0.05f), 0.7f),
                new GradientColorKey(new Color(0.6f, 0.1f, 0f), 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.6f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = sparkGradient;

            // Sparks shrink to nothing as they burn out
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(1f, 0f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Slight jitter so paths aren't perfectly ballistic
            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.3f;
            noise.frequency = 2f;
            noise.scrollSpeed = 1f;
            noise.damping = false;
            noise.quality = ParticleSystemNoiseQuality.Low;

            // Sparks skitter and bounce off the floor
            var collision = ps.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.quality = ParticleSystemCollisionQuality.Low;
            collision.dampen = 0.3f;
            collision.bounce = 0.4f;
            collision.lifetimeLoss = 0.2f;
            collision.radiusScale = 0.5f;

            // Stretched additive billboards read as bright streaks
            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetSparkMaterial();
                renderer.renderMode = ParticleSystemRenderMode.Stretch;
                renderer.velocityScale = 0.04f;
                renderer.lengthScale = 2f;
            }

            ps.Play();
            return obj;
        }

        private static GameObject BuildWeldingFlash(Transform parent)
        {
            GameObject obj = new GameObject("WeldFlash");
            obj.transform.SetParent(parent, false);

            ParticleSystem ps = obj.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Rapid stream of short-lived, stationary glowing blobs = pulsing arc glare
            var main = ps.main;
            main.duration = 1f;
            main.loop = true;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
            main.startSpeed = 0f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.5f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.8f, 0.9f, 1f, 0.9f), new Color(1f, 1f, 1f, 0.9f));
            main.maxParticles = 20;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 30f;

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.02f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient fade = new Gradient();
            fade.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(new Color(0.6f, 0.75f, 1f), 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = fade;

            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = GetFlameMaterial(); // additive + soft round texture = glow
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }

            ps.Play();
            return obj;
        }

        private static Material GetSparkMaterial()
        {
            if (_sparkMaterial == null)
            {
                Shader shader = FindBestParticleShader(additive: true);
                _sparkMaterial = shader != null ? new Material(shader) : null;
                if (_sparkMaterial != null)
                {
                    _sparkMaterial.mainTexture = GetDropletTexture(); // crisp round core for sharp streaks
                }
            }
            return _sparkMaterial;
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
}
