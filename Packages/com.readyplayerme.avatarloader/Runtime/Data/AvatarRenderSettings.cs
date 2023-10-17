using System.Collections.Generic;
using System.Text;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This structure holds all the data required a request to the Avatar Render API.
    /// </summary>
    public struct AvatarRenderSettings
    {
        public string Model;
        public AvatarRenderScene Scene;
        public string[] BlendShapeMeshes;
        public Dictionary<string, float> BlendShapes;

        public string GetParametersAsString()
        {
            BlendShapes ??= new Dictionary<string, float>();
            var queryBuilder = new QueryBuilder();
            queryBuilder.AddKeyValue(AvatarAPIParameters.RENDER_SCENE, Scene.GetSceneNameAsString());
            foreach (KeyValuePair<string, float> blendShape in BlendShapes)
            {
                foreach (var blendShapeMesh in BlendShapeMeshes)
                {
                    queryBuilder.AddKeyValue($"{AvatarAPIParameters.RENDER_BLEND_SHAPES}[{blendShapeMesh}][{blendShape.Key}]", blendShape.Value.ToString());
                }
            }
            
            return queryBuilder.Query;
        }
    }
}
