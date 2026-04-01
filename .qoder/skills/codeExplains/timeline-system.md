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
│   ├── SequenceDirector.SequenceHandle.cs       # 运行时播放句柄
│   ├── SequenceDirector.SequencePreviewHandle.cs # 编辑器预览句柄
│   ├── ISequenceHandle.cs           # 句柄接口
│   ├── ISequenceHolder.cs           # 持有序列的对象接口
│   ├── IErrorRecorder.cs            # 错误记录接口
│   ├── RuntimeTemplateSequence.cs        # 无资产的运行时临时序列
│   ├── RuntimeTemplateSequence.Runner.cs # 对应 Runner
│   ├── Unit/
│   │   ├── SequenceAsset.cs         # 序列资产（ScriptableObject）
│   │   ├── SequenceAsset.Runner.cs  # 序列执行器（多轨协调）
│   │   ├── SequenceRunner.cs        # Runner 抽象基类
│   │   ├── Track.cs                 # 轨道（Clip 容器）
│   │   ├── TrackRunner.cs           # 轨道执行器
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
│   ├── TimelineWindow.WindowState.cs        # 窗口状态管理
│   ├── TimelineWindow.Defines.cs            # 窗口常量
│   ├── TimelineWindow.EventCallback.cs      # 事件回调
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
SequenceRunner (管理整个序列)
  └── TrackRunner[] (每轨道一个)
        └── ClipRunner[] (每 Clip 一个)
```

### 状态流转
```
None → Running → Done / Paused / Failure → Disposed
```
Runner 状态通过 `OnStateChanged` Action 触发外部监听回调。

---

## 关键类说明

### `Define.cs` — 核心定义
- `ERunnerState`: Runner 状态枚举 (None/Running/Done/Paused/Failure/Disposed)
- `EFrameRate`: 帧率枚举 — Game(60fps)、HD(30fps)、Film(24fps)
- `UpdateContext`: 更新上下文结构体，包含 deltaTime、intervalType、frameRate
- `TimeFormat`: 时间格式化相关

### `Global.cs` — 类型自动映射
- `Clip2ClipHandleFunc`: Clip 类型→ClipRunner 类型的映射委托
- `Default_Clip2ClipHandleFunc()`: 反射扫描所有程序集，找到继承 `ClipRunner<TClip>` 的类型，自动建立映射缓存
- 首次执行 Clip 时自动触发，后续使用缓存

### `SequenceDirector.cs` — 播放控制入口
- MonoBehaviour，挂载在 GameObject 上
- 持有 `SequenceAsset` 引用
- 支持 `PlayOnAwake`
- `GetRunner()` 创建 SequenceRunner
- `ManualUpdate(deltaTime)` 驱动 Runner 更新
- 通过 `ISequenceHandle` 区分运行时/编辑器预览

### `SequenceAsset.cs` — 序列资产
- ScriptableObject，定义帧率和轨道列表
- `GetRunner(gameObject)` 创建对应 SequenceRunner
- 通过 `AddTrack()` / `RemoveTrack()` 管理轨道

### `SequenceRunner.cs` — 序列执行器基类
- 抽象类，管理状态转换和时间追踪
- `totalLength`: 序列总时长
- `normalizedTime`: 归一化进度 (0~1)
- 生命周期: `OnStart()` → `OnUpdate()` → `OnEnd()`

### `SequenceAsset.Runner.cs` — 序列资产执行器
- 继承 SequenceRunner
- `OnStart()`: 初始化所有 TrackRunner
- `OnUpdate()`: 按 deltaTime 缩放后创建 UpdateContext，驱动各 TrackRunner
- 支持 `force` 参数强制刷新（编辑器预览拖动时使用）

### `Track.cs` — 轨道
- Serializable ScriptableObject
- 包含 `Clip[]` 列表和所属 SequenceAsset 引用
- 提供 `AddClip()` / `RemoveClip()` 管理

### `TrackRunner.cs` — 轨道执行器
- 管理单轨道内所有 ClipRunner 的执行
- `OnUpdate(context)`: 遍历 Clip，判断时间范围，调用 ClipRunner 的 Start/Update/End
- 实现 `IErrorRecorder` 记录执行错误

### `Clip.cs` — Clip 抽象基类
- 定义时间信息: `startTime`/`endTime`（秒），`startFrame`/`endFrame`（帧）
- `GetRunner()`: 返回运行时 ClipRunner
- `GetPreviewRunner()`: 返回编辑器预览 ClipRunner
- 实现 `ISequenceUnit` 提供 Track/SequenceAsset 引用

### `ClipRunner.cs` — ClipRunner 抽象基类
- 泛型 `ClipRunner<TClip>` 关联具体 Clip 类型
- 生命周期: `OnStart()` → `OnUpdate(context)` → `OnEnd()`
- 持有 `clip` 引用和 `gameObject` 引用

### `AnimancerClip` — 动画 Clip 实现
- **AnimancerClip.cs**: 数据层，持有 `AnimationClip` 引用和速度等参数
- **AnimancerClip.Runner.cs**: 运行时用 AnimancerComponent 播放动画
- **AnimancerClip.PreviewRunner.cs**: 编辑器用 `AnimationClip.SampleAnimation()` 采样预览
- **AnimancerClip.Extension.cs**: 静态方法快捷创建运行时 AnimancerClip

---

## 编辑器-运行时双路径

| 场景 | Handle 类型 | ClipRunner 来源 |
|------|------------|----------------|
| 运行时播放 | `SequenceHandle` | `Clip.GetRunner()` |
| 编辑器预览 | `PreviewSequenceHandle` | `Clip.GetPreviewRunner()` |

- `SequenceHandle`: 在 Play Mode 下驱动实际 Runner
- `PreviewSequenceHandle`: 在 Edit Mode 下可拖动时间轴预览，通过 `ManualUpdateDirector(0, true)` 强制单帧刷新

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
2. `GetRunner()` → `SequenceAsset.GetRunner(gameObject)` 创建 SequenceRunner
3. `SequenceRunner.OnStart()` → 初始化所有 TrackRunner
4. 每帧 `SequenceDirector.Update()` → `ManualUpdate(deltaTime)`
5. `SequenceRunner.OnUpdate(deltaTime)` → 缩放 deltaTime → 创建 UpdateContext
6. 各 `TrackRunner.OnUpdate(context)` → 判断各 Clip 时间范围
7. 进入范围: `ClipRunner.OnStart()` → `ClipRunner.OnUpdate()`
8. 超出范围: `ClipRunner.OnEnd()`
9. 所有轨道完成 → SequenceRunner 状态变为 Done

---

## 编辑器预览流程

1. 双击 SequenceAsset → TimelineWindow 打开
2. 窗口检测到选中 SequenceAsset
3. 创建 `PreviewSequenceHandle`，使用 PreviewRunner
4. 拖动时间轴游标 → 设置 `State.Time`
5. `ManualUpdateDirector(0, true)` 强制刷新一帧
6. ClipDrawer 绘制 Clip 矩形，PreviewRunner 采样动画

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
- 支持 `IDisposable`
- 减少 GC 开销

---

## 如何扩展新的 Clip 类型

1. 创建新类继承 `Clip`，定义数据字段
2. 创建 `ClipRunner<YourClip>` 实现运行时逻辑
3. 可选: 创建 PreviewRunner 实现编辑器预览逻辑
4. 无需手动注册 — `Global.cs` 会通过反射自动发现映射

---

## RuntimeTemplateSequence

无需 ScriptableObject 资产的运行时临时序列：
- 直接在代码中构建 Track 和 Clip
- 通过 `RuntimeTemplateSequence.Runner` 执行
- 适用于动态生成的过场或技能表演
