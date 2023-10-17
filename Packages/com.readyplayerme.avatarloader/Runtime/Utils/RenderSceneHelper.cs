using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader;

public static class RenderSceneHelper
{
    public static string GetSceneNameAsString(this AvatarRenderScene avatarRenderScene)
    {
        return RenderSceneMap[avatarRenderScene];
    }

    public static readonly Dictionary<AvatarRenderScene, string> RenderSceneMap = new Dictionary<AvatarRenderScene, string>
    {
        { AvatarRenderScene.FullbodyPortrait, "fullbody-portrait-v1" },
        { AvatarRenderScene.HalfbodyPortrait, "halfbody-portrait-v1" },
        { AvatarRenderScene.FullbodyPortraitTransparent, "fullbody-portrait-v1-transparent" },
        { AvatarRenderScene.HalfbodyPortraitTransparent, "halfbody-portrait-v1-transparent" },
        { AvatarRenderScene.FullBodyPostureTransparent, "fullbody-posture-v1-transparent" }
    };
}
