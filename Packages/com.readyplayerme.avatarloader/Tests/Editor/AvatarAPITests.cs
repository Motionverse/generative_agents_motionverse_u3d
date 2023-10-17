using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarAPITests
    {
        private const string AVATAR_API_AVATAR_URL = "https://api.readyplayer.me/v1/avatars/638df693d72bffc6fa17943c.glb";
        private const int TEXTURE_SIZE_LOW = 256;
        private const int TEXTURE_SIZE_MED = 512;
        private const int TEXTURE_SIZE_HIGH = 1024;
        private const int AVATAR_CONFIG_BLEND_SHAPE_COUNT_MED = 15;

        private readonly List<GameObject> avatars = new List<GameObject>();
 
        private AvatarConfig avatarConfigHigh;
        private AvatarConfig avatarConfigLow;
        private AvatarConfig avatarConfigMed;
        private AvatarLoaderSettings settings;
        
        [OneTimeSetUp]
        public void Init()
        {
            avatarConfigLow = GetLowConfig();
            avatarConfigMed = GetMedConfig();
            avatarConfigHigh = GetHighConfig();
            settings = AvatarLoaderSettings.LoadSettings();
            settings.AvatarCachingEnabled = false;
        }
        
        [OneTimeTearDown]
        public void Cleanup()
        {
            AvatarCache.Clear();

            foreach (var avatar in avatars)
            {
                Object.DestroyImmediate(avatar);
            }
            avatars.Clear();
        }

        private AvatarConfig GetLowConfig()
        {
            var avatarConfig = ScriptableObject.CreateInstance<AvatarConfig>();
            avatarConfig.MeshLod = MeshLod.Low;
            avatarConfig.Pose = Pose.APose;
            avatarConfig.TextureAtlas = TextureAtlas.Low;
            avatarConfig.TextureSizeLimit = 256;
            avatarConfig.UseHands = false;
            avatarConfig.TextureChannel = GetAllTextureChannels();
            avatarConfig.UseDracoCompression = false;
            avatarConfig.UseMeshOptCompression = false;
            var morphTargets = new List<string>();
            morphTargets.Add("none");
            avatarConfig.MorphTargets = morphTargets;
            return avatarConfig;
        }

        private AvatarConfig GetMedConfig()
        {
            var avatarConfig = ScriptableObject.CreateInstance<AvatarConfig>();
            avatarConfig.MeshLod = MeshLod.Medium;
            avatarConfig.Pose = Pose.APose;
            avatarConfig.TextureAtlas = TextureAtlas.Medium;
            avatarConfig.TextureSizeLimit = 512;
            avatarConfig.UseHands = false;
            avatarConfig.TextureChannel = GetAllTextureChannels();
            avatarConfig.UseDracoCompression = false;
            avatarConfig.UseMeshOptCompression = false;
            var morphTargets = new List<string>();
            morphTargets.Add("Oculus Visemes");
            avatarConfig.MorphTargets = morphTargets;
            return avatarConfig;
        }

        private AvatarConfig GetHighConfig()
        {
            var avatarConfig = ScriptableObject.CreateInstance<AvatarConfig>();
            avatarConfig.MeshLod = MeshLod.High;
            avatarConfig.Pose = Pose.APose;
            avatarConfig.TextureAtlas = TextureAtlas.High;
            avatarConfig.TextureSizeLimit = 1024;
            avatarConfig.UseHands = false;
            avatarConfig.TextureChannel = GetAllTextureChannels();
            avatarConfig.UseDracoCompression = false;
            avatarConfig.UseMeshOptCompression = false;
            var morphTargets = new List<string>();
            morphTargets.Add("Oculus Visemes");
            avatarConfig.MorphTargets = morphTargets;
            return avatarConfig;
        }

        private TextureChannel[] GetAllTextureChannels()
        {
            var textureChannels = new TextureChannel[5];
            textureChannels[0] = TextureChannel.BaseColor;
            textureChannels[1] = TextureChannel.Normal;
            textureChannels[2] = TextureChannel.MetallicRoughness;
            textureChannels[3] = TextureChannel.Emissive;
            textureChannels[4] = TextureChannel.Occlusion;
            return textureChannels;
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_Mesh_LOD()
        {
            var avatarConfigs = new Queue<AvatarConfig>();
            avatarConfigs.Enqueue(avatarConfigLow);
            avatarConfigs.Enqueue(avatarConfigMed);
            avatarConfigs.Enqueue(avatarConfigHigh);

            var vertexCounts = new List<int>();

            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();

            loader.OnCompleted += (sender, args) =>
            {
                GameObject avatar = args.Avatar;
                avatars.Add(avatar);
                vertexCounts.Add(avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.vertexCount);
                if (avatarConfigs.Count == 0) return;
                loader.AvatarConfig = avatarConfigs.Dequeue();
                loader.LoadAvatar(AVATAR_API_AVATAR_URL);
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };

            loader.AvatarConfig = avatarConfigs.Dequeue();
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => vertexCounts.Count == 3 || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.Less(vertexCounts[0], vertexCounts[1]);
            Assert.Less(vertexCounts[1], vertexCounts[2]);
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_TextureSize()
        {
            var avatarConfigs = new Queue<AvatarConfig>();
            avatarConfigs.Enqueue(avatarConfigLow);
            avatarConfigs.Enqueue(avatarConfigMed);
            avatarConfigs.Enqueue(avatarConfigHigh);

            var textureSizes = new List<int>();

            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();

            loader.OnCompleted += (sender, args) =>
            {
                GameObject avatar = args.Avatar;
                avatars.Add(avatar);
                textureSizes.Add(avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.mainTexture.width);
                if (avatarConfigs.Count == 0) return;
                loader.AvatarConfig = avatarConfigs.Dequeue();
                loader.LoadAvatar(AVATAR_API_AVATAR_URL);
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };

            loader.AvatarConfig = avatarConfigs.Dequeue();
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => textureSizes.Count == 3 || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.AreEqual(TEXTURE_SIZE_LOW, textureSizes[0]);
            Assert.AreEqual(TEXTURE_SIZE_MED, textureSizes[1]);
            Assert.AreEqual(TEXTURE_SIZE_HIGH, textureSizes[2]);
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_MorphTargets_None()
        {
            GameObject avatar = null;
            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();
            loader.OnCompleted += (sender, args) =>
            {
                avatar = args.Avatar;
                avatars.Add(avatar);
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };
            loader.AvatarConfig = avatarConfigLow;
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => avatar != null || failureType != FailureType.None);

            var blendShapeCount = avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.blendShapeCount;
            Debug.Log(blendShapeCount);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(avatar);
            Assert.Zero(blendShapeCount);
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_MorphTargets_Oculus()
        {
            GameObject avatar = null;
            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();
            loader.OnCompleted += (sender, args) =>
            {
                avatar = args.Avatar;
                avatars.Add(avatar);
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };
            loader.AvatarConfig = avatarConfigMed;
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => avatar != null || failureType != FailureType.None);

            var blendShapeCount = avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.blendShapeCount;

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(avatar);
            Assert.AreEqual(AVATAR_CONFIG_BLEND_SHAPE_COUNT_MED, blendShapeCount);
        }
        
        [Test] 
        public async Task AvatarLoader_Avatar_API_MeshOptCompression()
        {
            var avatarConfig = ScriptableObject.CreateInstance<AvatarConfig>();
            avatarConfig.MeshLod = MeshLod.Low;
            avatarConfig.TextureAtlas = TextureAtlas.Low;
            avatarConfig.MorphTargets = new List<string> { "none" };
            avatarConfig.TextureChannel = new [] { TextureChannel.BaseColor };

            var downloader = new AvatarDownloader();
            var normalBytes = await downloader.DownloadIntoMemory(AVATAR_API_AVATAR_URL, avatarConfig);
            
            avatarConfig.UseMeshOptCompression = true;
            var meshOptBytes = await downloader.DownloadIntoMemory(AVATAR_API_AVATAR_URL, avatarConfig);
            
            Assert.Less(meshOptBytes.Length, normalBytes.Length);
        }
    }
}
