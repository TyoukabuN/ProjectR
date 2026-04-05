# Codex Review Skill

使用 OpenAI Codex CLI 审查 Qoder 生成的计划或代码。

## 功能

调用本地安装的 Codex CLI 对指定内容进行审查，返回 Codex 的分析结果。

## 使用方式

```
@codex-review <文件路径或内容>
```

## 示例

```
@codex-review plan.md
@codex-review "请审查以下方案：使用 Unity 的 DOTS 系统重构敌人 AI"
```

## 配置

确保 Codex CLI 已安装并在 PATH 中：
- 安装路径：`D:\CodexCLI\node_modules\.bin`
- 登录状态：已使用 ChatGPT 账号登录

## 参数

- `content`: 要审查的文件路径或直接内容
- `focus`: 审查重点（可选）
  - `architecture` - 架构设计
  - `performance` - 性能优化
  - `security` - 安全性
  - `completeness` - 完整性
