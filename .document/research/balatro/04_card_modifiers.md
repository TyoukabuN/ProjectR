# 04 卡牌修改器系统

[<< 上一章](03_jokers.md) · [返回索引](00_index.md) · [下一章 >>](05_consumables.md)

---

## 概述

**修改器（Modifiers）** 是可附加在普通牌和 Joker 上的升级属性，分为四类：
- **强化（Enhancements）**：附加在普通扑克牌上
- **印记（Seals）**：附加在普通扑克牌上
- **版本（Editions）**：附加在 Joker 和普通牌上
- **贴纸（Stickers）**：附加在 Joker 上（见 Joker 系统章节）

修改器一旦附加即永久生效（除版本外不可叠加同类）；附加新印记/强化会替换旧的。

---

## 强化（Enhancements）

共 8 种，通过塔罗牌、幻灵牌或增益包获得：

| 强化类型 | 图标 | 效果 |
|---|---|---|
| Bonus Card（加成牌）| 蓝色 | 计分时 +30 Chips |
| Mult Card（倍率牌）| 红色 | 计分时 +4 Mult |
| Wild Card（万能牌）| 多色 | 被视为所有花色同时存在 |
| Glass Card（玻璃牌）| 透明 | 计分时 ×2 Mult；1/4 概率计分后摧毁自身 |
| Steel Card（钢铁牌）| 银色 | 持在手中时 ×1.5 Mult（不需要打出）|
| Stone Card（石头牌）| 灰色 | 固定 +50 Chips，无点数/花色，始终计分 |
| Gold Card（黄金牌）| 金色 | 回合结束时若持在手中 +$3 |
| Lucky Card（幸运牌）| 绿色 | 1/5 概率 +20 Mult；1/15 概率 +$20（独立判定）|

**强化获取来源**：
- 塔罗牌（The Empress, The Hierophant, The Chariot 等）
- 幻灵牌（Familiar, Grim, Incantation 分别给面牌/A/数牌加随机强化）
- 增益包中的标准包（Standard Pack）
- Joker：Midas Mask（出牌时让花牌变 Gold）

---

## 印记（Seals）

共 4 种，附加在普通扑克牌上：

| 印记 | 颜色 | 效果 | 触发时机 |
|---|---|---|---|
| Gold Seal（金印）| 金色 | 该牌打出并计分时 +$3 | 计分时 |
| Red Seal（红印）| 红色 | 该牌所有效果再触发 1 次（包括手牌持有效果）| 计分时/持有时 |
| Blue Seal（蓝印）| 蓝色 | 回合末若持在手中，生成最后打出牌型对应的星球牌 | 回合结束 |
| Purple Seal（紫印）| 紫色 | 弃牌时生成一张塔罗牌（需有空位）| 弃牌时 |

**印记获取来源**：
- 幻灵牌（Talisman→金, Deja Vu→红, Trance→蓝, Medium→紫）
- 高难度增益包（Standard Pack 中的牌可能带印记）

---

## 版本（Editions）

版本可附加在 Joker 和普通牌上，效果有区别：

| 版本 | 普通牌效果 | Joker 效果 | 消耗品效果 |
|---|---|---|---|
| Base（基础）| 无 | 无 | 无 |
| Foil（铂金箔）| 计分时 +50 Chips | Joker 计分前 +50 Chips | N/A |
| Holographic（全息）| 计分时 +10 Mult | Joker 计分前 +10 Mult | N/A |
| Polychrome（多彩）| 计分时 ×1.5 Mult | Joker 计分**后** ×1.5 Mult | N/A |
| Negative（负面）| N/A | +1 Joker 槽位 | +1 消耗品槽位 |

**版本附加概率（无 Voucher）**：
- Joker：Negative 0.3% / Polychrome 0.3% / Holo 1.4% / Foil 2%
- 普通牌：Poly 1.2% / Holo 2.8% / Foil 4%

**版本获取方式**：
- 商店中自然生成（概率低）
- The Wheel of Fortune 塔罗牌（1/4 概率给随机 Joker 附加 Foil/Holo/Poly）
- Aura 幻灵牌（给手中选定牌附加 Foil/Holo/Poly）
- Hone / Glow Up Voucher（提升版本出现概率 2×/4×）
- Edition Tag（跳 Blind 奖励）

---

## 修改器出现概率（增益包中的普通牌）

| Voucher 状态 | 有强化概率 | 有印记概率 | Poly | Holo | Foil |
|---|---|---|---|---|---|
| 无 | 40% | 20% | 1.2% | 2.8% | 4% |
| Hone | 40% | 20% | 2.4% | 5.6% | 8% |
| Glow Up | 40% | 20% | 4.8% | 11.2% | 16% |

> 注：Illusion Voucher 让商店直接出售带强化的普通牌，但目前有 bug（不会给印记）

---

## 关键组合与策略

| 组合 | 策略说明 |
|---|---|
| Glass Card + Retrigger | 玻璃牌再触发即 ×2×2=×4 Mult，但有摧毁风险 |
| Steel Card + Baron Joker | Baron 让持手中的 K 给 ×1.5，Steel Card 也要持手中，两者协同 |
| Wild Card + Flush Build | 万能牌让任意花色都算同花，大幅降低 Flush 难度 |
| Stone Card + Splash Joker | Stone Card 无花色点数但始终计分，Splash 让所有牌都算分 |
| Purple Seal + Cartomancer | 弃牌触发生成塔罗牌，配合 Cartomancer Joker 实现循环 |
| Blue Seal + Telescope Voucher | 始终生成最多打出牌型对应星球牌，专精路线加速升级 |
| Lucky Card + 高倍率手 | 1/5 的 +20 Mult 在高 Chips 手中爆发效果显著 |

---

## 设计启示

1. **多层修改器叠加系统**：强化+印记+版本三层可以同时存在于同一张牌，形成"一张牌变成强力单元"的可能，是构建深度的重要来源。
2. **风险-收益权衡**：Glass Card（高收益但有摧毁风险）是优秀的风险机制设计，玩家需要在"利用"与"保护"之间权衡。
3. **持有策略 vs. 打出策略**：Steel/Gold Card 奖励持在手中而非打出，为"最优牌组管理"增加了额外维度，不是单纯出大牌就好。
4. **印记与弃牌策略**：Purple Seal 让弃牌行为变得有价值（生成消耗品），改变了"弃牌只是工具"的认知，形成新策略层次。

---

[<< 上一章](03_jokers.md) · [返回索引](00_index.md) · [下一章 >>](05_consumables.md)
