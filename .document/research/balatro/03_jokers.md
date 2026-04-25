# 03 Joker 系统详解

[<< 上一章](02_scoring.md) · [返回索引](00_index.md) · [下一章 >>](04_card_modifiers.md)

---

## 概述

Joker 是 Balatro 的核心机制。共 **150 张**（截至 v1.0.1o-FULL），提供被动效果，不参与出牌，放置在专用的 Joker 槽位。

- 默认 **5 个** Joker 槽位（部分牌组/Joker 可增减）
- 开局可用 105 张，其余 45 张需满足特定条件解锁
- 出售/摧毁 Joker 时，卖出价 = 购买价的一半
- 赢得 Boss Blind 时，Joker 获得对应难度颜色贴纸

---

## 稀有度（Rarities）

| 稀有度 | 数量 | 商店出现概率 | 价格范围 |
|---|---|---|---|
| Common（普通）| 61 张 | 70% | $1–6 |
| Uncommon（罕见）| 64 张 | 25% | $4–8 |
| Rare（稀有）| 20 张 | 5% | $7–10 |
| Legendary（传说）| 5 张 | 仅通过 The Soul 幻灵牌 | $20 |

> Legendary Joker 只能通过 The Soul 幻灵牌（出现概率 0.3%）获得

---

## Joker 版本（Editions）

版本是附加在 Joker 上的额外属性，增加商店价格：

| 版本 | 效果 | 触发时机 | 附加价格 |
|---|---|---|---|
| Base（基础）| 无额外效果 | — | 无 |
| Foil（铂金箔）| +50 Chips | Joker 计分前触发 | +$2 |
| Holographic（全息）| +10 Mult | Joker 计分前触发 | +$3 |
| Polychrome（多彩）| ×1.5 Mult | Joker 计分**后**触发 | +$5 |
| Negative（负面）| +1 Joker 槽位 | 被动 | +$5 |

---

## Joker 贴纸（Stickers）

高难度下，Joker 在商店出现时可能附有贴纸，改变 Joker 行为：

| 贴纸 | 效果 | 出现概率 |
|---|---|---|
| Eternal（永久）| 无法出售或被摧毁 | 黑注 30% |
| Perishable（易腐）| 5 轮后被 Debuff（失效）| 橙注 30% |
| Rental（租赁）| 每轮结束花费 $3，可以 $1 购买 | 金注 30% |

---

## 关键术语解析

理解这些术语是读懂 Joker 效果的基础：

| 术语 | 含义 |
|---|---|
| When Scored（计分时）| 对应牌被计入牌型时触发，Debuff 状态不触发 |
| Contains（包含）| 打出的手牌中包含指定牌型（如 3K 包含 Pair）|
| Is（是）| 整手牌恰好是指定牌型（而非更高级别）|
| When Blind is Selected（选择盲注时）| 进入盲注时触发，跳过不触发 |
| In Deck（在牌库中）| 指当前牌库中的牌（不含手牌/已弃/已出）|
| In Full Deck（在完整牌库中）| 包含所有位置的牌 |
| Add（加入）| 永久将一张牌加入牌组 |
| Destroy（摧毁）| 永久移除，不获得卖出价 |
| Create（创建）| 生成一张新牌（需有空位） |
| Gains（获得）| 为目标牌添加永久效果 |
| Retrigger（再触发）| 让一张牌再次计分一次 |
| Debuffed（Debuff状态）| 该 Joker 及其版本效果全部失效 |

---

## Joker 效果分类

### 按触发时机分类

| 分类 | 描述 | 示例 |
|---|---|---|
| 出牌时（On Played）| 出牌行为本身触发 | Midas Mask（出牌时把花牌变 Gold）|
| 计分时（On Scored）| 参与计分的牌触发 | Jolly Joker（包含 Pair 时 +8 Mult）|
| 手牌时（On Held）| 持有在手牌时触发 | Baron（手中每张 K 给 ×1.5 Mult）|
| 弃牌时（On Discard）| 弃牌行为触发 | Burnt Joker（升级第一次弃掉的牌型）|
| 选盲注时（On Blind Select）| 选择盲注时触发 | Ceremonial Dagger（摧毁右侧 Joker 获倍率）|
| 独立（Independent）| 条件满足即触发，不依赖出牌 | Gros Michel（+15 Mult，1/6 概率被摧毁）|
| 被动（Passive）| 全程生效的固定效果 | Splash（所有打出的牌都计分）|

### 按效果类型分类

| 类型 | 代表 Joker |
|---|---|
| 加法 Mult | Jolly Joker (+8 Mult if Pair)，Gros Michel (+15 Mult) |
| 乘法 Mult | Obelisk（连续不打最常用牌型 ×0.2/次）|
| Chips | Blue Joker（每张剩余牌 +2 Chips）|
| 经济 | Golden Joker（每轮末 +$4），Bull（每 $1 持有 +2 Chips）|
| 工具/被动 | Splash, Four Fingers, Shortcut（允许有缺口的 Straight）|
| 再触发 | Seltzer（接下来 10 手所有牌再触发），Red Seal 效果加成 |
| 创建 | Hallucination（打开增益包 1/2 概率生成 Tarot）|

---

## 知名 Joker 示例（设计参考）

| Joker | 类型 | 效果 | 设计亮点 |
|---|---|---|---|
| Obelisk | 乘法 Mult | 连续不打最常用牌型，每手 ×0.2 Mult | 反常规操作，营造多样化策略 |
| Joker Stencil | 乘法 Mult | 每个空 Joker 槽 ×1 Mult（含自身）| 鼓励少装 Joker，反直觉 |
| Baron | 乘法 Mult | 手中每张 K 给 ×1.5 Mult | "持有而非打出"策略 |
| Riff-Raff | 创建 | 选 Blind 时生成 2 张 Common Joker | 扩展 Joker 数量 |
| Perkeo | 创建 | 离开商店时复制 1 张消耗品（Negative 版本）| 消耗品复制爆发 |
| Brainstorm | 复制 | 复制最左侧 Joker 的效果 | 双份触发 |
| Blueprint | 复制 | 复制右侧 Joker 的效果 | 组合链式触发 |
| Chicot | 传说 | 禁用所有 Boss Blind 特殊效果 | 最强通关保障 |

---

## 设计启示

1. **Joker 协同是核心乐趣**：Joker 效果之间的搭配产生"1+1>2"的爆发感，设计时应明确哪些机制可以形成"飞轮"，哪些是"触发链"。
2. **稀有度不等于强度**：游戏明确说明稀有度高不一定更强，部分 Common Joker 在特定 Build 中是核心。这避免了"稀有 = 更好"的刻板印象。
3. **反直觉 Joker 设计**：Joker Stencil（少 Joker 更强）、Obelisk（不用主力牌型反而更强）等都是反常规设计，给熟练玩家带来惊喜发现感。
4. **传说稀有度保持神秘**：Legendary Joker 无法从商店获得，只能通过特定幻灵牌碰运气，制造了强烈的"彩票时刻"。
5. **版本叠加放大上限**：Polychrome + 高 +Mult 积累可以产生巨量分数，形成"投资 Joker 版本升级"的策略层次。

---

[<< 上一章](02_scoring.md) · [返回索引](00_index.md) · [下一章 >>](04_card_modifiers.md)
