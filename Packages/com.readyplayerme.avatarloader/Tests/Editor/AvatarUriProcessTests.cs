using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarUriProcessTests
    {
        private const string GUID = "633af24a573a46702919320f";
        private const string GUID_URL = "https://api.readyplayer.me/v1/avatars/633af24a573a46702919320f.glb";

        private const string SHORT_CODE = "DDBWOI";
        private const string SHORT_CODE_URL = "https://api.readyplayer.me/v1/avatars/DDBWOI.glb";

        private const string RANDOM_PARAM_HASH = "123456789";

        private readonly AvatarUri expectedShortcodeUri = new AvatarUri
        {
            Guid = SHORT_CODE,
            ModelUrl = $"{TestUtils.API_URL_PREFIX}{SHORT_CODE}{TestUtils.GLB_SUFFIX}",
            LocalModelPath = $"{DirectoryUtility.GetAvatarSaveDirectory(SHORT_CODE, false, RANDOM_PARAM_HASH)}/{SHORT_CODE}{TestUtils.GLB_SUFFIX}",
            MetadataUrl = $"{TestUtils.API_URL_PREFIX}{SHORT_CODE}{TestUtils.JSON_SUFFIX}",
            LocalMetadataPath = $"{DirectoryUtility.GetAvatarSaveDirectory(SHORT_CODE)}{SHORT_CODE}{TestUtils.JSON_SUFFIX}"
        };

        private readonly AvatarUri expectedUri = new AvatarUri
        {
            Guid = GUID,
            ModelUrl = $"{TestUtils.API_URL_PREFIX}{GUID}{TestUtils.GLB_SUFFIX}",
            LocalModelPath = $"{DirectoryUtility.GetAvatarSaveDirectory(GUID, false, RANDOM_PARAM_HASH)}/{GUID}{TestUtils.GLB_SUFFIX}",
            MetadataUrl = $"{TestUtils.API_URL_PREFIX}{GUID}{TestUtils.JSON_SUFFIX}",
            LocalMetadataPath = $"{DirectoryUtility.GetAvatarSaveDirectory(GUID)}{GUID}{TestUtils.JSON_SUFFIX}"
        };

        [Test]
        public async Task Process_Avatar_Url()
        {
            AvatarUri avatarUri;
            var dir = DirectoryUtility.GetAvatarSaveDirectory(GUID, false, RANDOM_PARAM_HASH);
            var jsonDir = DirectoryUtility.GetAvatarSaveDirectory(GUID);

            var urlProcessor = new UrlProcessor();
            try
            {
                avatarUri = await urlProcessor.ProcessUrl(GUID_URL, RANDOM_PARAM_HASH);
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.AreEqual(expectedUri.Guid, avatarUri.Guid);
            Assert.AreEqual(expectedUri.ModelUrl, avatarUri.ModelUrl);
            Assert.AreEqual(expectedUri.MetadataUrl, avatarUri.MetadataUrl);
            Assert.AreEqual($"{dir}/{avatarUri.Guid}{TestUtils.GLB_SUFFIX}", avatarUri.LocalModelPath);
            Assert.AreEqual($"{jsonDir}{avatarUri.Guid}{TestUtils.JSON_SUFFIX}", avatarUri.LocalMetadataPath);
        }

        [Test]
        public async Task Process_Avatar_Short_Code()
        {
            AvatarUri avatarUri;
            var dir = DirectoryUtility.GetAvatarSaveDirectory(SHORT_CODE, false, RANDOM_PARAM_HASH);
            var jsonDir = DirectoryUtility.GetAvatarSaveDirectory(SHORT_CODE);

            var urlProcessor = new UrlProcessor();
            try
            {
                avatarUri = await urlProcessor.ProcessUrl(SHORT_CODE, RANDOM_PARAM_HASH);
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.AreEqual(expectedShortcodeUri.Guid, avatarUri.Guid);
            Assert.AreEqual(expectedShortcodeUri.ModelUrl, avatarUri.ModelUrl);
            Assert.AreEqual(expectedShortcodeUri.MetadataUrl, avatarUri.MetadataUrl);
            Assert.AreEqual($"{dir}/{avatarUri.Guid}{TestUtils.GLB_SUFFIX}", avatarUri.LocalModelPath);
            Assert.AreEqual($"{jsonDir}{avatarUri.Guid}{TestUtils.JSON_SUFFIX}", avatarUri.LocalMetadataPath);
        }

        [Test]
        public async Task Process_Avatar_Short_Code_Url()
        {
            AvatarUri avatarUri;
            var dir = DirectoryUtility.GetAvatarSaveDirectory(SHORT_CODE, false, RANDOM_PARAM_HASH);
            var jsonDir = DirectoryUtility.GetAvatarSaveDirectory(SHORT_CODE);

            var urlProcessor = new UrlProcessor();
            try
            {
                avatarUri = await urlProcessor.ProcessUrl(SHORT_CODE_URL, RANDOM_PARAM_HASH);
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.AreEqual(expectedShortcodeUri.Guid, avatarUri.Guid);
            Assert.AreEqual(expectedShortcodeUri.ModelUrl, avatarUri.ModelUrl);
            Assert.AreEqual(expectedShortcodeUri.MetadataUrl, avatarUri.MetadataUrl);
            Assert.AreEqual($"{dir}/{avatarUri.Guid}{TestUtils.GLB_SUFFIX}", avatarUri.LocalModelPath);
            Assert.AreEqual($"{jsonDir}{avatarUri.Guid}{TestUtils.JSON_SUFFIX}", avatarUri.LocalMetadataPath);
        }
    }
}
