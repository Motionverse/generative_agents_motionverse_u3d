using System.ComponentModel;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This enumeration describes the body type of the avatar.
    /// </summary>
    public enum BodyType
    {
        None,
        [Description("fullbody")] FullBody,
        [Description("halfbody")] HalfBody
    }

    /// <summary>
    /// This enumeration describes the avatars OutfitGender which is used for setting the correct skeleton.
    /// </summary>
    public enum OutfitGender
    {
        None,
        [Description("masculine")] Masculine,
        [Description("feminine")] Feminine,
        [Description("neutral")] Neutral
    }

    /// <summary>
    /// This enumeration describes the type of mesh.
    /// </summary>
    public enum MeshType
    {
        HeadMesh,
        BeardMesh,
        TeethMesh
    }

    /// <summary>
    /// This enumeration describes the different render scene options.
    /// </summary>
    public enum AvatarRenderScene
    {
        [Description("Upper body render")] FullbodyPortrait,
        [Description("Upper body render")] HalfbodyPortrait,
        [Description("Upper body render with transparent background")] FullbodyPortraitTransparent,
        [Description("Upper body render with transparent background")] HalfbodyPortraitTransparent,
        [Description("Posed full body render with transparent background")] FullBodyPostureTransparent
    }

    /// <summary>
    /// This enumeration describes the pose options for the avatar skeleton.
    /// </summary>
    public enum Pose
    {
        APose,
        TPose
    }

    /// <summary>
    /// This enumeration describes the avatar mesh LOD (Level of Detail) options.
    /// </summary>
    public enum MeshLod
    {
        [InspectorName("High (LOD0)")]
        High,
        [InspectorName("Medium (LOD1)")]
        Medium,
        [InspectorName("Low (LOD2)")]
        Low
    }

    /// <summary>
    /// This enumeration describes the TextureAtlas setting options.
    /// </summary>
    /// <remarks>If set to <c>None</c> the avatar meshes, materials and textures will NOT be combined.</remarks>
    public enum TextureAtlas
    {
        None,
        [InspectorName("High (1024)")]
        High,
        [InspectorName("Medium (512)")]
        Medium,
        [InspectorName("Low (256)")]
        Low
    }

    public enum TextureChannel
    {
        BaseColor,
        Normal,
        MetallicRoughness,
        Emissive,
        Occlusion
    }

    /// <summary>
    /// This enumeration describes the different types of failures.
    /// </summary>
    public enum FailureType
    {
        None,
        NoInternetConnection,
        UrlProcessError,
        ShortCodeError,
        DownloadError,
        MetadataDownloadError,
        MetadataParseError,
        ModelDownloadError,
        ModelImportError,
        DirectoryAccessError,
        AvatarProcessError,
        AvatarRenderError,
        OperationCancelled
    }

}
