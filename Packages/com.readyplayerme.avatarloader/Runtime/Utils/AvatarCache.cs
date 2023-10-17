using System.IO;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible for managing the avatar cache that is used for storing the avatar assets locally.
    /// </summary>
    public static class AvatarCache
    {
        /// Calculate cache subfolder name based on hash for avatar Config.
        public static string GetAvatarConfigurationHash(AvatarConfig avatarConfig = null)
        {
            var hash = avatarConfig ? Hash128.Compute(AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig)).ToString() : Hash128.Compute("none").ToString();
            return hash;
        }

        /// Clears the avatars from the persistent cache.
        public static void Clear()
        {
            var path = DirectoryUtility.GetAvatarsDirectoryPath();
            DeleteFolder(path);
        }

        private static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
#if UNITY_EDITOR
            path += ".meta";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
#endif
        }
        
        /// Deletes stored data a specific avatar from persistent cache.
        public static void DeleteAvatarFolder(string guid, bool saveInProjectFolder = false)
        {
            var path = $"{DirectoryUtility.GetAvatarsDirectoryPath(saveInProjectFolder)}/{guid}";
            DeleteFolder(path);
        }
        
        /// deletes a specific avatar model from persistent cache, while leaving the metadata.json file
        public static void DeleteAvatarModel(string guid, bool saveInProjectFolder = false)
        {
            var path = $"{DirectoryUtility.GetAvatarsDirectoryPath(saveInProjectFolder)}/{guid}";
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);

                if (saveInProjectFolder)
                {
#if UNITY_EDITOR
                    foreach (DirectoryInfo dir in info.GetDirectories())
                    {
                        AssetDatabase.DeleteAsset($"Assets/{DirectoryUtility.DefaultAvatarFolder}/{guid}/{dir.Name}");
                    }
#endif
                }
                else
                {
                    foreach (DirectoryInfo dir in info.GetDirectories())
                    {
                        Directory.Delete(dir.FullName, true);
                    }
                }
            }
        }

        /// Is there any avatars present in the persistent cache.
        public static bool IsCacheEmpty()
        {
            var path = DirectoryUtility.GetAvatarsDirectoryPath();
            return !Directory.Exists(path) ||
                   Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }

        /// Total Avatars stored in persistent cache.
        public static int GetAvatarCount()
        {
            var path = DirectoryUtility.GetAvatarsDirectoryPath();
            return !Directory.Exists(path) ? 0 : new DirectoryInfo(path).GetDirectories().Length;

        }

        /// Total Avatar variants stored for specific avatar GUID in persistent cache.
        public static int GetAvatarVariantCount(string avatarGuid)
        {
            var path = $"{DirectoryUtility.GetAvatarsDirectoryPath()}/{avatarGuid}";
            return !Directory.Exists(path) ? 0 : new DirectoryInfo(path).GetDirectories().Length;

        }

        /// Total size of avatar stored in persistent cache. Returns total bytes.
        public static long GetCacheSize()
        {
            var path = DirectoryUtility.GetAvatarsDirectoryPath();
            return !Directory.Exists(path) ? 0 : DirectoryUtility.GetDirectorySize(new DirectoryInfo(path));
        }

        public static float GetCacheSizeInMb()
        {
            var path = DirectoryUtility.GetAvatarsDirectoryPath();
            return DirectoryUtility.GetFolderSizeInMb(path);
        }

        public static float GetAvatarDataSizeInMb(string avatarGuid)
        {
            var path = $"{DirectoryUtility.GetAvatarsDirectoryPath()}/{avatarGuid}";
            return DirectoryUtility.GetFolderSizeInMb(path);
        }
    }
}
