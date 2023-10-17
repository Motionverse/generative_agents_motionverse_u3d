using System.IO;
using System.Linq;
using UnityEngine;

namespace ReadyPlayerMe.Core
{
    public static class DirectoryUtility
    {
        private const float BYTES_IN_MEGABYTE = 1024 * 1024;

        /// The directory where avatar files will be downloaded.
        public static string DefaultAvatarFolder { get; set; } = "Ready Player Me/Avatars";

        public static void ValidateAvatarSaveDirectory(string guid, bool saveInProjectFolder = false)
        {
            ValidateDirectory(GetAvatarSaveDirectory(guid, saveInProjectFolder));
        }

        public static void ValidateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetAvatarSaveDirectory(string guid, bool saveInProjectFolder = false, string paramsHash = null)
        {
            return saveInProjectFolder ? $"{GetAvatarsDirectoryPath(true)}/{guid}" : $"{GetAvatarsDirectoryPath()}/{guid}/{paramsHash}";
        }

        public static string GetRelativeProjectPath(string guid)
        {
            return $"Assets/{DefaultAvatarFolder}/{guid}";
        }

        public static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            // Add file sizes.
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            var size = fileInfos.Sum(fi => fi.Length);

            // Add subdirectory sizes.
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            size += directoryInfos.Sum(GetDirectorySize);
            return size;
        }

        public static float GetFolderSizeInMb(string folderPath)
        {
            var bytes = GetDirectorySize(new DirectoryInfo(folderPath));
            return !Directory.Exists(folderPath) ? 0 : BytesToMegabytes(bytes);
        }

        private static float BytesToMegabytes(long bytes)
        {
            return bytes / BYTES_IN_MEGABYTE;
        }

        public static string GetAvatarsDirectoryPath(bool saveInProjectFolder = false)
        {
            var directory = saveInProjectFolder ? Application.dataPath : Application.persistentDataPath;
            return $"{directory}/{DefaultAvatarFolder}";
        }
    }
}
