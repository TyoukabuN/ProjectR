using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using System.IO;
using PJR.ClassExtension;

namespace PJR
{
    public partial class SimplifiedSkinnedMeshHolder : SerializedMonoBehaviour
    {
        public List<SkinnedMeshRenderer> skinnedMeshRenderers;
        public Transform rootBone;

        [NonSerialized]
        private Dictionary<SkinnedMeshRenderer, Mesh> renderer2Handle;
        [SerializeField, DisableIf("@true")]
        private List<SimplifiedSkinnedMeshHandle> handles;
        
        public bool Valid
        {
            get {
                if (handles == null || handles.Count <= 0)
                    return false;
                return true;
            }
        }

        public bool TryGetClosestPoint(Vector3 checkPoint,out ClosestPointInfo info)
        {
            info = default;
            if (!Valid)
                return false;
            handles[0].GetClosestPoint(checkPoint,out info);
            return true;
        }

#if UNITY_EDITOR
        public const string Simplified_Suffix = "Simplified";
        //[NonSerialized]
        [ShowInInspector]
        [LabelText("Renderer根节点"), BoxGroup("Editor", VisibleIf = "@!EditorApplication.isPlaying"),HorizontalGroup("Editor/Collect")]
        private Transform RendererRoot;

        [Button("生成简化Mesh并Setup"), HorizontalGroup("Editor/Collect")]
        void Editor_CollectSkinnedMeshRenderer()
        {
            if (RendererRoot == null)
            {
                Debug.LogWarning("你没有填Renderer根节点!!");
                return;
            }

            if (renderer2Handle != null)
                foreach (var pair in renderer2Handle)
                    DestroyImmediate(pair.Key.gameObject);

            foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            { 
                if(IsSimplifiedObject(renderer.gameObject))
                    DestroyImmediate(renderer.gameObject);
            }

            var renderers = RendererRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderers = renderers.ToList();

            if (skinnedMeshRenderers == null || skinnedMeshRenderers.Count <= 0)
                return;


            renderer2Handle = new Dictionary<SkinnedMeshRenderer, Mesh>();
            handles = new List<SimplifiedSkinnedMeshHandle>();

            string assetPath = string.Empty;
            Dictionary<SkinnedMeshRenderer, string> originalRenderer2SimplifiedMeshAssetPath = new Dictionary<SkinnedMeshRenderer, string>();


            AssetDatabase.StartAssetEditing();
            try {
                //Gen Simplified Mesh Asset
                for (int i = 0; i < skinnedMeshRenderers.Count; i++)
                {
                    var renderer = skinnedMeshRenderers[i];
                    if (renderer.gameObject.name.EndsWith(Simplified_Suffix))
                        continue;
                    var mesh = renderer?.sharedMesh;
                    if (Editor_GenerateSimplifiedMeshAndSave(mesh, MeshSimplifyQuality, out assetPath, true))
                    {
                        originalRenderer2SimplifiedMeshAssetPath[renderer] = assetPath;
                    }
                }
            }
            finally {
                AssetDatabase.StopAssetEditing();
            }
            AssetDatabase.Refresh();

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(assetPath)) 
                { 
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    if (asset != null)
                        EditorGUIUtility.PingObject(asset);
                }
                Editor_SetupSimplifiedSkinnedMeshRenderer(originalRenderer2SimplifiedMeshAssetPath);
            };
        }

        bool IsSimplifiedObject(UnityEngine.Object obj)
        {
            if (obj == null)
                return false;
            return obj.name.EndsWith(Simplified_Suffix);
        }
        
        void Editor_SetupSimplifiedSkinnedMeshRenderer(Dictionary<SkinnedMeshRenderer, string> originalRenderer2SimplifiedMeshAssetPath)
        {
            if (originalRenderer2SimplifiedMeshAssetPath == null || originalRenderer2SimplifiedMeshAssetPath.Count <= 0)
                return;
            handles = new List<SimplifiedSkinnedMeshHandle>();

            foreach (var pair in originalRenderer2SimplifiedMeshAssetPath)
            {
                var renderer = pair.Key;
                var meshAssetPath = pair.Value;
                if (renderer == null || string.IsNullOrEmpty(meshAssetPath))
                    continue;
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshAssetPath);
                if (mesh == null)
                    continue;
                var handle = Editor_CreateHandle(renderer, mesh);
                handles.Add(handle);
            }
        }
        SimplifiedSkinnedMeshHandle Editor_CreateHandle(SkinnedMeshRenderer original_renderer, Mesh mesh)
        {
            var gobj = new GameObject();
            gobj.transform.parent = gameObject.transform;
            gobj.transform.ResetValue();
            gobj.name = mesh.name;
            //
            var renderer = gobj.AddComponent<SkinnedMeshRenderer>();

            renderer.material = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset?.defaultMaterial;
            renderer.sharedMesh = mesh;
            renderer.rootBone = original_renderer.rootBone;
            renderer.bones = original_renderer.bones;
            renderer.enabled = false;

            return new SimplifiedSkinnedMeshHandle(rootBone, original_renderer, renderer);
        }

        [Button("生成减面Mesh并保存为Asset")]
        void Editor_GenerateSimplifiedMeshAndSave_Btn()
        {
            if (skinnedMeshRenderers == null || skinnedMeshRenderers.Count <= 0)
                return;
            for (int i = 0; i < skinnedMeshRenderers.Count; i++)
                Editor_GenerateSimplifiedMeshAndSave(skinnedMeshRenderers[i]?.sharedMesh, MeshSimplifyQuality);
        }
        public static bool Editor_GenerateSimplifiedMeshAndSave(Mesh mesh, float quality, bool returnIfExist = false) => Editor_GenerateSimplifiedMeshAndSave(mesh, quality, out var path, returnIfExist);
        public static bool Editor_GenerateSimplifiedMeshAndSave(Mesh mesh, float quality, out string savePath, bool returnIfExist = false)
        {
            savePath = string.Empty;
            if (!GetSimplifiedMesh(mesh, out var simplified, quality, false))
                return false;
            string path = AssetDatabase.GetAssetPath(mesh);
            if (string.IsNullOrEmpty(path))
                return false;
            string assetName = Path.GetFileNameWithoutExtension(path);
            if (AssetDatabase.IsSubAsset(mesh))
                assetName = $"{assetName}_{mesh.name}_{Simplified_Suffix}.mesh";

            savePath = Path.Combine(Path.GetDirectoryName(path), assetName);
            if (returnIfExist && AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(savePath) != null)
                return true;

            savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

            AssetDatabase.CreateAsset(simplified, savePath);
            Debug.Log($"生成{savePath}");
            return true;
        }


        [NonSerialized]//
        [ShowInInspector]
        [LabelText("检查点"), BoxGroup("Debug"), HorizontalGroup("Debug/Check")]
        private Transform CheckPoint;

        [Button("指向最近的点"), HorizontalGroup("Debug/Check")]
        void Editor_DisplayCheckPoint2ClosestPoint()
        {
            if (CheckPoint == null)
                return;
            foreach (var handle in handles)
            {
                handle.GetClosestPoint(CheckPoint.position);
            }
        }

        [NonSerialized]//
        [ShowInInspector]
        [LabelText("跟随物"), BoxGroup("Debug"), HorizontalGroup("Debug/Follow")]
        private Transform FollowObj;

        [Button("跟随MeshClosestPoint"), HorizontalGroup("Debug/Follow")]
        void Editor_FollowClosestPoint()
        {
            if (FollowObj == null)
                return;
            foreach (var handle in handles)
            {
                handle.GetClosestPoint(FollowObj.position,out var info);
                _followTest = new FollowTest(FollowObj, info);
            }
        }
        [Button("Clear"), HorizontalGroup("Debug/Follow")]
        void Editor_ClearFollowTest()
        {
            _followTest = null;
        }

        [NonSerialized]
        private FollowTest _followTest;
        public class FollowTest
        {
            public Transform follower;
            public ClosestPointInfo info;
            public FollowTest(Transform follower, ClosestPointInfo info)
            {
                this.follower = follower;
                this.info = info;
            }
            public void Update()
            {
                if (info.TryGetWSPoint(out var wpos))
                { 
                    follower.position = wpos;
                }
                if (info.TryGetWSNormal(out var wnormal))
                { 
                    follower.up = wnormal;
                }
            }
        }

        private void Update()
        {
            if (_followTest != null)
            {
                _followTest.Update();
            }
        }

#endif
    }


    [Serializable]
    public class SimplifiedSkinnedMeshHandle
    {
        public Transform rootBone;
        public SkinnedMeshRenderer originalRenderer;
        public SkinnedMeshRenderer renderer;
        public Mesh mesh => renderer.sharedMesh;
        public GameObject gameObject => renderer.gameObject;
        public Transform transform => renderer.transform;
        public SimplifiedSkinnedMeshHandle(Transform rootBone, SkinnedMeshRenderer originalRenderer, SkinnedMeshRenderer renderer)
        { 
            this.originalRenderer = originalRenderer;
            this.renderer = renderer;
            this.rootBone = rootBone;
        }

        private Mesh tempMesh;
        public Vector3 GetClosestPoint(Vector3 checkPoint) => GetClosestPoint(checkPoint, out var info);
        public Vector3 GetClosestPoint(Vector3 checkPoint,out ClosestPointInfo info)
        {
            info = default;
            if (checkPoint == null)
                return Vector3.zero;

            tempMesh ??= new Mesh();
            renderer.BakeMesh(tempMesh);
            tempMesh.boneWeights = renderer.sharedMesh.boneWeights;
            tempMesh.RecalculateNormals();

            //var closestPos = SimplifiedSkinnedMeshHolder.FindClosestPointOnMesh(tempMesh, checkPoint, renderer.localToWorldMatrix);
            //info = SimplifiedSkinnedMeshHolder.FindClosestPointOnMesh(renderer, tempMesh, checkPoint, rootBone.localToWorldMatrix);
            info = SimplifiedSkinnedMeshHolder.FindClosestPointOnMesh(renderer, tempMesh, checkPoint, renderer.transform.localToWorldMatrix);
            Debug.DrawLine(checkPoint, info.closestPoint, Color.red, 1f);

            if (info.TryGetWSNormal(out var wnormal))
            { 
                Debug.DrawLine(info.closestPoint, info.closestPoint + wnormal * 0.1f, Color.yellow, 1f);
            }
            UnityEngine.Profiling.Profiler.EndSample();

            return info.closestPoint;
        }
    }

    public struct ClosestPointInfo
    {
        public Vector3 closestPoint;
        public Transform rootBone;
        public int[] trangle;
        public Vector3[] transientLocalNormals;
        public BoneWeight[] boneWeight;
        public Transform[] bones;
        /// <summary>
        /// 记录获取瞬间closestPoint在影响对应三角形顶点的bone下本地坐标,用于后续帧还原closestPoint的当时的wpos
        /// </summary>
        public Vector3[] transientLocalPoint;
        public bool TryGetWSPoint(out Vector3 wpos) => TryGetWSPoint(0, out wpos);
        public bool TryGetWSPoint
            (int index, out Vector3 wpos)
        {
            wpos = Vector3.zero;
            var bone = bones[index];
            if (bone == null)
                return false;
            var localPos = transientLocalPoint[index];
            wpos = bone.TransformPoint(localPos);
            return true;
        }
        public bool TryGetWSNormal(out Vector3 wnormal) => TryGetWSNormal(0, out wnormal);
        public bool TryGetWSNormal(int index, out Vector3 wnormal)
        {
            wnormal = Vector3.zero;
            var bone = bones[index];
            if (bone == null)
                return false;
            var localNormal = transientLocalNormals[index];
            wnormal = bone.TransformVector(localNormal);
            return true;
        }
    }

    public partial class SimplifiedSkinnedMeshHolder
    {
        public static bool Enable = false;

        public static float MeshSimplifyQuality = 0.1f;
        private static Dictionary<int, Mesh> _originalMeshHash2SimplifiedMesh;
        public static Dictionary<int, Mesh> OriginalMeshHash2SimplifiedMesh => _originalMeshHash2SimplifiedMesh ??= new Dictionary<int, Mesh>();

    #if UNITY_EDITOR
        public static bool GetSimplifiedMesh(SkinnedMeshRenderer skinnedMeshRenderer, out Mesh mesh, bool cache = true)
        {
            mesh = null;
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
                return false;

            return GetSimplifiedMesh(skinnedMeshRenderer.sharedMesh, out mesh, cache);
        }
        public static bool GetSimplifiedMesh(Mesh orignal, out Mesh mesh, bool cache = true)
        {
            return GetSimplifiedMesh(orignal, out mesh, MeshSimplifyQuality, cache);
        }
        public static bool GetSimplifiedMesh(Mesh orignal, out Mesh mesh, float quality, bool cache = true)
        {
            mesh = null;
            var hash = orignal.GetHashCode();
            if (cache)
            { 
                if (OriginalMeshHash2SimplifiedMesh.TryGetValue(hash, out mesh) && mesh != null)
                    return true;
            }

            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(orignal);
            meshSimplifier.SimplifyMesh(quality);

            mesh = meshSimplifier.ToMesh();
            mesh.name = $"{orignal.name}_{Simplified_Suffix}";
            if (cache)
            { 
                OriginalMeshHash2SimplifiedMesh[hash] = mesh;
            }

            return true;
        }
    #endif
        public static ClosestPointInfo FindClosestPointOnMesh(SkinnedMeshRenderer skinnedMeshRenderer, Mesh mesh, Vector3 point, Matrix4x4 meshToWorldMatrix)
        {
            var bones = skinnedMeshRenderer.bones;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] normals = mesh.normals;

            Vector3 closestPoint = Vector3.zero;
            float closestDistanceSqr = Mathf.Infinity;

            ClosestPointInfo info = new ClosestPointInfo();
            info.rootBone = skinnedMeshRenderer.rootBone;
            info.trangle = new int[3];
            info.transientLocalNormals = new Vector3[3];
            info.boneWeight = new BoneWeight[3];
            info.bones = new Transform[3];
            info.transientLocalPoint = new Vector3[3];

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = skinnedMeshRenderer.transform.TransformPoint((vertices[triangles[i]]));
                Vector3 v1 = skinnedMeshRenderer.transform.TransformPoint((vertices[triangles[i + 1]]));
                Vector3 v2 = skinnedMeshRenderer.transform.TransformPoint((vertices[triangles[i + 2]]));

                Debug.DrawLine(v0, v1, Color.blue, 1f);

                if (!ClosestPointOnTriangle(point, v0, v1, v2, out var pointOnTriangle))
                    continue;
                float distanceSqr = (point - pointOnTriangle).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestPoint = pointOnTriangle;

                    info.closestPoint = closestPoint;

                    info.trangle[0] = triangles[i];
                    info.trangle[1] = triangles[i + 1];
                    info.trangle[2] = triangles[i + 2];

                    info.boneWeight[0] = mesh.boneWeights[info.trangle[0]];
                    info.boneWeight[1] = mesh.boneWeights[info.trangle[1]];
                    info.boneWeight[2] = mesh.boneWeights[info.trangle[2]];

                    info.bones[0] = bones[info.boneWeight[0].boneIndex0];
                    info.bones[1] = bones[info.boneWeight[1].boneIndex0];
                    info.bones[2] = bones[info.boneWeight[2].boneIndex0];

                    info.transientLocalPoint[0] = info.bones[0].InverseTransformPoint(closestPoint);
                    info.transientLocalPoint[1] = info.bones[1].InverseTransformPoint(closestPoint);
                    info.transientLocalPoint[2] = info.bones[2].InverseTransformPoint(closestPoint);

                    info.transientLocalNormals[0] = info.bones[0].InverseTransformVector(meshToWorldMatrix.MultiplyVector(normals[info.trangle[0]]));
                    info.transientLocalNormals[1] = info.bones[1].InverseTransformVector(meshToWorldMatrix.MultiplyVector(normals[info.trangle[1]]));
                    info.transientLocalNormals[2] = info.bones[2].InverseTransformVector(meshToWorldMatrix.MultiplyVector(normals[info.trangle[2]]));
                }
            }

            return info;
        }

        public static Vector3 FindClosestPointOnMesh(Mesh mesh, Vector3 point, Matrix4x4 meshToWorldMatrix) => FindClosestPointOnMesh(mesh.vertices, mesh.triangles, point, meshToWorldMatrix);
        public static Vector3 FindClosestPointOnMesh(Vector3[] vertices, int[] triangles, Vector3 point, Matrix4x4 meshToWorldMatrix)
        {
            Vector3 closestPoint = Vector3.zero;
            float closestDistanceSqr = Mathf.Infinity;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = meshToWorldMatrix.MultiplyPoint3x4(vertices[triangles[i]]);
                Vector3 v1 = meshToWorldMatrix.MultiplyPoint3x4(vertices[triangles[i + 1]]);
                Vector3 v2 = meshToWorldMatrix.MultiplyPoint3x4(vertices[triangles[i + 2]]);

                //Debug.DrawLine(v0, v1, Color.blue, 1f);

                Vector3 pointOnTriangle = ClosestPointOnTriangle(point, v0, v1, v2);
                float distanceSqr = (point - pointOnTriangle).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestPoint = pointOnTriangle;
                }
            }

            return closestPoint;
        }

        #region Math things
        public static Vector3 ClosestPointOnTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
        {
            // Compute vectors
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = point - a;

            // Compute dot products
            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            float d3 = Vector3.Dot(ab, ab);
            float d4 = Vector3.Dot(ac, ac);
            float d5 = Vector3.Dot(ab, ac);

            // Compute barycentric coordinates
            float denom = d3 * d4 - d5 * d5;
            float v = (d4 * d1 - d5 * d2) / denom;
            float w = (d3 * d2 - d5 * d1) / denom;

            if (v >= 0 && w >= 0 && v + w <= 1)
            {
                // Inside the triangle
                return a + v * ab + w * ac;
            }

            // Outside the triangle, project onto edges
            return ClosestPointOnLineSegment(point, a, b, c);
        }
        static bool ClosestPointOnTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c, out Vector3 res)
        {
            res = Vector3.zero;
            // Compute vectors
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = point - a;

            // Compute dot products
            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            float d3 = Vector3.Dot(ab, ab);
            float d4 = Vector3.Dot(ac, ac);
            float d5 = Vector3.Dot(ab, ac);

            // Compute barycentric coordinates
            float denom = d3 * d4 - d5 * d5;
            float v = (d4 * d1 - d5 * d2) / denom;
            float w = (d3 * d2 - d5 * d1) / denom;

            if (v >= 0 && w >= 0 && v + w <= 1)
            {
                // Inside the triangle
                res = a + v * ab + w * ac;
                return true;
            }

            // Outside the triangle, project onto edges
            res = ClosestPointOnLineSegment(point, a, b, c);
            return true;
        }
        static Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 closest = ClosestPointOnLine(point, a, b);
            float dist1 = (point - closest).sqrMagnitude;

            Vector3 closest2 = ClosestPointOnLine(point, a, c);
            float dist2 = (point - closest2).sqrMagnitude;

            Vector3 closest3 = ClosestPointOnLine(point, b, c);
            float dist3 = (point - closest3).sqrMagnitude;

            if (dist1 < dist2 && dist1 < dist3)
                return closest;

            return dist2 < dist3 ? closest2 : closest3;
        }
        static Vector3 ClosestPointOnLine(Vector3 point, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float t = Mathf.Clamp01(Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab));
            return a + t * ab;
        }
        #endregion
    }
}
