using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static JobSystemTest;

public class JobSystemTest : MonoBehaviour
{
    [LabelText("使用Job计算")]
    public bool UsingJobs = false;
    [LabelText("分阶段执行Job")]
    public bool SplitJobWorkingPeriod = true;

    [LabelText("Primitive用材质")]
    public Material PrimitiveMaterial;

    [LabelText("Primitive方阵大小")]
    public Vector2Int PrimitiveMatrixSize = new Vector2Int(16,16);

    private void Update()
    {
        if (primitiveMap != null && primitiveMap.Values.Count > 0)
        {
            if (UsingJobs) 
            { 
                JobOperate(true, !SplitJobWorkingPeriod);
            }
            else {
                foreach (var wrap in primitiveMap.Values)
                {
                    var pos = wrap.originPosition;
                    pos.y += math.sin(math.sqrt(math.dot(pos.xz, pos.xz)) + Time.time);

                    wrap.gameObject.transform.position = pos;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (primitiveMap != null && primitiveMap.Values.Count > 0)
        {
            if (UsingJobs)
            { 
                JobOperate(false, SplitJobWorkingPeriod);
            }
        }
    }

    [NonSerialized]
    private bool running = false;
    [Button]
    public void Run()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        running = !running;
        if (running)
            CreateObjects();
        else
            Clear();
    }

    public class PrimitiveWrap
    {
        public int index;
        public GameObject gameObject;
        public float3 originPosition;

        public MovementJob job;
        public JobHandle jobHandle;
        private NativeArray<float3> result;
        //private NativeParallelHashMap<int, float3> parallelResult;

        //public void CreateJob(NativeParallelHashMap<int, float3> _result)
        //{
        //    parallelResult = _result;
        //    job = new MovementJob()
        //    {
        //        index = index,
        //        pos = originPosition,
        //        time = Time.time,
        //        result = parallelResult,
        //    };
        //}

        public void CreateJob()
        {
            if(!result.IsCreated)
                result = new NativeArray<float3>(1, Allocator.TempJob);
            job = new MovementJob()
            {
                index = index,
                pos = originPosition,
                time = Time.time,
                result = result,
            };
        }
        public void CreateJob(NativeArray<float3> _result)
        {
            result = _result;
            job = new MovementJob()
            {
                index = index,
                pos = originPosition,
                time = Time.time,
                result = _result,
            };
        }
        public void ScheduleJob()
        {
            jobHandle = job.Schedule(jobHandle);
        }
        public void JobCompleteAndApplyResult()
        {
            jobHandle.Complete();
            //gameObject.transform.position = parallelResult[index];
            gameObject.transform.position = result[0];
            //result.Dispose();
        }
        public void Dispose()
        { 
            if(gameObject != null)
                DestroyImmediate(gameObject);
            if(result.IsCreated)
                result.Dispose();
        }
    }

    private Dictionary<int, PrimitiveWrap> primitiveMap;

    public void Clear()
    {
        if (primitiveMap == null)
            return;
        foreach (var pair in primitiveMap)
        {
            pair.Value.Dispose();
        }
        primitiveMap.Clear();
    }
    public void CreateObjects()
    {
        primitiveMap ??= new Dictionary<int, PrimitiveWrap>();

        Clear();

        int index = 0;

        for (int x = 0; x < PrimitiveMatrixSize.x; x++)
        {
            for (int z = 0; z < PrimitiveMatrixSize.y; z++)
            { 
                var gobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                if(PrimitiveMaterial != null)
                    gobj.GetComponent<MeshRenderer>().material = PrimitiveMaterial;

                var wrap = new PrimitiveWrap() {
                    index = index++,
                    gameObject = gobj,
                    originPosition = new Vector3(x, 0, z),
                };
                primitiveMap.Add(wrap.index, wrap);

                gobj.transform.position = wrap.originPosition;
            }
        }
    }


    private NativeParallelHashMap<int,float3> result;
    public void JobOperate() => JobOperate(true, true);
    public void JobOperate(bool createAndSchedule = true, bool completeAndApply = true)
    {
        //if (completeAndApply)
        //{
        //    if(!result.IsCreated)
        //        result = new NativeParallelHashMap<int, float3>(PrimitiveMatrixSize.x * PrimitiveMatrixSize.y, Allocator.Persistent);
        //}
        foreach (var wrap in primitiveMap.Values)
        {
            if (createAndSchedule)
            {
                wrap.CreateJob();
                wrap.ScheduleJob();
            }
            if (completeAndApply)
            { 
                wrap.JobCompleteAndApplyResult();
            }
        }
    }

    [BurstCompile]
    public struct MovementJob : IJob
    {
        public int index;   
        public float3 pos;
        public float time;

        public NativeArray<float3> result;
        //public NativeParallelHashMap<int, float3> result;
        public void Execute()
        {
            pos.y += math.sin(math.sqrt(math.dot(pos.xz, pos.xz)) + time);
            result[0] = pos;
            //result[index] = pos;
        }
    }
}
