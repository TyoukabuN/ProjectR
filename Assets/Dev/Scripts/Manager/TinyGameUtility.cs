using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//using AssetManagement;
using UnityEngine.Animations;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PJR
{
    public static class TinyGameUtility
    {
        public static Action onLoadLevelDone;
        public static GameObject currentLevel;
        public static string currentNameLevel;
        public static Vector3 levelPositon = new Vector3(2000, 2000, 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="callback"></param>
        //public static AssetInternalLoader LoadLevel(string relativePath,Action callback = null)
        //{
        //    ClearLevel();
        //    onLoadLevelDone = callback;
        //    var loader = AssetUtility.LoadAsset<GameObject>(relativePath);
        //    loader.onComplete = OnLoadLevelDone;
        //    return loader;
        //}
        public static void ClearLevel()
        {
            if (currentLevel)
            {
                GameObject.Destroy(currentLevel);
                currentLevel = null;
            }
            currentNameLevel = null;
        }
        //static void OnLoadLevelDone(AssetInternalLoader loader)
        //{
        //    if (!string.IsNullOrEmpty(loader.Error))
        //    {
        //        Debug.LogWarning(string.Format("[TinyGame][TinyGameUtility.OnLoadLevelDone]Fail to load level: %s", loader.assetName));
        //        return;
        //    }

        //    InitLevel(loader);

        //    if (onLoadLevelDone != null)
        //    {
        //        try {
        //            onLoadLevelDone.Invoke();
        //        }
        //        catch (Exception e) {
        //            Debug.LogError(e.ToString());
        //        }
        //    }
        //}

        //static void InitLevel(AssetInternalLoader loader)
        //{
        //    var gobj = loader.Instantiate<GameObject>();
        //    gobj.transform.position = levelPositon;
        //    currentLevel = gobj;
        //    currentNameLevel = loader.assetName;
        //}

        public static void Log(object message)
        {
            if (!TinyGameManager.debug)
                return;
            try
            {
                Debug.Log(message);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        public static void Log(object message,UnityEngine.Object context)
        {
            if (!TinyGameManager.debug)
                return;
            try
            {
                Debug.Log(message, context);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private static Dictionary<string, Coroutine> m_LoadingOverrideClip = new Dictionary<string, Coroutine>();
        //public static void LoadAnimationClipAndAssignToOC(MonoBehaviour coroutineTarget, Animator animator,string clipName,string overrideClipName,Action callback = null)
        //{
        //    if (!TinyGameManager.instance)
        //        return;
        //    if (!coroutineTarget)
        //        return;    
        //    string guid = animator.GetInstanceID().ToString() + clipName;
        //    Coroutine coroutine = null;
        //    if (m_LoadingOverrideClip.TryGetValue(guid, out coroutine))
        //    { 
        //        TinyGameManager.instance.StopCoroutine(coroutine);
        //        m_LoadingOverrideClip.Remove(guid);
        //    }
        //    //
        //    coroutine = coroutineTarget.StartCoroutine(LoadAnimatonClip(animator, clipName, overrideClipName, callback));
        //    if (coroutine != null)
        //        m_LoadingOverrideClip[guid] = coroutine;
        //}

        /// <summary>
        /// 加载动画片段
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="AssetName"></param>
        /// <param name="animator"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        //private static IEnumerator LoadAnimatonClip(Animator animator, string clipName, string overrideClipName, Action callback = null)
        //{
        //    string loadAssetName = overrideClipName;
        //    if (string.IsNullOrEmpty(loadAssetName))
        //    {
        //        XLogger.WARNING_Format("XAvatarPart.LoadAnimatonClip loadAssetName IsNullOrEmpty clipName:{0},assetName:{1}", clipName, loadAssetName);
        //        SafeInvokeAction(callback, "TinyGame.TinyGameUtility.LoadAnimatonClip");
        //        yield break;
        //    }

        //    //项目中是否存在该资源
        //    if (!AssetUtility.Contains(loadAssetName))
        //    {
        //        XLogger.WARNING_Format("XAvatarPart.LoadAnimatonClip load error name:{0}  loadAssetName:{1}{2}", clipName, loadAssetName, " 资源不存在");
        //        SafeInvokeAction(callback, "TinyGame.TinyGameUtility.LoadAnimatonClip");
        //        yield break;
        //    }

        //    AnimationClip clip = null;
        //    if (AssetCache.ContainsRawObject(loadAssetName))
        //    {
        //        clip = AssetCache.GetRawObject<AnimationClip>(loadAssetName);
        //    }
        //    if (!clip)
        //    {
        //        AssetInternalLoader async = AssetUtility.LoadAsset<AnimationClip>(loadAssetName);

        //        yield return async;

        //        clip = async.GetRawObject<AnimationClip>();
        //    }

        //    AnimatorOverrideController oc = animator.runtimeAnimatorController as AnimatorOverrideController;
        //    if (oc == null)
        //    {
        //        SafeInvokeAction(callback, "TinyGame.TinyGameUtility.LoadAnimatonClip");
        //        yield break;
        //    }
        //    var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        //    oc.GetOverrides(overrides);
        //    if (overrides.Count <= 0)
        //    {
        //        //ListPool<KeyValuePair<AnimationClip, AnimationClip>>.Release(overrides);
        //        SafeInvokeAction(callback, "TinyGame.TinyGameUtility.LoadAnimatonClip");
        //        yield break;
        //    }
        //    for (int i = 0; i < overrides.Count; i++)
        //    {
        //        if (overrides[i].Key.name == clipName)
        //        {
        //            //Debug.LogError("clipName========" + clipName);
        //            //overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, clip);
        //            //oc.ApplyOverrides(overrides);
        //            oc[clipName] = clip;
        //            //ListPool<KeyValuePair<AnimationClip, AnimationClip>>.Release(overrides);

        //            SafeInvokeAction(callback, "TinyGame.TinyGameUtility.LoadAnimatonClip");

        //            yield break;
        //        }
        //    }
        //    SafeInvokeAction(callback, "TinyGame.TinyGameUtility.LoadAnimatonClip");
        //}

        //private static void SafeInvokeAction(Action callback,string msg = "")
        //{
        //    if (callback == null)
        //        return;
        //    try
        //    {
        //        callback.Invoke();
        //    }
        //    catch(Exception e) {
        //        XLogger.ERROR(msg + ":" + e.ToString());
        //    }
        //}

        public static void ResetPlayerPosition()
        {
            ResetPlayerPosition(100f, Vector2.zero, new Vector2(-5, 0));
        }
        public static void ResetPlayerPosition(Vector2 offset)
        {
            ResetPlayerPosition(100f, Vector2.zero, offset);
        }
        public static void ResetPlayerPosition(Vector2 floorCheckOffset, Vector2 offset)
        {
            ResetPlayerPosition(100f, floorCheckOffset, offset);
        }
        public static void ResetPlayerPosition(float floorCheckRange,Vector2 floorCheckOffset, Vector2 offset)
        {
            if (TinyGameManager.instance == null)
                return;
            var player = TinyGameManager.instance.player;
            if (player == null)
                return;

            Vector2 pos = TinyGameManager.instance.cameraSpot.position;

            player.ClearTweenerPosY();

            float dis = floorCheckRange;
            Vector2 rayOrigin = new Vector2(player.transform.position.x, player.transform.position.y + dis) + floorCheckOffset;
//            RaycastHit2D res = Physics2D.Raycast(rayOrigin, Vector3.down, dis * 1.1f, TBlockUtility.FloorLayer);
//            if (res)
//            {
//#if UNITY_EDITOR
//                Debug.DrawLine(rayOrigin, rayOrigin + (Vector2.down * dis * 1.1f), Color.blue, 1f);
//#endif
//                pos = res.point;
//                pos += new Vector2(0, player.capsuleCollider2d.size.y / 2) + player.capsuleCollider2d.offset;
//            }

            //
            //偏移
            pos += offset;
            player.transform.position = pos;
            //设置镜头点
            TinyGameManager.instance.cameraSpot.position = player.transform.position;
        }

        public static void SetTranslationOffsetEx(this ParentConstraint parentConstraint, int index, Vector3 value)
        {
            if (parentConstraint == null)
                return;
            parentConstraint.SetTranslationOffset(index, value);
        }
    }
}
