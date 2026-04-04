# 如何实现一个新的 Clip 类型

参考实现：`Assets/Scripts/Timeline/Runtime/Impl/Animation/AnimancerClip`（共4个文件）

---

## 文件结构规范

每种 Clip 实现拆分为以下文件，统一放在 `Assets/Scripts/Timeline/Runtime/Impl/<功能名>/` 目录下：

```
YourClip.cs                  # 数据层（Clip 字段 + 菜单注册）
YourClip.Runner.cs           # 运行时执行器（partial class 内嵌 Runner）
YourClip.PreviewRunner.cs    # 编辑器预览执行器（#if UNITY_EDITOR）
YourClip.Extension.cs        # 可选：运行时快捷创建方法（RuntimeTemplate 场景用）
```

所有文件通过 `public partial class YourClip` 合并为同一个类。

---

## 1. 数据层 `YourClip.cs`

```csharp
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Timeline
{
    [Serializable]
    [TrackCreateMenuItem(nameof(YourClip))]  // 在编辑器 + 按钮中显示该 Clip 类型
    public partial class YourClip : Clip
    {
        // 序列化数据字段
        public SomeDataType someData;

        // 在轨道上显示的名称（固定类型名）
        public override string GetClipName() => "你的功能名";

        // 在轨道 Clip 块上显示的具体信息（可含动态数据）
        public override string GetClipInfo()
        {
            if (someData == null)
                return "[功能名] null";
            return $"[功能名] {someData.name}";
        }

        // 运行时 Runner 工厂方法——固定写法，从对象池获取
        public override ClipRunner GetRunner() => Runner.Get(this);

#if UNITY_EDITOR
        // 编辑器预览 Runner 工厂方法——固定写法，从对象池获取
        public override ClipRunner Editor_GetPreviewRunner() => PreviewRunner.Get(this);

        // 右键菜单（可选，不需要时不重写）
        public override void GetContextMenu(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent("你的功能名"));
        }
#endif
    }
}
```

**关键点：**
- 必须标 `[Serializable]` 和 `[TrackCreateMenuItem]`
- `GetRunner()` / `Editor_GetPreviewRunner()` 固定从对象池（`Pool.ObjectPool<T>.Get()`）获取，不要 `new`
- 数据字段直接 `public` 序列化，Odin 会自动处理复杂类型

---

## 2. 运行时执行器 `YourClip.Runner.cs`

```csharp
using System;

namespace PJR.Timeline
{
    public partial class YourClip
    {
        public class Runner : ClipRunner<YourClip>
        {
            // 必须实现，返回对应 Clip 类型
            public override Type ClipType => typeof(YourClip);

            // 运行时私有状态字段（组件引用、状态缓存等）
            private SomeComponent _component;

            // 两个构造器——固定写法
            public Runner() : base(null) { }
            public Runner(YourClip clip) : base(clip) { }

            // 对象池工厂方法——固定写法
            public static ClipRunner Get(YourClip clip) => Pool.ObjectPool<Runner>.Get()?.Reset(clip);

            // Clip 进入时间范围时调用一次
            public override void OnStart(UpdateContext context)
            {
                base.OnStart(context);  // 必须调用，设置 runnerState = Running
                _component = context.gameObject?.GetComponent<SomeComponent>();
                if (_component == null)
                {
                    AsFailure("component == null");  // 标记失败，后续 Update 不会再调用
                    return;
                }
                // 初始化逻辑...
            }

            // 以帧间隔更新（每帧触发一次，适合帧精度逻辑）
            protected override void OnFrameUpdate(UpdateContext context)
            {
                // 通常留空，或做帧精度控制
            }

            // 以 deltaTime 间隔更新（按秒推进，适合连续插值）
            protected override void OnDeltaUpdate(UpdateContext context)
            {
                // GetLocalSecond() 返回当前时间减去 Clip.start，即 Clip 内本地时间
                float localTime = GetLocalSecond();
                // 驱动组件...
            }

            // Clip 超出时间范围时调用，基类已处理 runnerState = Done，子类重写 OnEnd
            // 注意：不要在 OnEnd 里调 Clear()，否则组件引用丢失，导致 Clip 二次进入时无法重新播放
            protected override void OnEnd()
            {
                // 停止调用组件效果，保留引用支持二次播放
                // 示例：_component.Stop();
            }

            // 清理私有状态——对象池归还时也会调用
            protected override void OnClear()
            {
                base.OnClear();
                // 停止组件效果、归还引用
                _component = null;
            }

            // 对象池归还——固定写法
            public override void Release()
            {
                Clear();
                Pool.ObjectPool<Runner>.Release(this);
            }
        }
    }
}
```

**关键点：**
- `OnStart` 必须先调用 `base.OnStart(context)`
- 失败时调用 `AsFailure("原因")`，之后 `IsFailure == true`，`OnUpdate` 不会再被调用
- `GetLocalSecond()` = `context.currentTime - clip.start`，即 Clip 内的本地秒数
- `OnFrameUpdate` vs `OnDeltaUpdate`：由上层 `context.updateIntervalType` 决定走哪个，通常连续驱动写在 `OnDeltaUpdate`
- `OnEnd()`：Clip 超出时间范围时由基类 `End()` 调用，子类在此做收尾（如停止组件播放）；**不要在 `OnEnd()` 里调 `Clear()`**，否则组件引用丢失导致无法二次播放
- `Release()` 固定写法：`Clear()` + `ObjectPool.Release(this)`

---

## 3. 编辑器预览执行器 `YourClip.PreviewRunner.cs`

```csharp
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class YourClip
    {
        public class PreviewRunner : ClipRunner<YourClip>
        {
            public override Type ClipType => typeof(YourClip);

            private SomeComponent _component;

            public PreviewRunner() : base() { }
            public PreviewRunner(YourClip clip) : base(clip) { }

            public static ClipRunner Get(YourClip clip) => Pool.ObjectPool<PreviewRunner>.Get()?.Reset(clip);

            public override void OnStart(UpdateContext context)
            {
                base.OnStart(context);
                _component = context.gameObject?.GetComponent<SomeComponent>();
                if (_component == null)
                {
                    AsFailure("component == null");
                    return;
                }
                // EditMode 下的初始采样
                SamplePreview(0f);
            }

            protected override void OnFrameUpdate(UpdateContext context) { }

            protected override void OnDeltaUpdate(UpdateContext context)
            {
                // EditMode 采样：用 GetLocalSecond() 驱动预览
                SamplePreview(GetLocalSecond());
            }

            private void SamplePreview(float localTime)
            {
                // EditMode 安全检查
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;
                if (_component == null)
                    return;
                // 采样逻辑（AnimancerClip 用 AnimationClip.SampleAnimation）
            }

            // EditMode 预览结束时通常不需要特殊处理，OnEnd() 留空或不重写
            // End() 不可重写，基类会自动调用 OnEnd()

            protected override void OnClear()
            {
                base.OnClear();
                // 恢复初始状态（AnimancerClip 调 _animator.Rebind()）
                _component = null;
            }

            public override void Release()
            {
                Clear();
                Pool.ObjectPool<PreviewRunner>.Release(this);
            }
        }
    }
}
#endif
```

**关键点：**
- 整个文件包在 `#if UNITY_EDITOR` 内
- 采样函数内加 `EditorApplication.isPlayingOrWillChangePlaymode` 守卫，避免 PlayMode 误调用
- `End()` 不可重写（基类 sealed），预览结束无需特殊处理时直接不重写 `OnEnd()`；`OnClear()` 负责恢复预览对象初始状态（如 `Animator.Rebind()`）

---

## 4. 快捷创建扩展 `YourClip.Extension.cs`（按需）

适用于代码动态构建 `RuntimeTemplateSequence` 的场景：

```csharp
using UnityEngine;

namespace PJR.Timeline
{
    public partial class YourClip : Clip
    {
        // 按帧范围创建
        public static YourClip GetRuntimeTemplate(SomeDataType data, int startFrame, int endFrame)
        {
            var temp = CreateInstance<YourClip>();
            temp.someData = data;
            temp.SetFrameScope(startFrame, endFrame);
            return temp;
        }

        // 按时间范围创建
        public static YourClip GetRuntimeTemplate(SomeDataType data, float startTime, float endTime)
        {
            var temp = CreateInstance<YourClip>();
            temp.someData = data;
            temp.SetTimeScope(startTime, endTime);
            return temp;
        }
    }
}
```

---

## 注意事项

1. **不要直接 `new Runner()`**，必须通过 `Pool.ObjectPool<Runner>.Get()` 从池中获取，否则破坏对象池复用。
2. **`OnStart` 必须调 `base.OnStart(context)`**，否则 `runnerState` 不会设为 `Running`，后续 Update 不会执行。
3. **`ClipType` 必须返回 `typeof(YourClip)`**，`Global.cs` 的反射映射依赖此字段，否则 Clip 无法自动找到 Runner。
4. **Clip 自动注册**，`Global.cs` 反射扫描所有继承 `ClipRunner<T>` 的类，无需手动注册映射。
5. **OnDeltaUpdate 用于连续驱动，OnFrameUpdate 用于帧精度逻辑**（如关键帧触发、状态机切换）。
6. **PreviewRunner 的 `End()` 通常留空**，因为编辑器游标拖拽时 Clip 会反复进出范围，`OnClear` 才是真正的清理时机。
