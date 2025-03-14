using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using static CShaderPerformanceTest;
using static UnityEngine.Rendering.PostProcessing.PostProcessResources;

public class MeshClosestPointFindTest : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public ComputeShader computeShader;
    public Transform referTransform;


    public bool isDebug = false;

    [NonSerialized]
    ComputeBuffer triBuffer;
    [NonSerialized]
    ComputeBuffer triClosestPointBuf;
    [NonSerialized]
    ComputeBuffer closestPointBuf;
    [NonSerialized]
    ComputeBuffer debugBuf;

    public bool setuped = false;

    public void OnEnable()
    {
        Clearup();
        Setup();
    }

    public void Update()
    {
        if(!IsValid())
            return;
        FindPoint();
    }

    public void Register(bool enable)
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        if(enable)
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    public void OnPlayModeStateChanged(PlayModeStateChange playerModeStateChange)
    {
        if (playerModeStateChange == PlayModeStateChange.ExitingPlayMode)
            Clearup();
    }

    public const int GroudThreadsX = 1024;
    public void Setup()
    {
        var mesh = skinnedMeshRenderer.sharedMesh;
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var meshToWorldMatrix = skinnedMeshRenderer.localToWorldMatrix;

        triBuffer ??= new ComputeBuffer(triangles.Length, Marshal.SizeOf<Triangle>());
        triClosestPointBuf ??= new ComputeBuffer(triangles.Length, Marshal.SizeOf<Vector3>());
        closestPointBuf ??= new ComputeBuffer(GroudThreadsX, Marshal.SizeOf<Vector3>());
        debugBuf ??= new ComputeBuffer(triangles.Length, Marshal.SizeOf<Vector4>());
        //
        computeShader.SetBuffer(0, "triangles", triBuffer);
        computeShader.SetBuffer(0, "triClosestPoints", triClosestPointBuf);
        computeShader.SetBuffer(0, "closestPoint", closestPointBuf);
        computeShader.SetBuffer(0, "debugs", debugBuf);

        setuped = true;
    }
    public bool IsValid()
    {
        if (skinnedMeshRenderer.sharedMesh == null)
            return false;
        if (computeShader == null)
            return false;
        return false;
    }

    public int groudThreadsX = 16;
    public Vector3[] closestPoints;
    public static Vector3 Invalid = new Vector3(999999, 999999, 999999);
    public void FindPoint()
    {
        var mesh = skinnedMeshRenderer.sharedMesh;
        var triangles = mesh.triangles;
        var local2WorldMatrix = skinnedMeshRenderer.localToWorldMatrix;
        int taskAmount = triangles.Length;
        Vector3 checkPoint = this.referTransform != null ? this.referTransform.transform.position : Vector3.zero;

        //给Buffer填SetData
        var tri = FillTriangle(mesh);
        triBuffer.SetData(tri);
        var res = new Vector3[tri.Length];
        triClosestPointBuf.SetData(res);

        closestPoints = new Vector3[groudThreadsX];
        Array.Fill(closestPoints, Invalid);
        closestPointBuf.SetData(closestPoints);

        var debug = new Vector4[triangles.Length];
        debugBuf.SetData(debug);

        //


        computeShader.SetInt("taskAmount", taskAmount);
        computeShader.SetVector("checkPoint", new Vector4(checkPoint.x, checkPoint.y, checkPoint.z, 0));
        computeShader.SetMatrix("local2world", local2WorldMatrix);

        computeShader.Dispatch(0, Mathf.CeilToInt(triangles.Length / 1024.0f), 1, 1);

        triClosestPointBuf.GetData(res);
        Vector3 p = GetClosestPointReferTo(res, checkPoint);
        Debug.DrawLine(checkPoint + Vector3.up * 0.02f, p, Color.yellow, 1f);
        //Debug.Log(p);

        closestPointBuf.GetData(closestPoints);
        Vector3 p1 = GetClosestPointReferTo(closestPoints, checkPoint);
        Debug.DrawLine(checkPoint, p1, Color.red, 1f);
        //Debug.Log(p1);

        debugBuf.GetData(debug);
    }
    public Vector3 GetClosestPointReferTo(Vector3[] point, Vector3 referPoint)
    {
        Vector3 p = Vector3.zero;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < point.Length; i++)
        {
            float distanceSqr = (referPoint - point[i]).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                p = point[i];
            }
        }

        return p;
    }

    public void Clearup()
    {
        triBuffer.Dispose();
        triClosestPointBuf.Dispose();
        closestPointBuf.Dispose();
        debugBuf.Dispose();

        triBuffer = null;
        triClosestPointBuf = null;
        closestPointBuf = null;
        debugBuf = null;
    }


    Vector3[] triClosestPoints;
    public struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
    }
    Triangle[] trianglePoints = null;
    Triangle[] FillTriangle(Mesh mesh)
    {
        trianglePoints = new Triangle[mesh.triangles.Length / 3];
        for (int i = 0, j = 0; i < mesh.triangles.Length; i += 3, j++)
        {
            trianglePoints[j].a = mesh.vertices[mesh.triangles[i]];
            trianglePoints[j].b = mesh.vertices[mesh.triangles[i + 1]];
            trianglePoints[j].c = mesh.vertices[mesh.triangles[i + 2]];
        }
        return trianglePoints;
    }
    Triangle[] FillTriangle(Mesh mesh, Matrix4x4 local2World)
    {
        trianglePoints = new Triangle[mesh.triangles.Length / 3];
        for (int i = 0, j = 0; i < mesh.triangles.Length; i += 3, j++)
        {
            trianglePoints[j].a = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i]]);
            trianglePoints[j].b = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i + 1]]);
            trianglePoints[j].c = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i + 2]]);
        }
        return trianglePoints;
    }
}
