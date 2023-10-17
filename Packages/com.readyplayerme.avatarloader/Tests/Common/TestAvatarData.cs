namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public static class TestAvatarData
    {
        private const string FULLBODY_MASCULINE = "64184ac404207164c85216d6";
        private const string FULLBODY_FEMININE = "641975b2398f7e86e696913e";
        private const string HALFBODY_MASCULINE = "6419862204207164c854045b";
        private const string HALFBODY_FEMININE = "6419864b398f7e86e696aa77";

        public const string TEST_WRONG_GUID = "wrong-guid";

        public static readonly AvatarUri WrongUri = new AvatarUri
        {
            Guid = TEST_WRONG_GUID,
            ModelUrl = $"{TestUtils.API_URL_PREFIX}{TEST_WRONG_GUID}{TestUtils.GLB_SUFFIX}",
            LocalModelPath = $"{TestUtils.TestAvatarDirectory}/{TEST_WRONG_GUID}/{TEST_WRONG_GUID}{TestUtils.GLB_SUFFIX}",
            MetadataUrl = $"{TestUtils.API_URL_PREFIX}{TEST_WRONG_GUID}{TestUtils.JSON_SUFFIX}",
            LocalMetadataPath = $"{TestUtils.TestAvatarDirectory}/{TEST_WRONG_GUID}/{TEST_WRONG_GUID}{TestUtils.JSON_SUFFIX}"
        };

        public static readonly AvatarUri DefaultAvatarUri = new AvatarUri
        {
            Guid = FULLBODY_MASCULINE,
            ModelUrl = $"{TestUtils.API_URL_PREFIX}{FULLBODY_MASCULINE}{TestUtils.GLB_SUFFIX}",
            LocalModelPath = $"{TestUtils.TestAvatarDirectory}/{FULLBODY_MASCULINE}/{FULLBODY_MASCULINE}{TestUtils.GLB_SUFFIX}",
            MetadataUrl = $"{TestUtils.API_URL_PREFIX}{FULLBODY_MASCULINE}{TestUtils.JSON_SUFFIX}",
            LocalMetadataPath = $"{TestUtils.TestAvatarDirectory}/{FULLBODY_MASCULINE}/{FULLBODY_MASCULINE}{TestUtils.JSON_SUFFIX}"
        };

        public static string GetAvatarApiJsonUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetAvatarGuid(bodyType, outfitGender);
            return $"{TestUtils.API_URL_PREFIX}{avatarGuid}{TestUtils.JSON_SUFFIX}";
        }

        public static string GetAvatarModelsJsonUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetAvatarGuid(bodyType, outfitGender);
            return $"{TestUtils.MODELS_URL_PREFIX}{avatarGuid}{TestUtils.JSON_SUFFIX}";
        }

        public static string GetAvatarGuid(BodyType bodyType, OutfitGender outfitGender)
        {
            if (bodyType == BodyType.HalfBody)
            {
                return outfitGender == OutfitGender.Masculine ? HALFBODY_MASCULINE : HALFBODY_FEMININE;
            }
            return outfitGender == OutfitGender.Masculine ? FULLBODY_MASCULINE : FULLBODY_FEMININE;
        }
    }
}
