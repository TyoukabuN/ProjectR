using PJR;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine.Profiling;
using System;
using static UnityEngine.Rendering.PostProcessing.PostProcessResources;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using static CShaderPerformanceTest;

public class CShaderPerformanceTest : MonoBehaviour
{
    public enum EComputeApproach
    { 
        CPU,
        GPU,
        CPU_Job,
    }
    public bool isDebug = false;

    public SkinnedMeshRenderer skinnedMeshRenderer;

    [LabelText("计算方式")]
    public EComputeApproach computeApproach = EComputeApproach.CPU;

    [BoxGroup("GPU", VisibleIf = "@computeApproach == EComputeApproach.GPU")]
    public ComputeShader computeShader;

    [BoxGroup("Job", VisibleIf = "@computeApproach == EComputeApproach.CPU_Job")]
    public int innerloopBatchCount = 10;

    [InlineButton("FindClosetPoint", "找Mesh最近点")]
    public Transform checkPoint;

    private void Awake()
    {
        Setup();
    }

    Vector3[] vertices = null;
    int[] triangleIdx = null;
    public void FindClosetPoint()
    {
        if (checkPoint == null)
            return;
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
            return;

        if (computeApproach == EComputeApproach.CPU)
        {
            vertices ??= skinnedMeshRenderer.sharedMesh.vertices;
            triangleIdx ??= skinnedMeshRenderer.sharedMesh.triangles;

            var closestPoint = SimplifiedSkinnedMeshHolder.FindClosestPointOnMesh(vertices, triangleIdx, checkPoint.position, skinnedMeshRenderer.localToWorldMatrix);
            if (isDebug)
                Debug.DrawLine(checkPoint.position, closestPoint, Color.red, 1f);
        }
        else if (computeApproach == EComputeApproach.GPU)
        {
            if (this.triangles == null || CBuffer_tri == null || CBuffer_triClosestP == null)
                Setup();

            var checkPoint = this.checkPoint.position;

            computeShader.SetVector("checkPoint", new Vector4(checkPoint.x, checkPoint.y, checkPoint.z, 0));
            computeShader.SetMatrix("local2world", skinnedMeshRenderer.localToWorldMatrix);

            computeShader.Dispatch(0, triangles.Length, 1, 1);

            CBuffer_triClosestP.GetData(triClosestPoints);

            Vector3 p = Vector3.zero;
            float closestDistanceSqr = Mathf.Infinity;

            for (int i = 0; i < triangles.Length; i++)
            {
                float distanceSqr = (checkPoint - triClosestPoints[i]).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    p = triClosestPoints[i];
                }
            }

            if (isDebug)
                Debug.DrawLine(checkPoint, p, Color.yellow, 1f);
        }
        else if (computeApproach == EComputeApproach.CPU_Job)
        {
            if(!result.IsCreated)
                result = new NativeArray<float3>(triangles.Length, Allocator.TempJob);
            if (!nTriangles.IsCreated)
            { 
                nTriangles = new NativeArray<Triangle>(triangles.Length, Allocator.TempJob);
                nTriangles.CopyFrom(triangles);
            }
            

            var job = new ClosestPointOnTriangleJob() {
                point = checkPoint.position,
                triangles = nTriangles,
                triClosestPoints = result,
            };
            var jobHandle = job.Schedule(triangles.Length, innerloopBatchCount);
            jobHandle.Complete();

            Vector3 p = Vector3.zero;
            float closestDistanceSqr = Mathf.Infinity;
            float3 checkP = new float3(checkPoint.position.x, checkPoint.position.y, checkPoint.position.z);
            for (int i = 0; i < result.Length; i++)
            {
                float distanceSqr = math.lengthsq(checkP - result[i]);

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    p = result[i];
                }
            }

            if (result.IsCreated)
                result.Dispose();


            Debug.DrawLine(checkP, p, Color.green, 1f);
        }
    }

    private NativeArray<float3> result;
    private NativeArray<Triangle> nTriangles;

    Vector3[] closestPoints;

    [NonSerialized]
    ComputeBuffer CBuffer_tri;
    [NonSerialized]
    ComputeBuffer CBuffer_triClosestP;
    public void Setup()
    {
        if (computeApproach == EComputeApproach.CPU)
        {
        }
        else if (computeApproach == EComputeApproach.GPU)
        {
            CBuffer_tri?.Release();
            CBuffer_triClosestP?.Release();

            var triangles = FillTriangle(skinnedMeshRenderer.sharedMesh);

            CBuffer_tri = new ComputeBuffer(triangles.Length, Marshal.SizeOf<Triangle>());
            CBuffer_triClosestP = new ComputeBuffer(triangles.Length, Marshal.SizeOf<Vector3>());

            CBuffer_tri.SetData(triangles);
            triClosestPoints = new Vector3[triangles.Length];
            CBuffer_triClosestP.GetData(triClosestPoints);

            computeShader.SetBuffer(0, "triangles", CBuffer_tri);
            computeShader.SetBuffer(0, "triClosestPoints", CBuffer_triClosestP);
            computeShader.SetVector("globalWidth", new Vector4(0, triangles.Length, 0, 0));
            computeShader.SetInt("taskAmount", triangles.Length);
        }
        else if (computeApproach == EComputeApproach.CPU_Job)
        {
            FillTriangle(skinnedMeshRenderer.sharedMesh,skinnedMeshRenderer.localToWorldMatrix);
        }
    }

    private void Update()
    {
        Profiler.BeginSample("FindClosetPoint");
        FindClosetPoint();
        Profiler.EndSample();
    }

    [Button]
    public void Test()
    {
        var mesh = skinnedMeshRenderer.sharedMesh;
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var meshToWorldMatrix = skinnedMeshRenderer.localToWorldMatrix;

        int i = 0;
        Vector3 v0 = meshToWorldMatrix.MultiplyPoint3x4(vertices[triangles[i]]);
        Vector3 v1 = meshToWorldMatrix.MultiplyPoint3x4(vertices[triangles[i + 1]]);
        Vector3 v2 = meshToWorldMatrix.MultiplyPoint3x4(vertices[triangles[i + 2]]);

        Vector3 checkPoint = this.checkPoint != null ? this.checkPoint.transform.position : Vector3.zero;

        //var point = SimplifiedSkinnedMeshHolder.ClosestPointOnTriangle(checkPoint, v0, v1, v2);

        var point = SimplifiedSkinnedMeshHolder.FindClosestPointOnMesh(skinnedMeshRenderer.sharedMesh, checkPoint, skinnedMeshRenderer.localToWorldMatrix);

        Debug.DrawLine(checkPoint, point, Color.red, 1f);
        Debug.Log(point);
    }

    Vector3[] triClosestPoints;
    public struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
    }
    Triangle[] triangles = null;
    Triangle[] FillTriangle(Mesh mesh)
    {
        triangles = new Triangle[mesh.triangles.Length / 3];
        for (int i = 0,j = 0; i < mesh.triangles.Length; i += 3, j++)
        {
            triangles[j].a = mesh.vertices[mesh.triangles[i]];
            triangles[j].b = mesh.vertices[mesh.triangles[i + 1]];
            triangles[j].c = mesh.vertices[mesh.triangles[i + 2]];
        }
        return triangles;
    }
    Triangle[] FillTriangle(Mesh mesh, Matrix4x4 local2World)
    {
        triangles = new Triangle[mesh.triangles.Length / 3];
        for (int i = 0,j = 0; i < mesh.triangles.Length; i += 3, j++)
        {
            triangles[j].a = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i]]);
            triangles[j].b = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i + 1]]);
            triangles[j].c = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i + 2]]);
        }
        return triangles;
    }

    [Button]
    public void Test2()
    {
        var mesh = skinnedMeshRenderer.sharedMesh;
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var meshToWorldMatrix = skinnedMeshRenderer.localToWorldMatrix;

        ComputeBuffer triBuffer = new ComputeBuffer(triangles.Length, Marshal.SizeOf<Triangle>());
        ComputeBuffer closestPoint = new ComputeBuffer(triangles.Length, Marshal.SizeOf<Vector3>());
        Vector4 globalWidth = new Vector4(0, triangles.Length, 0, 0);
        int taskAmount = triangles.Length;
        Vector3 checkPoint = this.checkPoint != null? this.checkPoint.transform.position : Vector3.zero;

        var tri = FillTriangle(mesh);
        triBuffer.SetData(tri);

        Vector3[] res = new Vector3[tri.Length];
        closestPoint.SetData(res);

        computeShader.SetBuffer(0, "triangles", triBuffer);
        computeShader.SetBuffer(0, "closestPoint", closestPoint);
        computeShader.SetVector("globalWidth", globalWidth);
        computeShader.SetInt("taskAmount", taskAmount);
        computeShader.SetVector("checkPoint", new Vector4(checkPoint.x, checkPoint.y, checkPoint.z,0));
        computeShader.SetMatrix("local2world", meshToWorldMatrix);

        computeShader.Dispatch(0, triangles.Length, 1, 1);
        closestPoint.GetData(res);

        Vector3 p = Vector3.zero;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < taskAmount; i ++)
        {
            float distanceSqr = (checkPoint - res[i]).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                p = res[i];
            }
        }

        Debug.DrawLine(checkPoint, p, Color.yellow, 1f);
        Debug.Log(p);
    }


    #region Job
    [BurstCompile]
    public struct ClosestPointOnTriangleJob : IJobParallelFor
    {
        public Vector3 point;
        public NativeArray<Triangle> triangles;

        public NativeArray<float3> triClosestPoints;
        public void Execute(int index)
        {
            var v0 = triangles[index].a;
            var v1 = triangles[index].b;
            var v2 = triangles[index].c;
            triClosestPoints[index] = SimplifiedSkinnedMeshHolder.ClosestPointOnTriangle(point, v0, v1, v2);
        }
    }
    #endregion
}
