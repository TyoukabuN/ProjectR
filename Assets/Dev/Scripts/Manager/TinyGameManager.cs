using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PJR
{
    public enum GameState:int
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Initialize,
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        /// <summary>
        /// 剧情模式,即Timeline
        /// </summary>
        Story,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,

        End,
        Dead,
    }
    /// <summary>
    /// 游戏结束方式
    /// </summary>
    public enum GameEndMode
    { 
        /// <summary>
        /// 状态切换至End时立即结束
        /// </summary>
        Immediately,
        /// <summary>
        /// 状态切换至End后,玩家离开视界
        /// </summary>
        PlayerOutOSight,
    }

    public class TinyGameManager : MonoBehaviour
    {
        public static TinyGameManager instance = null;
        //private static TinyGameManager old_instance = null;
        public static bool debug = true;
        //
        public GameState gameState;
        public GameEndMode gameEndMode = GameEndMode.Immediately;
        public  int frameRate = 60;
        public  float fixedDeltaTime = 0.01f;
        //environment
        public GameObject gameRoot;
        public Transform blockRequire;
        public Transform cameraRoot;
        public Transform entityRoot;
        public Transform blockRoot;
        public Transform blockRootHeap;
        public Transform blockRootTail;
        public Transform blockRootOrigin;
        public Transform backGroundBlockRoot;
        public List<GameObject> blocksPrefab;
        public List<GameObject> backGroundBlockPrefab;


        public float blockSpeedScale = 1f;
        public float bgBlockSpeedScale = 0.45f;
        //camera
        private Camera _camera;
        public Camera Camera {
            get { return _camera; }
        }
        public List<CinemachineVirtualCamera> virtualCameras;
        public CinemachineBrain cinemachineBrain;
        public bool cameraFollow {
            get {
                return cinemachineBrain && cinemachineBrain.enabled;
            }
            set {
                if(cinemachineBrain)
                    cinemachineBrain.enabled = value;
            }
        }
        //entity
        //private TPlayerEntity m_player;
        //public TPlayerEntity player {
        //    get {
        //        if (!m_player && entityRoot) {
        //            foreach (var entity in entityRoot.GetComponentsInChildren<TPlayerEntity>()) {
        //                if (entity.gameObject.tag == "Player")
        //                    m_player = entity;
        //            }
        //        }
        //        return m_player;
        //    }
        //}
        [HideInInspector] public Transform cameraSpot = null;
        public float hangingCameraSpotYFactor = 1.5f;

        public static bool playerControllable = true;

        public static bool checkPlayerOutOfSight = true;
        //
        public static Action<GameState, GameState> onStateChange;
        void OnDisable()
        {
            if (instance == this)
                instance = null;
        }
        void Awake()
        {
            instance = this;
            //if (old_instance) { 
            //    DestroyImmediate(old_instance);
            //    old_instance = null;
            //}
            Time.fixedDeltaTime = fixedDeltaTime;
            Application.targetFrameRate = frameRate;
            
            gameRoot = gameObject;
            if (gameRoot)
            {
                cameraRoot = gameRoot.transform.Find("Cameras");
                blockRequire = gameRoot.transform.Find("BlockRequire");
                entityRoot = gameRoot.transform.Find("Entitys");
                blockRoot = gameRoot.transform.Find("Blocks");
                backGroundBlockRoot = gameRoot.transform.Find("BackGroundBlocks");
            }

            if (blockRequire)
            {
                blockRootHeap = blockRequire.Find("Heap");
                blockRootTail = blockRequire.Find("Tail");
                blockRootOrigin = blockRequire.Find("Origin");
            }
            //if (entityRoot)
            //{
            //    foreach (var entity in entityRoot.GetComponentsInChildren<TPlayerEntity>())
            //    {
            //        if (entity.gameObject.tag == "Player") {
            //            m_player = entity ;
            //            if (blockRootOrigin)
            //                m_player.gameObject.transform.position = blockRootOrigin.position;
            //        }
            //    }
            //}
            if (cameraRoot)
            {
                cameraSpot = cameraRoot.Find("CameraSpot");

                _camera = cameraRoot.GetComponentInChildren<Camera>();
                virtualCameras = new List<CinemachineVirtualCamera>(cameraRoot.GetComponentsInChildren<CinemachineVirtualCamera>());
                foreach (var camera in virtualCameras)
                {
                    //if(player)
                    //    camera.Follow = cameraSpot;
                    VirtualCameraSwitcher.Register(camera);
                }
                cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
            }
            //blockRoots = new List<TBlockRoot>(gameRoot.GetComponentsInChildren<TBlockRoot>()) ;
            ////获取存在TPlatformBlock的BlockRoot
            //platformBlockRoots = new List<TBlockRoot>();
            //foreach (var blockRoot in blockRoots)
            //{
            //    var platformBlock = blockRoot.GetComponentInChildren<TPlatformBlock>();
            //    if (platformBlock) 
            //    {
            //        platformBlockRoots.Add(blockRoot);
            //        break;   
            //    }
            //    if (blockRoot.BlockPrefabs.Any(b => b.GetComponent<TPlatformBlock>() != null))
            //    { 
            //        platformBlockRoots.Add(blockRoot);
            //        break;
            //    }
            //}
        }
        public bool checkFrameRate = true;
        public void Update()
        {
            //if (checkFrameRate)
            //{
            //    if (Application.targetFrameRate != TinyGameManager.instance.frameRate)
            //    {
            //        Application.targetFrameRate = TinyGameManager.instance.frameRate;
            //    }
            //    if (Time.fixedDeltaTime != TinyGameManager.instance.fixedDeltaTime)
            //    {
            //        Time.fixedDeltaTime = TinyGameManager.instance.fixedDeltaTime;
            //    }
            //}
        }
        public void OnDestroy()
        {
            if (instance == this)
                instance = null;

            if (virtualCameras != null)
            { 
                foreach (var camera in virtualCameras)
                    VirtualCameraSwitcher.Unregister(camera);
            }
        }

        public bool IsState(GameState state)
        {
            return gameState == state;
        }
        public void SetState(GameState state)
        {
            if (gameState == state)
                return;
            var oldState = gameState;
            gameState = state;
            OnStateChange(state,oldState);
        }
        public void SwitchCamera(int index)
        {
            if (virtualCameras == null || index < 0 || index >= virtualCameras.Count)
                return;
            VirtualCameraSwitcher.SwitchCamera(virtualCameras[index]);
        }
        public void SwitchCamera(string gobjName)
        {
            if (virtualCameras == null)
                return;
            foreach (var camera in virtualCameras)
            {
                if (camera != null && camera.gameObject.name == gobjName)
                { 
                    VirtualCameraSwitcher.SwitchCamera(camera);
                    return;
                }
            }
        }
        void OnStateChange(GameState state, GameState oldState)
        {
            if (onStateChange!=null)
            {
                try
                {
                    onStateChange.Invoke(state, oldState);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            if (state == GameState.Story)
                SwitchCamera("StoryCamera");
            else if (state == GameState.End)
            {
                TinyGameUtility.Log("结束");
                cameraFollow = false;
            }
            else if (state == GameState.Running && oldState == GameState.Initialize)
            {
                if (!string.IsNullOrEmpty(CameraInitializeToRunning))
                {
                    SwitchCamera(CameraInitializeToRunning);
                }
                else
                {
                    SwitchCamera("RunningCamera");
                }
            }
            else
            { 
                SwitchCamera("RunningCamera");
            }

            CallEntitysOnStateChange(state, oldState);
        }
        [LabelText("初始化到开始的时候镜头切到")]
        public string CameraInitializeToRunning = "ZoomOutRunningCamera";
        public void CallEntitysOnStateChange(GameState state, GameState oldState)
        {
            //if(player != null)
            //    player.OnStateChange(state, oldState);

            //if (platformBlockRoots == null)
            //    return;
            //foreach (var blockRoot in platformBlockRoots)
            //{
            //    if (blockRoot == null || blockRoot.ActiveBlocks == null)
            //        continue;
            //    foreach (var block in blockRoot.ActiveBlocks)
            //    {
            //        if (block == null || block.entitys == null)
            //            continue;
            //        foreach (TEntity entity in block.entitys)
            //        {
            //            if (entity != null)
            //                entity.OnStateChange(state, oldState);
            //        }
            //    }
            //}
        }
        public bool isRunning
        {
            get { return IsState(GameState.Running); }
        }
        public bool isPause
        {
            get { return IsState(GameState.Pause); }
        }
        public bool isInStory
        {
            get { return IsState(GameState.Story); }
        }
        public bool isEnd
        {
            get { return IsState(GameState.End); }
        }
        //public Vector3 GetPlayerPos()
        //{
        //    return player.transform.position;
        //}

        // Update is called once per frame
        void FixedUpdate()
        {
            //foreach (var blockRoot in blockRoots)
            //{
            //    blockRoot.FixedUpdate();
            //}
        }
    }
}