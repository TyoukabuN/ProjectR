# Timeline 自定义序列系统代码解释

## 概述
位于 `Assets/Scripts/Timeline` 的自定义 Timeline 系统，提供运行时序列播放和编辑器时间轴编辑功能。采用 Runner 模式执行，支持帧率感知时序、编辑器预览和运行时播放。

**不依赖 Unity 原生 PlayableGraph**，而是自建了一套 SequenceAsset → Track → Clip 的数据模型和对应的 Runner 执行框架。

---

## 目录结构

```
Assets/Scripts/Timeline/
├── Runtime/
│   ├── Define.cs                    # 核心枚举与结构体
│   ├── Global.cs                    # Clip→Runner 全局映射（反射自动发现）
│   ├── Debug.cs                     # 条件编译日志
│   ├── SequenceDirector.cs          # MonoBehaviour 播放入口
│   ├── SequenceDirector.SequenceHandle.cs  # Handle 抽象基类
│   ├── SequencePreviewHandle.cs     # 编辑器预览 Handle
│   ├── RuntimeSequenceHandle.cs     # 运行时 Handle
│   ├── ISequenceHandle.cs           # 句柄接口
│   ├── ISequenceHolder.cs           # 持有序列的对象接口
│   ├── IErrorRecorder.cs            # 错误记录接口
│   ├── RuntimeTemplateSequence.cs        # 无资产的运行时临时序列
│   ├── RuntimeTemplateSequence.Runner.cs # 对应 Runner
│   ├── Unit/
│   │   ├── UnitRunner.cs            # Runner 最底层基类 + UnitRunner<T> 泛型中间层
│   │   ├── SequenceAsset.cs         # 序列资产（ScriptableObject）
│   │   ├── SequenceAsset.Runner.cs  # 序列执行器（多轨协调）
│   │   ├── SequenceRunner.cs        # SequenceRunner 抽象基类（继承 UnitRunner<TrackRunner>）
│   │   ├── Track.cs                 # 轨道（Clip 容器）
│   │   ├── TrackRunner.cs           # 轨道执行器（继承 UnitRunner<ClipRunner>）
│   │   ├── Clip.cs                  # Clip 抽象基类
│   │   └── ClipRunner.cs            # ClipRunner 抽象基类
│   ├── Impl/Animation/
│   │   ├── AnimancerClip.cs              # Animancer 动画 Clip
│   │   ├── AnimancerClip.Runner.cs       # 运行时动画控制
│   │   ├── AnimancerClip.PreviewRunner.cs # 编辑器预览动画采样
│   │   └── AnimancerClip.Extension.cs    # 运行时快捷创建
│   ├── Pool/
│   │   └── ObjectPool.cs            # 通用对象池
│   └── Utility/
│       ├── TimeUtil.cs              # 帧/时间转换工具
│       └── Utility.cs               # 反射与类型工具
├── Editor/
│   ├── TimelineWindow.cs                    # 主编辑器窗口
│   ├── TimelineWindow.WindowState.cs        # 窗口状态管理（播放控制、SequenceHandle管理）
│   ├── TimelineWindow.Defines.cs            # 窗口常量
│   ├── TimelineWindow.EventCallback.cs      # 事件回调（OnEditorUpdate tick）
│   ├── TimelineWindow.Selection.cs          # 选择管理
│   ├── TimelineWindow.Shortcut.cs           # 快捷键
│   ├── TimelineWindow.Inspector.cs          # Inspector 面板
│   ├── GUIControl/
│   │   ├── ClipDrawer.cs            # Clip 可视化与交互
│   │   ├── TrackDrawer.cs           # Track 渲染
│   │   ├── TrackGUI.cs              # Track 视图布局
│   │   ├── TimelineGUIElement.cs    # GUI 元素基类
│   │   ├── MenuDrawer.cs            # 右键菜单
│   │   └── MenuDrawer.Static.cs     # 菜单工具函数
│   ├── Layout/
│   │   ├── GUIViewportScope.cs      # 视口变换 Scope
│   │   └── MidAlignmentScope.cs     # 垂直居中 Scope
│   ├── Static/
│   │   ├── ClipEditorHelper.cs      # Clip 编辑辅助
│   │   ├── Const.cs                 # 编辑器常量
│   │   ├── Define.cs                # 编辑器定义
│   │   ├── EditorPreferences.cs     # 持久化偏好
│   │   ├── EventUtil.cs             # 事件工具
│   │   ├── GUIUtil.cs               # GUI 工具
│   │   ├── Graphics.cs              # 自定义绘图
│   │   ├── SequenceUndo.cs          # 撤销操作
│   │   ├── SequenceUnitCreateHelper.cs # 资产创建
│   │   ├── TimeReferenceUtility.cs  # 时间坐标系转换
│   │   └── TimelineAssetMenus.cs    # 资产菜单
│   ├── Styles.cs                    # GUI 样式
│   ├── DirectorNamedColor.cs        # 主题颜色
│   └── State/
│       └── ISequenceState.cs        # 状态接口
```

---

## 核心架构

### 数据层级
```
SequenceAsset (ScriptableObject)
  └── Track[] tracks
        └── Clip[] clips (各种类型如 AnimancerClip)
```

### Runner 执行层级
```
UnitRunner (abstract)                         ← 状态机、播放控制、时间字段、Sequence属性
  └── UnitRunner<TSubRunner> (abstract)        ← 持有子Runner列表，封装ForeachSubRunner
        ├── SequenceRunner (abstract)           ← 继承 UnitRunner<TrackRunner>，时间驱动
        │     ├── SequenceAsset.Runner          ← 多轨协调，SecondTimeDriver + FrameTimeDriver
        │     └── RuntimeTemplateSequence.Runner ← 单轨简化版
        └── TrackRunner                        ← 继承 UnitRunner<ClipRunner>，管理Clip执行
              └── ClipRunner (abstract)         ← 继承 UnitRunner，具体Clip逻辑
                    └── ClipRunner<TClip>       ← 泛型，关联具体Clip类型
```

### 状态流转
```
None → Running → Done / Paused / Failure → Disposed
```
Runner 状态变更通过 `OnStateChanged` Action 触发外部监听回调。
`Play()` / `Pause()` 会通过 `ForeachSubRunner` 递归向下传播状态。

---

## Runner 基类详解

### `UnitRunner` — 最底层基类
所有 Runner 的根，提供：
- `ISequence Sequence { get; protected set; }` — 关联的序列对象，由子类在 Reset 时赋值
- `ERunnerState runnerState` — 状态机，变更时触发 `OnStateChanged`
- `TotalTime` / `UnscaleTotalTime` — 时间累计
- `Play()` — 设自身 Running，通过 `ForeachSubRunner` 调 `OnPlay()` 递归传播
- `Pause()` — 设自身 Paused，通过 `ForeachSubRunner` 调 `OnPause()` 递归传播
- `SetRunnerStateRecursive(state)` — 强制递归设置所有子Runner状态（SeekTo用）
- `Clear()` — sealed，递归清理子Runner → OnClear() → 重置基类字段
- `abstract OnPlay()` / `abstract OnPause()` — 子类实现具体状态响应

### `UnitRunner<TSubRunner>` — 泛型中间层
- 持有 `protected List<TSubRunner> _subRunners`
- 重写 `ForeachSubRunner(Action<UnitRunner>)` 桥接为类型安全版本
- 提供 `ForeachSubRunner(Action<TSubRunner>)` 供子类直接使用
- 提供 `ClearSubRunner(TSubRunner)` 辅助方法

### `SequenceRunner` — 序列级 Runner 基类
继承 `UnitRunner<TrackRunner>`，子类通过 `_subRunners` 管理 TrackRunner 列表。
- `Sequence` 属性已在 `UnitRunner` 基类，Reset 时直接赋值即可
- `DriveUpdate(deltaTime)` — 入口，有 `maximumDeltaTime = 0.1f` 上限保护
- `GetTimeScale()` — **EditMode 下返回 1.0，PlayMode 下返回 `Time.timeScale`**
- `GetSecondPerFrame()` / `GetSecondPerFrame_Float()` — 从 `Sequence.FrameRateType` 计算

### `TrackRunner` — 轨道级 Runner
继承 `UnitRunner<ClipRunner>`，通过 `_subRunners` 管理 ClipRunner 列表。
- `public List<ClipRunner> clipRunners => _subRunners` — 只读暴露
- `Reset(ISequence, ITrack)` — 创建所有 ClipRunner，赋值 `Sequence`
- `OnUpdate(context)` — 判断每个 Clip 的时间范围，驱动 Start/Update/End
- `OnPlay()` 和 `OnPause()` 会通过 `ForeachSubRunner` 调 `sub.Play()` / `sub.Pause()` 向下传播

---

## Handle 体系详解

### 接口层
```
ISequenceHandle
  ├── float Time { get; set; }      ← 当前时间（游标位置）
  ├── bool Valid
  ├── ISequence Sequence
  ├── SequenceAsset SequenceAsset
  └── void Release()

ISequencePlayableHandle : ISequenceHandle
  ├── SequenceDirector Director
  ├── bool IsPlaying()
  ├── void Play() / Pause() / Stop()
  ├── void SeekTo(float time)
  └── void ManualUpdate(float deltaTime)
```

### 实现层
```
SequenceDirector.SequenceHandle (abstract)  ← 连接 Director，ISequencePlayableHandle 的基类
  ├── PreviewSequenceHandle                 ← EditMode 预览用
  └── RuntimeSequenceHandle                ← PlayMode 运行时用

TimelineWindow.SequenceEditHandle           ← 仅编辑器窗口用，只持有 SequenceAsset，无 Director
```

### 关键细节：`Time` 属性
`SequenceDirector.SequenceHandle` 中存在两个时间属性：
- `public float time` (小写) — 读写 `_director.Runner.TotalTime`，是真实时间
- `float ISequenceHandle.Time` (大写，显式接口实现) — **转发到 `time`**，编辑器游标读的就是这个

> 注意：如果 `ISequenceHandle.Time` 改成独立自动属性 `{ get; set; }` 将与 Runner 断开，导致游标停止不动。

`SequenceEditHandle.Time` 是独立字段，仅用于编辑器拖拽时的临时存储。

---

## 编辑器 Tick 机制

### EditMode 下（非 Play 模式）
```
EditorApplication.update
  → TimelineWindow.OnEditorUpdate()
      if (EditorApplication.isPlaying) return;   ← PlayMode 下不走这里
      deltaTime = timeSinceStartup - lastUpdateTime
      State.ManualUpdateDirector(deltaTime)
        → SequencePlayableHandle.ManualUpdate(deltaTime)
          → Director.ManualUpdate(deltaTime)
            → if (runner.runnerState == Running) runner.DriveUpdate(deltaTime)
              → SequenceRunner.OnDriveUpdate(deltaTime)
                → TotalTime += deltaTime * GetTimeScale()  ← EditMode下GetTimeScale()=1.0
```

### PlayMode 下
- `OnEditorUpdate` 直接 return，**编辑器预览按钮的 Play/Pause 在 PlayMode 下无效**
- `SequenceDirector.Update()` 自己调 `ManualUpdate(Time.deltaTime)` 驱动 Runner
- 编辑器窗口 `TimelineWindow.Update()` 通过 `State.IsPlaying` 触发 `Repaint()`，但游标时间读自 `SequenceHandle.Time`

### 游标绘制时间来源
```csharp
// TimelineWindow.cs Draw_TimeRulerCursor()
float posX = State.TimeToPixel(State.SequenceHandle.Time);  // 读 ISequenceHandle.Time
```
`State.SequenceHandle` 在 EditMode 预览时是 `PreviewSequenceHandle`，其 `Time` 转发到 `Director.Runner.TotalTime`。

---

## 关键类说明

### `Define.cs` — 核心定义
- `ERunnerState`: Runner 状态枚举 (None/Running/Done/Paused/Failure/Disposed)
- `EFrameRate`: 帧率枚举 — Game(60fps)、HD(30fps)、Film(24fps)
- `UpdateContext`: 更新上下文结构体，包含 totalTime、totalFrame、timeScale、intervalType、gameObject
- `IntervalType`: Second（deltaTime间隔）/ Frame（帧间隔）

### `Global.cs` — 类型自动映射
- `Clip2ClipHandleFunc`: Clip 类型→ClipRunner 类型的映射委托
- `Default_Clip2ClipHandleFunc()`: 反射扫描所有程序集，找到继承 `ClipRunner<TClip>` 的类型，自动建立映射缓存
- 首次执行 Clip 时自动触发，后续使用缓存

### `SequenceDirector.cs` — 播放控制入口
- MonoBehaviour，挂载在 GameObject 上
- `ManualUpdate(deltaTime)` — 核心驱动，判断 None→OnStart，Running→DriveUpdate，Disposed→Release Runner
- `EnsureRunnerReady()` — 确保 Runner 已初始化并 OnStart
- `SeekTo(seekTime)` — 保存前一状态，Play + DriveUpdate(0) 强制刷新，再恢复状态
- `GetHandle()` — EditMode 返回 `PreviewSequenceHandle`，PlayMode 返回 `RuntimeSequenceHandle`

### `SequenceAsset.cs` — 序列资产
- ScriptableObject，定义帧率和轨道列表
- `GetRunner(gameObject)` 创建 SequenceAsset.Runner

### `SequenceAsset.Runner.cs` — 序列资产执行器
- `Reset(gameObject, sequenceAsset)` — 创建所有 TrackRunner，赋值 `Sequence = sequenceAsset`
- `OnDriveUpdate(deltaTime)` — SecondTimeDriver 驱动 deltaTime 更新，FrameTimeDriver 驱动帧更新
- 同时维护 `TotalTime`（秒累计）和 `_totalFrame`（帧累计）

### `TrackRunner.cs` — 轨道执行器
- `Reset(ISequence, ITrack)` — 创建 ClipRunner 列表（EditMode 用 PreviewRunner，PlayMode 用 Runner）
- `OnUpdate(context)` — 遍历 Clip，调 `OutOfRange()` 判断时间，驱动 Start/Update/End
- `IsClipRunnerUpdatable()` — 过滤 null 和已完成的 ClipRunner

### `Clip.cs` — Clip 抽象基类
- 定义时间信息: `start`/`end`（秒）
- `OutOfRange(totalTime, spf)` — 判断当前时间是否已超出 Clip 范围
- `abstract GetRunner()` — 运行时 ClipRunner
- `abstract Editor_GetPreviewRunner()` — 编辑器预览 ClipRunner

### `ClipRunner.cs` — ClipRunner 抽象基类
- `ClipRunner<TClip>`: 泛型，关联具体 Clip 类型，通过 `Reset(clip)` 初始化
- 生命周期: `OnInit()` → `OnStart(context)` → `OnUpdate(context)` (Frame/Delta 分发) → `End()`
- `updateContext` — 最近一次更新上下文，可用 `GetLocalSecond()` 获取 Clip 内本地时间

### `AnimancerClip` — 动画 Clip 实现
- **AnimancerClip.cs**: 数据层，持有 `AnimationClip` 引用和速度等参数
- **AnimancerClip.Runner.cs**: 运行时用 AnimancerComponent 播放动画
- **AnimancerClip.PreviewRunner.cs**: 编辑器用 `AnimationClip.SampleAnimation()` 采样预览

### `WindowState` — 编辑器窗口状态
- `SequenceHandle` — 当前编辑中的 Handle，设置时自动缓存 `_sequencePlayableHandle`
- `IsPlaying` — `_sequencePlayableHandle?.IsPlaying()`
- `Play()` / `Pause()` — **有 `EditorApplication.isPlaying` 守卫，PlayMode 下无效**
- `ManualUpdateDirector(deltaTime)` — 转发到 `SequencePlayableHandle.ManualUpdate()`
- `TimeToPixel(time)` / `PixelToTime(pixel)` — 时间↔像素坐标转换

---

## 编辑器-运行时双路径

| 场景 | Handle 类型 | ClipRunner 来源 | Tick 来源 |
|------|------------|----------------|----------|
| EditMode 预览 | `PreviewSequenceHandle` | `Clip.Editor_GetPreviewRunner()` | `EditorApplication.update → OnEditorUpdate` |
| PlayMode 运行时 | `RuntimeSequenceHandle` | `Clip.GetRunner()` | `SequenceDirector.Update()` |

> PlayMode 下编辑器预览窗口的播放按钮**不工作**，`WindowState.Play()` 内有 `isPlaying` 守卫。

---

## 帧率系统

| 枚举值 | FPS | 用途 |
|--------|-----|------|
| Game | 60 | 默认游戏帧率 |
| HD | 30 | 高清视频帧率 |
| Film | 24 | 电影帧率 |

`TimeUtil` 提供帧↔秒的精确转换，支持 epsilon 容差的帧对齐舍入。

---

## 运行时播放流程

1. `SequenceDirector.Start()` → 若 PlayOnAwake → `GetRunner()`
2. `GetRunner()` → `Sequence.GetRunner(gameObject)` 创建 SequenceRunner，Runner.Sequence 赋值
3. 每帧 `SequenceDirector.Update()` → `ManualUpdate(deltaTime)`
4. `ManualUpdate`: None → `OnStart()`；Running → `DriveUpdate(deltaTime)`
5. `SequenceAsset.Runner.OnDriveUpdate()` → `TotalTime += deltaTime * GetTimeScale()`
6. SecondTimeDriver 触发 `OnUpdateInternal()` → 遍历 `_subRunners`（TrackRunner）
7. 各 `TrackRunner.OnUpdate(context)` → 判断各 Clip 时间范围
8. 进入范围: `ClipRunner.OnStart()` → `ClipRunner.OnUpdate()`
9. 超出范围: `ClipRunner.End()`
10. 所有轨道完成 → SequenceRunner 状态变为 Done

---

## 编辑器预览流程

1. 双击 SequenceAsset 或选中带 SequenceDirector 的 GameObject → TimelineWindow 打开
2. `State.SequenceHandle = PreviewSequenceHandle`
3. 点击播放按钮 → `WindowState.Play()` → `Director.Play()` → `Runner.Play()`
4. `EditorApplication.update` 每帧触发 `OnEditorUpdate` → `ManualUpdateDirector(deltaTime)`
5. `Runner.DriveUpdate()` → `TotalTime` 增加（`GetTimeScale()` 在 EditMode 返回 1.0）
6. `TimelineWindow.Update()` 检测 `State.IsPlaying` → `Repaint()`
7. 游标位置 = `TimeToPixel(SequenceHandle.Time)` → `SequenceHandle.Time` 读 `Runner.TotalTime`

---

## 类型发现机制 (`Global.cs`)

1. 首次 Clip 执行时调用 `Default_Clip2ClipHandleFunc()`
2. 反射扫描所有程序集，查找继承 `ClipRunner<>` 的类型
3. 通过 `Utility.GetGenericType()` 提取泛型参数（即 Clip 类型）
4. 缓存到 `clipType2HandleType` 字典
5. 后续 Clip 执行直接查字典实例化对应 Runner

---

## 对象池 (`ObjectPool.cs`)

泛型对象池 `ObjectPool<T>`，用于复用 Runner 和 Handle 实例：
- `Get()` / `Release()` 获取/归还
- `Clear()` 由 `PoolableObject` 基类定义，Runner 的 `Clear()` 是 sealed
- 减少 GC 开销

---

## 如何扩展新的 Clip 类型

1. 创建新类继承 `Clip`，定义数据字段
2. 创建 `ClipRunner<YourClip>` 实现 `OnFrameUpdate()` 和 `OnDeltaUpdate()` 运行时逻辑
3. 可选: 创建 PreviewRunner 继承 `ClipRunner<YourClip>` 实现编辑器预览逻辑
4. 无需手动注册 — `Global.cs` 会通过反射自动发现映射

---

## RuntimeTemplateSequence

无需 ScriptableObject 资产的运行时临时序列：
- 直接在代码中构建 Track 和 Clip
- 通过 `RuntimeTemplateSequence.Runner` 执行（单 TrackRunner，存储于 `_subRunners[0]`）
- 适用于动态生成的过场或技能表演

---

## 常见问题排查

### 游标不移动
1. **`GetTimeScale()` 返回 0** — EditMode 下 `Time.timeScale = 0`，`SequenceRunner.GetTimeScale()` 已做 `#if UNITY_EDITOR` 守卫，EditMode 返回 1.0
2. **`ISequenceHandle.Time` 与 Runner 断开** — `SequenceDirector.SequenceHandle` 中显式接口 `float ISequenceHandle.Time` 必须转发到 `time`（小写属性），不能是独立自动属性
3. **OnEditorUpdate 在 PlayMode 被跳过** — 正常行为，PlayMode 由 `SequenceDirector.Update()` 驱动

### Runner 不推进（TotalTime 不变）
- 检查 `runnerState` 是否为 `Running`，`ManualUpdate` 中只有 Running 才调 `DriveUpdate`
- 检查 `_subRunners` 是否为 null

### ClipRunner 不执行
- 检查 `Sequence.FrameRateType` 是否正确，影响 `OutOfRange` 判断中的 SPF 值
- 检查 `TrackRunner.Sequence` 是否在 Reset 时赋值（`Sequence = sequence`）
