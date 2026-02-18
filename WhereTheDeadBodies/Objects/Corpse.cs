using Planetbase;
using PlanetbaseModUtilities;
using System.Linq;
using UnityEngine;
using static Planetbase.Human;

namespace WhereTheDeadBodies
{
    internal class Corpse : ResourceType
    {
        public const string Name = "Corpse";
        private static GameObject[] mModels;
        private static Color[] mColors;

        public Corpse()
        {
            mStatsColor = new Color32(255, 255, 255, byte.MaxValue);
            mValue = -10;
            mMerchantCategory = MerchantCategory.Count;
            mSize = ResourceType.LargeResourceSize;

            mName = Name;
            mIcon = Util.applyColor(ContentManager.IconCorpse, mStatsColor);
            mModel = ResourceUtil.loadPrefab("Prefabs/Resources/PrefabMetal"); // A dummy prefab for built-in flow, and will be replaced immediately by ReplaceVisual()

            mModels = new GameObject[] {
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleBiologist"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanFemaleBiologist"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleEngineer"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanFemaleEngineer"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleGuard"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanFemaleGuard"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleMedic"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanFemaleMedic"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleWorker"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanFemaleWorker"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleVisitor"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanFemaleVisitor"),
                ResourceUtil.loadPrefab("Prefabs/Characters/PrefabAstronaut")
            };
            foreach(var item in mModels) {
                BoxCollider boxCollider = item.AddComponent<BoxCollider>();
                boxCollider.center = new Vector3(0f, 0.075f, 0f);
                boxCollider.size = mSize * 1.5f;
                item.findTaggedObject("CharacterExoskeleton")?.SetActive(false);
            }

            mColors = new Color[] {
                SpecializationList.find<Biologist>().getColor(),
                SpecializationList.find<Engineer>().getColor(),
                SpecializationList.find<Guard>().getColor(),
                SpecializationList.find<Medic>().getColor(),
                SpecializationList.find<Worker>().getColor(),
                SpecializationList.find<Visitor>().getColor()
            };
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("corpse", Name);
        }

        public static GameObject GetPrefab(Human human)
        {
            var index = 0;

            if(human.getLocation() == Location.Exterior) {
                index = mModels.Length - 1;
            }
            else { 
                var specialization = human.getSpecialization();
                if(specialization is Biologist)
                    index = 0;
                else
                if(specialization is Engineer)
                    index = 1;
                else
                if(specialization is Guard)
                    index = 2;
                else
                if(specialization is Medic)
                    index = 3;
                else
                if(specialization is Worker)
                    index = 4;
                else
                    index = 5; // Visitor, intruder or others

                var mGender = CoreUtils.GetMember<Human, Gender>("mGender", human);
                index = index * 2 + (mGender == Gender.Female ? 1 : 0);
                //WhereTheDeadBodies.ModEntry.Logger.Log($"GetPrefab index={index}, gender={mGender.ToString()}, specialization={human.getSpecialization().ToString()}");
            }

            return mModels[index];
        }

        public static void ReplaceVisual(Resource resource, GameObject prefab, bool isExterior, Color mColor)
        {
            // Replace the model of resource
            Object.Destroy(resource.getSelectionModel());
            var mModel = Object.Instantiate(prefab);
            mModel.name = "CorpseModel";
            mModel.GetComponent<Rigidbody>();
            mModel.disablePhysics();

            if(isExterior) {
                // Set color of exterior astronaut suit
                SkinnedMeshRenderer[] componentsInChildren = mModel.GetComponentsInChildren<SkinnedMeshRenderer>();
                for(int i = 0; i < componentsInChildren.Length; i++) {
                    Material val2 = Object.Instantiate<Material>(((Renderer)componentsInChildren[i]).sharedMaterial);
                    ((Renderer)componentsInChildren[i]).material = val2;
                    val2.SetColor(ShaderProperty.EmissionColor, mColor);
                }
            }

            CoreUtils.InvokeMethod<Resource>("setModel", resource, mModel);

            // Set animation
            var animationName = "human_die";
            var animation = mModel.AddComponent<Animation>();
            var animationClip = ResourceList.getInstance().PrefabHumanAnimations.GetComponent<AnimationList>().Animations.First(x => x.name == animationName);
            animation.AddClip(animationClip, animationClip.name);
            var animationState = animation[animationName];
            animationState.speed = 999;
            animationState.wrapMode = WrapMode.Clamp;
            animation.Play(animationName);
        }

        public static void ReplaceVisualRandom(Resource resource)
        {
            var isExterior = resource.getLocation() == Location.Exterior || Random.Range(0, 5) == 0;
            var prefab = isExterior ? mModels[mModels.Length - 1] : mModels[Random.Range(0, mModels.Length - 1)];
            var mColor = mColors[Random.Range(0, mColors.Length)];
            ReplaceVisual(resource, prefab, isExterior, mColor);
        }

        internal static void CreateResource(Human human)
        {
            // Create new resource object
            var resource = Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Corpse>(), human.getPosition(), human.getLocation());
            var isExterior = resource.getLocation() == Location.Exterior;
            var prefab = GetPrefab(human);
            var mColor = human.getSpecialization().getColor();

            // Copy rotation
            resource.setRotation(human.getRotation());

            ReplaceVisual(resource, prefab, isExterior, mColor);

            // Drop to ground
            resource.drop(Resource.State.Idle);

            // Fix position to ground
            resource.setPosition(new Vector3(resource.getPosition().x, resource.getPosition().y - 1f, resource.getPosition().z));
        }
    }
}
