using System;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible for making processing the avatar after it has been loaded into a GameObject.
    /// </summary>
    public class AvatarProcessor : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarProcessor);

        public int Timeout { get; set; }

        /// <summary>
        /// An <see cref="Action" /> callback that can be used to subscribe to <see cref="WebRequestDispatcherExtension" />
        /// <c>ProgressChanged</c> events.
        /// </summary>
        public Action<float> ProgressChanged { get; set; }

        /// <summary>
        /// Executes the operation to process the avatar <see cref="GameObject" />.
        /// </summary>
        /// <param name="context">A container for all the data related to the Avatar model.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The updated <c>AvatarContext</c>.</returns>
        public Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.Data is GameObject)
            {
                context = ProcessAvatarGameObject(context);
                ProcessAvatar(context.Data as GameObject, context.Metadata);
                ProgressChanged?.Invoke(1);
                return Task.FromResult(context);
            }

            throw new CustomException(FailureType.AvatarProcessError, $"Avatar postprocess failed. {context.Data} is either null or is not of type GameObject");
        }

        /// <summary>
        /// Replaces the instance of the avatar GameObject if it already exists and sets the name of the GameObject.
        /// </summary>
        /// <param name="context">A container for all the data related to the Avatar model.</param>
        /// <returns>The <see cref="AvatarContext" />.</returns>
        private AvatarContext ProcessAvatarGameObject(AvatarContext context)
        {
#if UNITY_EDITOR
            if (context.SaveInProjectFolder)
            {
                Object.DestroyImmediate((Object) context.Data);
                AssetDatabase.Refresh();
                var path = $"{DirectoryUtility.GetRelativeProjectPath(context.AvatarUri.Guid)}/{context.AvatarUri.Guid}.glb";
                var avatarAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                context.Data = Object.Instantiate(avatarAsset);
            }
#endif
            GameObject oldInstance = GameObject.Find(context.AvatarUri.Guid);
            if (oldInstance)
            {
                Object.DestroyImmediate(oldInstance);
            }

            ((Object) context.Data).name = context.AvatarUri.Guid;

            return context;
        }

        /// <summary>
        /// This method triggers GameObject changes and setup for the new avatar GameObject.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="avatarMetadata"></param>
        public void ProcessAvatar(GameObject avatar, AvatarMetadata avatarMetadata)
        {
            SDKLogger.Log(TAG, PROCESSING_AVATAR);

            try
            {
                if (avatar.transform.Find(BONE_HALF_BODY_ROOT))
                {
                    RemoveHalfBodyRoot(avatar);
                }

                if (!avatar.transform.Find(BONE_ARMATURE))
                {
                    AddArmatureBone(avatar);
                }

                if (avatarMetadata.BodyType == BodyType.FullBody)
                {
                    SetupAnimator(avatar, avatarMetadata.OutfitGender);
                }

                RenameChildMeshes(avatar);
            }
            catch (Exception e)
            {
                var message = $"Avatar postprocess failed. {e.Message}";
                SDKLogger.Log(TAG, message);
                throw new CustomException(FailureType.AvatarProcessError, message);
            }
        }


        #region Setup Armature and Animations

        // Animation avatars resource paths
        private const string MASCULINE_ANIMATION_AVATAR_NAME = "AnimationAvatars/MasculineAnimationAvatar";
        private const string FEMININE_ANIMATION_AVATAR_NAME = "AnimationAvatars/FeminineAnimationAvatar";

        // Bone names
        private const string BONE_HIPS = "Hips";
        private const string BONE_ARMATURE = "Armature";
        private const string BONE_HALF_BODY_ROOT = "AvatarRoot";

        /// <summary>
        /// Removes the roo bone to ensure the correct skeleton hierarchy.
        /// </summary>
        /// <param name="avatar">The <see cref="GameObject" /> to update.</param>
        private void RemoveHalfBodyRoot(GameObject avatar)
        {
            Transform root = avatar.transform.Find(BONE_HALF_BODY_ROOT);
            for (var i = root.childCount - 1; i >= 0; --i)
            {
                root.GetChild(i).transform.SetParent(avatar.transform);
            }
            Object.DestroyImmediate(root.gameObject);
        }

        /// <summary>
        /// Adds the root armature bone to ensure the correct skeleton hierarchy.
        /// </summary>
        /// <param name="avatar">The <see cref="GameObject" /> to update.</param>
        private void AddArmatureBone(GameObject avatar)
        {
            SDKLogger.Log(TAG, ADDING_ARMATURE_BONE);

            var armature = new GameObject();
            armature.name = BONE_ARMATURE;
            armature.transform.parent = avatar.transform;

            Transform hips = avatar.transform.Find(BONE_HIPS);
            if (hips) hips.parent = armature.transform;
        }

        /// <summary>
        /// Adds an <see cref="Animator" /> component and sets the target <see cref="UnityEngine.Avatar" />.
        /// </summary>
        /// <param name="avatar">The <see cref="GameObject" /> to update.</param>
        /// <param name="gender">Get gender of the Avatar.</param>
        private void SetupAnimator(GameObject avatar, OutfitGender gender)
        {
            SDKLogger.Log(TAG, SETTING_UP_ANIMATOR);

            var animationAvatarSource = gender == OutfitGender.Masculine
                ? MASCULINE_ANIMATION_AVATAR_NAME
                : FEMININE_ANIMATION_AVATAR_NAME;
            var animationAvatar = Resources.Load<Avatar>(animationAvatarSource);
            var animator = avatar.AddComponent<Animator>();
            animator.avatar = animationAvatar;
            animator.applyRootMotion = true;
        }

        #endregion

        #region Set Component Names

        // Prefix to remove from names for correction
        private const string PREFIX = "Wolf3D_";

        // Default prefixes
        private const string AVATAR_PREFIX = "Avatar";
        private const string RENDERER_PREFIX = "Renderer";
        private const string MATERIAL_PREFIX = "Material";
        private const string SKINNED_MESH_PREFIX = "SkinnedMesh";
        private const string PROCESSING_AVATAR = "Processing avatar.";
        private const string ADDING_ARMATURE_BONE = "Adding armature bone";
        private const string SETTING_UP_ANIMATOR = "Setting up animator";


        // Texture property IDs
        private static readonly string[] ShaderProperties =
        {
            "baseColorTexture",
            "normalTexture",
            "emissiveTexture",
            "occlusionTexture",
            "metallicRoughnessTexture"
        };

        /// <summary>
        /// Rename avatar assets.
        /// </summary>
        /// <param name="avatar">The <see cref="GameObject" /> to update.</param>
        /// <remarks>Naming convention is 'Avatar_Type_Name'. This makes it easier to view them in profiler</remarks>
        private void RenameChildMeshes(GameObject avatar)
        {
            SkinnedMeshRenderer[] renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                var assetName = renderer.name.Replace(PREFIX, "");

                renderer.name = $"{RENDERER_PREFIX}_{assetName}";
                renderer.sharedMaterial.name = $"{MATERIAL_PREFIX}_{assetName}";
                SetTextureNames(renderer, assetName);
                SetMeshName(renderer, assetName);
            }
        }

        /// <summary>
        /// Set the names of each <see cref="Texture" />.
        /// </summary>
        /// <param name="renderer">Search for textures in this renderer.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <remarks>Naming convention is 'Avatar_PropertyName_AssetName'. This makes it easier to view them in profiler</remarks>
        private void SetTextureNames(Renderer renderer, string assetName)
        {
            foreach (var propertyName in ShaderProperties)
            {
                var propertyID = Shader.PropertyToID(propertyName);

                if (renderer.sharedMaterial.HasProperty(propertyID))
                {
                    Texture texture = renderer.sharedMaterial.GetTexture(propertyID);
                    if (texture != null) texture.name = $"{AVATAR_PREFIX}{propertyName}_{assetName}";
                }
            }
        }

        /// <summary>
        /// Set the name of the <see cref="SkinnedMeshRenderer" />.
        /// </summary>
        /// <param name="renderer">SkinMeshRenderer to update.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <remarks>Naming convention is 'SkinnedMesh_AssetName'. This makes it easier to view in profiler</remarks>
        private void SetMeshName(SkinnedMeshRenderer renderer, string assetName)
        {
            renderer.sharedMesh.name = $"{SKINNED_MESH_PREFIX}_{assetName}";
            renderer.updateWhenOffscreen = true;
        }

        #endregion
    }
}
