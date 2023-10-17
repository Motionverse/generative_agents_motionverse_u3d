using System.Collections.Generic;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This static class contains maps that can be used for avatar configuration.
    /// </summary>
    public static class AvatarConfigMap
    {
        /// A map of all possible
        /// <c>Pose</c>
        /// options
        public static readonly Dictionary<Pose, string> Pose = new Dictionary<Pose, string>
        {
            { AvatarLoader.Pose.APose, "A" },
            { AvatarLoader.Pose.TPose, "T" }
        };

        /// A map of all possible
        /// <c>TextureAtlas</c>
        /// settings
        public static readonly Dictionary<TextureAtlas, string> TextureAtlas = new Dictionary<TextureAtlas, string>
        {
            { AvatarLoader.TextureAtlas.None, "none" },
            { AvatarLoader.TextureAtlas.High, "1024" },
            { AvatarLoader.TextureAtlas.Medium, "512" },
            { AvatarLoader.TextureAtlas.Low, "256" }
        };
    }
}
