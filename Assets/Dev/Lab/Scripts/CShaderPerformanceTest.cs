using PJR;
using PJR.Profile;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CShaderPerformanceTest : MonoBehaviour
{
    public enum EComputeApproach
    { 
        CPU,
        GPU,
    }

    public SkinnedMeshRenderer skinnedMeshRenderer;

    [LabelText("计算方式")]
    public EComputeApproach computeApproach = EComputeApproach.CPU;

    [ShowIf("@computeApproach == EComputeApproach.GPU")]
    public ComputeShader computeShader;

    [InlineButton("FindClosetPoint", "找Mesh最近点")]
    public Transform checkPoint;
    public void FindClosetPoint()
    {
        if (checkPoint == null)
            return;
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
            return;

        Setup();

        if (computeApproach == EComputeApproach.CPU)
        {
            var closestPoint = SimplifiedSkinnedMeshHolder.FindClosestPointOnMesh(skinnedMeshRenderer.sharedMesh, checkPoint.position, skinnedMeshRenderer.localToWorldMatrix);
            Debug.DrawLine(checkPoint.position, closestPoint, Color.red, 1f);
        }
        else if (computeApproach == EComputeApproach.GPU)
        {

        }
    }

    public void Setup()
    {
        if (computeApproach == EComputeApproach.CPU)
        {
        }
        else if (computeApproach == EComputeApproach.GPU)
        { 
        }
    }

    private void Update()
    {
        Profiler.BeginSample("FindClosetPoint");
        FindClosetPoint();
        Profiler.EndSample();
    }
}
