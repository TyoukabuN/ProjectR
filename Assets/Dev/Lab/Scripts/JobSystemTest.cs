using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using System;
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
    [LabelText("使用ParallelJob"), ShowIf("@UsingJobs")]
    public bool UsingParallelJob = false;
    [ShowIf("@UsingParallelJob")]
    public int InnerloopBatchCount = 1;
    [LabelText("分阶段执行Job"), ShowIf("@UsingJobs")]
    public bool SplitJobWorkingPeriod = true;


    [LabelText("Primitive类型")]
    public PrimitiveType PrimitiveType;
    [LabelText("Primitive用材质")]
    public Material PrimitiveMaterial;

    [LabelText("Primitive方阵大小")]
    public Vector2Int PrimitiveMatrixSize = new Vector2Int(16,16);

    public int PrimitiveCount => PrimitiveMatrixSize.x * PrimitiveMatrixSize.y;


    private void Awake()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                if (primitiveOriginalPositions.IsCreated) primitiveOriginalPositions.Dispose();
            }
        };
#endif
    }

    private void Update()
    {
        if (!objectCreateDone)
            return;

        if (primitiveMap != null && primitiveMap.Length > 0)
        {
            if (UsingJobs) 
            { 
                JobOperate(true, !SplitJobWorkingPeriod);
            }
            else {
                foreach (var wrap in primitiveMap)
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
        if (primitiveMap != null && primitiveMap.Length > 0)
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

    [Button]
    public void Debug1()
    {
        Debug.LogError("123");
    }

    public class PrimitiveWrap
    {
        public int index;
        public GameObject gameObject;
        public float3 originPosition;

        public MovementJob job;
        public JobHandle jobHandle;
        private NativeArray<float3> result;
       
        public JobHandle CreateJob()
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
            return jobHandle;
        }
        public JobHandle CreateParallelJob(NativeArray<float3> _result)
        {
            result = _result;
            job = new MovementJob()
            {
                index = index,
                pos = originPosition,
                time = Time.time,
                result = _result,
            };
            return jobHandle;
        }
        public void ScheduleJob() => ScheduleJob(jobHandle);
        public void ScheduleJob(JobHandle dependency)
        {
            jobHandle = job.Schedule(dependency);
        }
        public void JobCompleteAndApplyResult()
        {
            jobHandle.Complete();
            gameObject.transform.position = result[0];
            //gameObject.transform.position = result[0];
            if(result.IsCreated) result.Dispose();
        }
        public void ApplyParallelResult(NativeArray<float3> parallelResult)
        {
            gameObject.transform.position = parallelResult[index];
        }
        public void Dispose()
        { 
            if(gameObject != null)
                DestroyImmediate(gameObject);
            if(result.IsCreated)
                result.Dispose();
        }
    }

    private PrimitiveWrap[] primitiveMap;
    private NativeArray<float3> primitiveOriginalPositions;

    public void Clear()
    {
        if (primitiveMap != null)
        { 
            for (int i = 0; i < primitiveMap.Length; i++)
            {
                if (primitiveMap[i] == null)
                    continue;
                primitiveMap[i].Dispose();
            }
            primitiveMap?.Free();
            primitiveMap = null;
        }

        if(primitiveOriginalPositions.IsCreated) primitiveOriginalPositions.Dispose();
    }

    [NonSerialized]
    private bool objectCreateDone = false;
    public void CreateObjects()
    {
        Clear();

        primitiveMap = ArrayPool<PrimitiveWrap>.New(PrimitiveCount);

        int index = 0;
        primitiveOriginalPositions = new NativeArray<float3>(PrimitiveCount, Allocator.Persistent);

        for (int x = 0; x < PrimitiveMatrixSize.x; x++)
        {
            for (int z = 0; z < PrimitiveMatrixSize.y; z++)
            { 
                var gobj = GameObject.CreatePrimitive(PrimitiveType);
                if(PrimitiveMaterial != null)
                    gobj.GetComponent<MeshRenderer>().material = PrimitiveMaterial;

                var wrap = new PrimitiveWrap() {
                    index = index++,
                    gameObject = gobj,
                    originPosition = new Vector3(x, 0, z),
                };
                primitiveMap[wrap.index] = wrap;

                primitiveOriginalPositions[wrap.index] = wrap.originPosition;

                gobj.transform.position = wrap.originPosition;
                gobj.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        objectCreateDone = true;
    }


    private NativeArray<float3> parallelResult;
    public void JobOperate() => JobOperate(true, true);

    public JobHandle parallelJobHandle;
    public void JobOperate(bool createAndSchedule = true, bool completeAndApply = true)
    {
        if (UsingParallelJob)
        {
            if (!primitiveOriginalPositions.IsCreated)
                return;
            if (createAndSchedule)
            {
                if (parallelResult.IsCreated)
                    parallelResult.Dispose();
                parallelResult = new NativeArray<float3>(PrimitiveCount, Allocator.TempJob);

                var job = new PMovementJob();
                job.originalPositions = primitiveOriginalPositions;
                job.time = Time.time;
                job.result = parallelResult;

                parallelJobHandle = job.Schedule(PrimitiveCount, InnerloopBatchCount);
            }

            if (completeAndApply)
            {
                parallelJobHandle.Complete();

                for (int i = 0; i < primitiveMap.Length; i++)
                {
                    primitiveMap[i].ApplyParallelResult(parallelResult);
                }

                if (parallelResult.IsCreated)
                    parallelResult.Dispose();
            }
        }
        else
        {
            foreach (var wrap in primitiveMap)
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

    [BurstCompile]
    public struct PMovementJob : IJobParallelFor
    {
        public NativeArray<float3> originalPositions;
        public float time;

        public NativeArray<float3> result;
        public void Execute(int index)
        {
            var pos = originalPositions[index];
            pos.y += math.sin(math.sqrt(math.dot(pos.xz, pos.xz)) + time);
            result[index] = pos;
        }
    }
}
