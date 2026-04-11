---
name: code-explains
description: 提供项目自定义系统的代码架构解释和实现细节。当处理 Timeline、Sequence、Track、Clip、Runner 相关代码时，或用户询问这些系统的架构、实现方式时使用。
---

# 代码架构解释

项目中自定义系统的代码架构参考文档，帮助快速理解代码结构和设计模式。

# 写作规范

**风格**：短章节、通俗易懂，每章节独立可读，不设独立“注意事项”段落，细节直接写进主流程或代码注释。

**文件大小控制**：单个文档不超过 300 行。内容较多时按主题拆成多个文件，每个文件对应一个独立关注点，通过索引文件串联。拆分粒度参考已有编号（如 04 生命周期、04a MonoEntity 生成）。

**结构模板**：

```
# 模块名 — 一句话说明

简短引言（1-2句，说明本文档覆盖什么）

---

## 章节一：概念/结构

[文字说明 + 必要时附层级图]

---

## 章节二：关键流程

[调用链伪代码或 csharp 代码块，说明执行顺序]

---

## 章节三：...

---

## 关键组件职责汇总

| 组件 | 职责 |
|------|------|
| ... | ... |
```

**代码块规范**：
- 调用链用缩进伪代码（`→` 符号）
- 具体实现用 `csharp` 代码块，保留关键注释
- 省略不相关的错误处理行

**禁止**：
- 不加独立 `⚠️ 注意` 段落，相关内容融入主流程
- 不写"以下将介绍……"等空话引言

## 写作流程

1. **先查文档**：用 grep 检查 codeExplains 目录里是否已有相关内容
2. **读代码**：读相关 .cs 文件，理清层级和调用关系
3. **确定文档位置**：
   - 已有文档可补充 → 在对应 .md 里增加章节
   - 新主题 → 新建独立 .md 文件（如 `state-machine.md`），并在本 SKILL.md 的"可用参考文档"中登记
4. **写文档**：按上方模板，优先写调用链和职责汇总
5. **更新登记**：新文件在本 SKILL.md 的"可用参考文档"中补充一条

## 可用参考文档

### Timeline 自定义序列系统
- 路径: `Assets/Scripts/Timeline/`
- 详细文档: [timeline-system.md](timeline-system.md)
- 核心模式: Runner 执行模式（SequenceAsset → Track → Clip 数据层 + SequenceRunner → TrackRunner → ClipRunner 执行层）
- 关键特性:
  - 不依赖 Unity PlayableGraph，自建数据模型和执行框架
  - 编辑器/运行时双路径（PreviewSequenceHandle vs RuntimeSequenceHandle）
  - 反射自动发现 Clip→ClipRunner 映射（Global.cs）
  - 帧率系统：Game(60fps) / HD(30fps) / Film(24fps)
  - 扩展新 Clip 只需继承 `Clip` + `ClipRunner<T>`，无需手动注册
