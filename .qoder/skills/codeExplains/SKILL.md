---
name: code-explains
description: 提供项目自定义系统的代码架构解释和实现细节。当处理 Timeline、Sequence、Track、Clip、Runner 相关代码时，或用户询问这些系统的架构、实现方式时使用。
---

# 代码架构解释

项目中自定义系统的代码架构参考文档，帮助快速理解代码结构和设计模式。

## 可用参考文档

### Timeline 自定义序列系统
- 路径: `Assets/Scripts/Timeline/`
- 详细文档: [timeline-system.md](timeline-system.md)
- 核心模式: Runner 执行模式（SequenceAsset → Track → Clip 数据层 + SequenceRunner → TrackRunner → ClipRunner 执行层）
- 关键特性:
  - 不依赖 Unity PlayableGraph，自建数据模型和执行框架
  - 编辑器/运行时双路径（SequenceHandle vs PreviewSequenceHandle）
  - 反射自动发现 Clip→ClipRunner 映射（Global.cs）
  - 帧率系统：Game(60fps) / HD(30fps) / Film(24fps)
  - 扩展新 Clip 只需继承 `Clip` + `ClipRunner<T>`，无需手动注册
