using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Planetbase.Human;
using PlanetbaseModUtilities;
using System.Xml.Linq;

namespace WhereTheDeadBodies
{
    internal class Corpse : ResourceType
    {
        public const string Name = "Corpse";

        //private GameObject mObject;
        //protected GameObject mBody;
        //protected GameObject mExoskeleton;
        //protected Animation mAnimation;

        public Corpse()
        {
            //mObject = new GameObject();
            mStatsColor = new Color32(255, 0, 0, byte.MaxValue);
            mValue = 10;
            mMerchantCategory = MerchantCategory.Count;
            mSize = ResourceType.LargeResourceSize;

            mName = Name;
            mModel = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleWorker");
            //setModel(ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleWorker"));
            mIcon = Util.applyColor(ContentManager.IconCorpse, mStatsColor);

            BoxCollider boxCollider = mModel.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0f, 0.075f, 0f);
            boxCollider.size = mSize;//new Vector3(mRadius * 2f, 0.15f, mLength);

            //new Bounds(getPosition() + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));

            // TODO: set model for exterior, specialization and gender
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("corpse", Name);
        }

        internal static void CreateFromCharacter(Character character)
        {
            if(!(character is Human)) 
                return;            

            // Duplicate game object, but skin will become purple after the origin gone. T pose of corpse that outside current view
            var oldObject = character.getGameObject();

            var location = character.getLocation();

            var resource = Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Corpse>(), oldObject.transform.position, location);
            resource.drop(Resource.State.Idle);

            //var newObject = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabHumanMaleWorker");
            //CoreUtils.SetMember<Resource, GameObject>("mModel", resource, newObject);

            var newObject = CoreUtils.GetMember<Resource, GameObject>("mModel", resource);
            newObject.transform.rotation = oldObject.transform.rotation;

            // ExoSkeleton
            var mExoskeleton = newObject.findTaggedObject("CharacterExoskeleton");
            mExoskeleton.SetActive(false);

            // Animation
            var characterAnimation = new CharacterAnimation(CharacterAnimationType.Die);
            var gender = CoreUtils.GetMember<Human, Gender>("mGender", (Human)character);
            var mAnimation = addHumanAnimations(newObject, gender, location);

            var animationType = characterAnimation.getAnimationType();

            var mSpecialization = character.getSpecialization();
            var mExteriorAnimationNames = CoreUtils.GetMember<Specialization, List<string>[]>("mExteriorAnimationNames", mSpecialization);
            var mFemaleAnimationNames = CoreUtils.GetMember<Specialization, List<string>[]>("mFemaleAnimationNames", mSpecialization);
            var mAnimationNames = CoreUtils.GetMember<Specialization, List<string>[]>("mAnimationNames", mSpecialization);

            List<string> animationNames;
            if(location == Location.Exterior && mExteriorAnimationNames != null)
                animationNames = mExteriorAnimationNames[(int)animationType];
            else
            if(gender == Human.Gender.Female && mFemaleAnimationNames != null)
                animationNames = mFemaleAnimationNames[(int)animationType];
            else
                animationNames = mAnimationNames[(int)animationType];

            var animationName = ((animationNames.Count != 1) ? animationNames[UnityEngine.Random.Range(0, animationNames.Count)] : animationNames[0]);
            var mCurrentAnimationState = mAnimation[animationName];
            mCurrentAnimationState.speed = 999;
            mCurrentAnimationState.wrapMode = WrapMode.ClampForever;
            mAnimation.Play(animationName);
        }

        public static void RandomVisual(Resource resource)
        {
            var location = resource.getLocation();

            var mModel = CoreUtils.GetMember<Resource, GameObject>("mModel", resource);
            if(mModel == null)
                return;

            var rotation = UnityEngine.Random.rotation;
            rotation.x = 0;
            rotation.z = 0;
            mModel.transform.rotation = rotation;

            // ExoSkeleton
            var mExoskeleton = mModel.findTaggedObject("CharacterExoskeleton");
            mExoskeleton.SetActive(false);

            // Animation
            var characterAnimation = new CharacterAnimation(CharacterAnimationType.Die);
            var gender = Gender.Male; // CoreUtils.GetMember<Human, Gender>("mGender", (Human)character);
            var mAnimation = addHumanAnimations(mModel, gender, location);

            var animationType = characterAnimation.getAnimationType();

            var mSpecialization =  SpecializationList.find<Worker>(); // character.getSpecialization();
            var mExteriorAnimationNames = CoreUtils.GetMember<Specialization, List<string>[]>("mExteriorAnimationNames", mSpecialization);
            var mFemaleAnimationNames = CoreUtils.GetMember<Specialization, List<string>[]>("mFemaleAnimationNames", mSpecialization);
            var mAnimationNames = CoreUtils.GetMember<Specialization, List<string>[]>("mAnimationNames", mSpecialization);

            List<string> animationNames;
            if(location == Location.Exterior && mExteriorAnimationNames != null)
                animationNames = mExteriorAnimationNames[(int)animationType];
            else
            if(gender == Human.Gender.Female && mFemaleAnimationNames != null)
                animationNames = mFemaleAnimationNames[(int)animationType];
            else
                animationNames = mAnimationNames[(int)animationType];

            var animationName = ((animationNames.Count != 1) ? animationNames[UnityEngine.Random.Range(0, animationNames.Count)] : animationNames[0]);
            var mCurrentAnimationState = mAnimation[animationName];
            mCurrentAnimationState.speed = 999;
            mCurrentAnimationState.wrapMode = WrapMode.ClampForever;
            mAnimation.Play(animationName);
        }

        public void setModel(GameObject newModel)
        {
            ////if(mObject != null) 
            //{
            //    UnityEngine.Object.Destroy(mModel);
            //    mModel = newModel;
            //    //mModel.transform.SetParent(mObject.transform, worldPositionStays: false);
            //    mModel.name = "Character Model";
            //    mBody = mModel.findTaggedObject("CharacterBody");
            //    if(mBody == null) {
            //        Debug.LogWarning("Character has no body: " + getName());
            //    }

            //    mAnimation = mModel.GetComponentInChildren<Animation>();
            //    mModel.setLayerRecursive(14);
            //}

            ////mObject.setLayerRecursive((mLocation == Location.Exterior) ? 17 : 14);
            ////mObject.setLayerRecursive(14);
        }

        private static Animation addHumanAnimations(GameObject model, Human.Gender gender, Location location)
        {
            Animation animation = model.AddComponent<Animation>();
            animation.playAutomatically = false;
            animation.cullingType = AnimationCullingType.BasedOnRenderers;
            AnimationClip[] animations = ResourceList.getInstance().PrefabHumanAnimations.GetComponent<AnimationList>().Animations;
            foreach(AnimationClip animationClip in animations) {
                if(!(animationClip != null)) {
                    continue;
                }
                string name = animationClip.name;
                if(animation[name] != null) {
                    Debug.LogWarning("Duplicated animation: " + name);
                }
                bool flag = false;
                if(location == Location.Exterior) {
                    flag = name.StartsWith("human_astronaut") || (name.StartsWith("human_") && !name.StartsWith("human_male") && !name.StartsWith("human_female"));
                }
                else {
                    switch(gender) {
                        case Human.Gender.Male:
                            flag = name.StartsWith("human_male") || (name.StartsWith("human_") && !name.StartsWith("human_astronaut") && !name.StartsWith("human_female"));
                            break;
                        case Human.Gender.Female:
                            flag = name.StartsWith("human_female") || (name.StartsWith("human_") && !name.StartsWith("human_astronaut") && !name.StartsWith("human_male"));
                            break;
                    }
                }
                if(flag) {
                    animation.AddClip(animationClip, animationClip.name);
                }
            }

            return animation;
        }
    }
}
