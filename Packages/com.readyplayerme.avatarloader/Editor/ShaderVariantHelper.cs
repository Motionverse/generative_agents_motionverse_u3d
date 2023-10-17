using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public enum RenderPipeline { Standard, URP, HDRP }
    public static class ShaderVariantHelper
    {
        private const string TAG = nameof(ShaderVariantHelper);
        private const string PRELOADED_SHADER_PROPERTY = "m_PreloadedShaders";
        private const string GRAPHICS_SETTING_PATH = "ProjectSettings/GraphicsSettings.asset";

#if DISABLE_AUTO_INSTALLER
        private const string SHADER_VARIANT_FOLDER = "Assets/Ready Player Me/Avatar Loader/Shaders";
#else
    private const string SHADER_VARIANT_FOLDER = "Packages/com.readyplayerme.avatarloader/Shaders";
#endif

        private const string SHADER_VARIANTS_STANDARD = "glTFastShaderVariants";
        private const string SHADER_VARIANTS_URP = "glTFastShaderVariantsURP";
        private const string SHADER_VARIANTS_HDRP = "glTFastShaderVariantsHDRP";

        private const string HDRP_TYPE_NAME = "HDRenderPipelineAsset";
        private const string URP_TYPE_NAME = "UniversalRenderPipelineAsset";
        private const string SHADER_SESSION_CHECK = "SHADER_SESSION_CHECK";
        private const string VARIANTS_FOUND_LOG = "glTFast shader variants found in Graphics Settings->Preloaded Shaders";
        private const string SHADER_VARIANTS_EXTENSION = ".shadervariants";

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (SessionState.GetBool(SHADER_SESSION_CHECK, false)) return;
            SessionState.SetBool(SHADER_SESSION_CHECK, true);

            EditorApplication.update += CheckAndUpdatePreloadShaders;
        }

        private static void CheckAndUpdatePreloadShaders()
        {
            EditorApplication.update -= CheckAndUpdatePreloadShaders;
            AddPreloadShaderVariants(true);
        }

        public static void AddPreloadShaderVariants(bool checkForMissingVariants = false)
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            SerializedProperty shaderPreloadArray = serializedGraphicsObject.FindProperty(PRELOADED_SHADER_PROPERTY);
            AssetDatabase.Refresh();

            var newArrayIndex = shaderPreloadArray.arraySize;
            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(GetTargetShaderPath());
            if (checkForMissingVariants)
            {
                var shadersMissing = true;
                var serializedVariants = new SerializedObject(shaderVariants);

                foreach (SerializedProperty shaderInclude in shaderPreloadArray)
                {
                    if (shaderInclude.objectReferenceValue.name == serializedVariants.targetObject.name)
                    {
                        SDKLogger.Log(TAG, VARIANTS_FOUND_LOG);
                        shadersMissing = false;
                        break;
                    }
                }
                if (!shadersMissing) return;
            }


            shaderPreloadArray.InsertArrayElementAtIndex(newArrayIndex);
            SerializedProperty shaderInArray = shaderPreloadArray.GetArrayElementAtIndex(newArrayIndex);
            shaderInArray.objectReferenceValue = shaderVariants;

            serializedGraphicsObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }


        public static bool IsMissingVariants()
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);

            SerializedProperty shaderPreloadArray = serializedGraphicsObject.FindProperty(PRELOADED_SHADER_PROPERTY);

            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(GetTargetShaderPath());
            var shadersMissing = true;
            var serializedVariants = new SerializedObject(shaderVariants);

            foreach (SerializedProperty shaderInclude in shaderPreloadArray)
            {
                if (shaderInclude.objectReferenceValue.name == serializedVariants.targetObject.name)
                {
                    SDKLogger.Log(TAG, VARIANTS_FOUND_LOG);

                    shadersMissing = false;
                    break;
                }
            }
            return shadersMissing;
        }

        private static string GetTargetShaderPath()
        {
            switch (GetCurrentRenderPipeline())
            {
                case RenderPipeline.URP:
                    return $"{SHADER_VARIANT_FOLDER}/{SHADER_VARIANTS_URP}{SHADER_VARIANTS_EXTENSION}";
                case RenderPipeline.HDRP:
                    return $"{SHADER_VARIANT_FOLDER}/{SHADER_VARIANTS_HDRP}{SHADER_VARIANTS_EXTENSION}";
                default:
                    return $"{SHADER_VARIANT_FOLDER}/{SHADER_VARIANTS_STANDARD}{SHADER_VARIANTS_EXTENSION}";
            }
        }

        private static RenderPipeline GetCurrentRenderPipeline()
        {
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                var renderPipelineType = GraphicsSettings.renderPipelineAsset.GetType().ToString();
                if (renderPipelineType.Contains(HDRP_TYPE_NAME))
                {
                    return RenderPipeline.HDRP;
                }
                if (renderPipelineType.Contains(URP_TYPE_NAME))
                {
                    return RenderPipeline.URP;
                }
            }
            return RenderPipeline.Standard;
        }
    }
}
