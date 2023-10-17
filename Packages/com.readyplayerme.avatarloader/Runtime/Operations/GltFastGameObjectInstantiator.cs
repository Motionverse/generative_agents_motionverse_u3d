using GLTFast;
using GLTFast.Logging;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible instantiating Avatar model as a GameObject.
    /// </summary>
    public class GltFastGameObjectInstantiator : GameObjectInstantiator
    {
        public GltFastGameObjectInstantiator(
            IGltfReadable gltf,
            Transform parent,
            ICodeLogger logger = null,
            InstantiationSettings settings = null
        )
            : base(gltf, parent, logger, settings)
        {
        }

        /// <inheritdoc />
        public override void AddPrimitive(
            uint nodeIndex,
            string meshName,
            Mesh mesh,
            int[] materialIndices,
            uint[] joints = null,
            uint? rootJoint = null,
            float[] morphTargetWeights = null,
            int primitiveNumeration = 0
        )
        {
            if ((m_Settings.Mask & ComponentType.Mesh) == 0)
            {
                return;
            }

            GameObject meshGo;
            if (primitiveNumeration == 0)
            {
                // Use Node GameObject for first Primitive
                meshGo = m_Nodes[nodeIndex];
                // Ready Player Me - Parent mesh to Avatar root game object
                meshGo.transform.SetParent(m_Parent.transform);
            }
            else
            {
                meshGo = new GameObject(meshName);
                meshGo.transform.SetParent(m_Nodes[nodeIndex].transform, false);
                meshGo.layer = m_Settings.Layer;
            }

            Renderer renderer;

            var hasMorphTargets = mesh.blendShapeCount > 0;
            if (joints == null && !hasMorphTargets)
            {
                var meshFilter = meshGo.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var meshRenderer = meshGo.AddComponent<MeshRenderer>();
                renderer = meshRenderer;
            }
            else
            {
                var skinnedMeshRenderer = meshGo.AddComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.updateWhenOffscreen = m_Settings.SkinUpdateWhenOffscreen;
                if (joints != null)
                {
                    var bones = new Transform[joints.Length];
                    for (var j = 0; j < bones.Length; j++)
                    {
                        var jointIndex = joints[j];
                        bones[j] = m_Nodes[jointIndex].transform;
                    }
                    skinnedMeshRenderer.bones = bones;
                    if (rootJoint.HasValue)
                    {
                        skinnedMeshRenderer.rootBone = m_Nodes[rootJoint.Value].transform;
                    }
                }
                skinnedMeshRenderer.sharedMesh = mesh;
                if (morphTargetWeights != null)
                {
                    for (var i = 0; i < morphTargetWeights.Length; i++)
                    {
                        var weight = morphTargetWeights[i];
                        skinnedMeshRenderer.SetBlendShapeWeight(i, weight);
                    }
                }
                renderer = skinnedMeshRenderer;
            }

            var materials = new Material[materialIndices.Length];
            for (var index = 0; index < materials.Length; index++)
            {
                Material material = m_Gltf.GetMaterial(materialIndices[index]) ?? m_Gltf.GetDefaultMaterial();
                materials[index] = material;
            }

            renderer.sharedMaterials = materials;
        }
    }
}
