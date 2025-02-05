using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class UnitaskTest : MonoBehaviour
{
    [Button("Test")]
    void Test()
    {
        AsyncTest();
    }

    async void AsyncTest()
    {
        await UniTask.Delay(1000);
        Debug.Log("Done");
    }

    // 您可以返回一个形如 UniTask<T>(或 UniTask) 的类型，这种类型事为Unity定制的，作为替代原生 Task<T> 的轻量级方案
    // 为 Unity 集成的零堆内存分配，快速调用，0消耗的 async/await 方案
    async UniTask<string> DemoAsync()
    {
        // 您可以等待一个 Unity 异步对象
        var asset = await Resources.LoadAsync<TextAsset>("foo");
        var txt = (await UnityWebRequest.Get("https://...").SendWebRequest()).downloadHandler.text;
        await SceneManager.LoadSceneAsync("scene2");

        // .WithCancellation 会启用取消功能，GetCancellationTokenOnDestroy 表示获取一个依赖对象生命周期的 Cancel 句柄，当对象被销毁时，将会调用这个 Cancel 句柄，从而实现取消的功能
        // 在 Unity 2022.2之后，您可以在 MonoBehaviour 中使用`destroyCancellationToken`
        var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());
        
        // .ToUniTask 可接收一个 progress 回调以及一些配置参数，Progress.Create 是 IProgress<T> 的轻量级替代方案
        var asset3 = await Resources.LoadAsync<TextAsset>("baz").ToUniTask(Progress.Create<float>(x => Debug.Log(x)));

        // 等待一个基于帧的延时操作（就像一个协程一样）
        await UniTask.DelayFrame(100);

        // yield return new WaitForSeconds/WaitForSecondsRealtime 的替代方案
        await UniTask.Delay(TimeSpan.FromSeconds(10), ignoreTimeScale: false);

        // 可以等待任何 playerloop 的生命周期（PreUpdate，Update，LateUpdate等）
        await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

        // yield return null 的替代方案
        await UniTask.Yield();
        await UniTask.NextFrame();

        // WaitForEndOfFrame 的替代方案
#if UNITY_2023_1_OR_NEWER
    await UniTask.WaitForEndOfFrame();
#else
        // 需要 MonoBehaviour（CoroutineRunner）
        await UniTask.WaitForEndOfFrame(this); // this是一个 MonoBehaviour
#endif

        // yield return new WaitForFixedUpdate 的替代方案，（等同于 UniTask.Yield(PlayerLoopTiming.FixedUpdate)）
        await UniTask.WaitForFixedUpdate();

        // yield return WaitUntil 的替代方案
        await UniTask.WaitUntil(() => isActiveAndEnabled == false);

        // WaitUntil 扩展，指定某个值改变时触发
        await UniTask.WaitUntilValueChanged(this, x => x.isActiveAndEnabled);

        // 您可以直接 await 一个 IEnumerator 协程
        //await FooCoroutineEnumerator();

        // 您可以直接 await 一个原生 task
        await Task.Run(() => 100);

        // 多线程示例，在此行代码后的内容都运行在一个线程池上
        await UniTask.SwitchToThreadPool();

        /* 工作在线程池上的代码 */

        // 转回主线程（等同于 UniRx 的`ObserveOnMainThread`）
        await UniTask.SwitchToMainThread();

        // 获取异步的 webrequest
        async UniTask<string> GetTextAsync(UnityWebRequest req)
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }

        var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
        var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
        var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));

        // 构造一个 async-wait，并通过元组语义轻松获取所有结果
        var (google, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);

        // WhenAll 的简写形式，元组可以直接 await
        var (google2, bing2, yahoo2) = await (task1, task2, task3);

        // 返回一个异步值，或者您也可以使用`UniTask`（无结果），`UniTaskVoid`（不可等待）
        return (asset as TextAsset)?.text ?? throw new InvalidOperationException("Asset not found");
    }
}
