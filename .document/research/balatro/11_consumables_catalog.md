# Balatro 消耗品完整图鉴

> 来源：balatrowiki.org | 版本：v1.0.1o-FULL | 整理：2026.04.26

---

**导航**：[00 索引](00_index.md) | [01 概述](01_overview.md) | [02 评分](02_scoring.md) | [03 Joker系统](03_jokers.md) | [04 卡牌修改器](04_card_modifiers.md) | [05 消耗品](05_consumables.md) | [06 商店经济](06_shop_economy.md) | [07 卡组难度](07_decks_stakes.md) | [08 设计启示](08_design_insights.md) | [10 Joker图鉴](10_jokers_catalog.md) | **[11 消耗品图鉴]**

---

## 目录
1. [消耗品系统概述](#1-消耗品系统概述)
2. [塔罗牌（22张）](#2-塔罗牌22张)
3. [星球牌（12张）](#3-星球牌12张)
4. [幻灵牌（18张）](#4-幻灵牌18张)
5. [数值分析备注](#5-数值分析备注)

---

## 1. 消耗品系统概述

| 属性 | 塔罗牌 | 星球牌 | 幻灵牌 |
|------|--------|--------|--------|
| 数量 | 22张 | 12张（含3张隐藏）| 18张 |
| 主要作用 | 修改牌库中的牌 | 升级扑克牌型等级 | 高风险特殊效果 |
| 获取方式 | Arcana Pack、商店、紫印记弃牌、特定Joker | Celestial Pack、蓝印记手持、商店、特定Joker | Spectral Pack（$4/pack，约1个/2关）|
| 商店单价 | $3 | $3 | 仅礼包形式（不直接出现于商店）|
| 使用限制 | 需目标牌在手中或有空位 | 无限制（立即生效）| 通常有代价 |
| 卡槽 | 共用消耗品槽（初始2个）| 共用消耗品槽 | 共用消耗品槽 |

**来源关键Joker**：
- 塔罗生成：8 Ball、Superposition、Cartomancer、Vagabond、Hallucination、Purple Seal
- 星球生成：The High Priestess（塔罗效果）、Blue Seal（手持触发）、Perkeo（Negative副本）
- 幻灵生成：Sixth Sense、Séance

---

## 2. 塔罗牌（22张）

> 全部初始可用（首次购买/使用后发现）。发现全22张解锁 **Cartomancer** Joker。
> 商店单价 $3。如全22张均被封锁，则强制生成 Strength。

### 2.1 增强类（Enhancement）—— 修改选中牌的类型

| 名称 | 效果 | 可选牌数 | 赋予的增强类型 |
|------|------|----------|----------------|
| The Magician | 将选中牌增强为**幸运牌（Lucky Card）** | 2张 | +20 Chips（1/5概率）+$20（1/15概率）|
| The Empress | 将选中牌增强为**Mult牌** | 2张 | +4 Mult（当评分时）|
| The Hierophant | 将选中牌增强为**Bonus牌** | 2张 | +30 Chips（当评分时）|
| The Lovers | 将选中牌增强为**Wild牌** | 1张 | 可充当任意花色 |
| The Chariot | 将选中牌增强为**Steel牌** | 1张 | 手持时X1.5 Mult |
| Justice | 将选中牌增强为**Glass牌** | 1张 | X2 Mult（评分时）；1/4概率打碎销毁 |
| The Devil | 将选中牌增强为**Gold牌** | 1张 | 手持并在轮末时+$3 |
| The Tower | 将选中牌增强为**Stone牌** | 1张 | +50 Chips（无论花色点数）|

### 2.2 花色转换类

| 名称 | 效果 | 可选牌数 | 目标花色 |
|------|------|----------|----------|
| The Star | 将选中牌转为**方块（♦）** | 3张 | Diamonds |
| The Moon | 将选中牌转为**梅花（♣）** | 3张 | Clubs |
| The Sun | 将选中牌转为**红心（♥）** | 3张 | Hearts |
| The World | 将选中牌转为**黑桃（♠）** | 3张 | Spades |

### 2.3 点数/牌库操作类

| 名称 | 效果 | 备注 |
|------|------|------|
| Strength | 将选中最多2张牌点数+1（2→3→…→A→2循环）| 可叠加使用 |
| The Hanged Man | 销毁最多2张选中牌（永久从牌库移除）| 瘦牌库核心操作 |
| Death | 选2张牌，将左牌复制为右牌（可拖拽调序）| 复制特定牌型 |

### 2.4 生成类

| 名称 | 效果 | 条件 |
|------|------|------|
| The High Priestess | 生成最多2张随机**星球牌** | 需有消耗品空位 |
| The Emperor | 生成最多2张随机**塔罗牌** | 需有消耗品空位 |
| Judgement | 生成1张随机**Joker牌** | 需有Joker空位 |
| The Fool | 生成本局上一张使用的塔罗/星球牌（The Fool本身除外）| 无额外条件 |

### 2.5 经济类

| 名称 | 效果 | 上限 |
|------|------|------|
| The Hermit | 金钱翻倍 | 上限+$20（即最多从$0到$20）|
| Temperance | 获得当前所有Joker卖出价值之和 | 上限$50 |
| The Wheel of Fortune | 1/4概率给随机Joker添加Foil/Holo/Poly版本 | 无上限（随机）|

### 2.6 增强效率对比

| 增强类型 | 单张价值 | 最佳搭配 |
|----------|----------|----------|
| Lucky Card | 期望值约+4 Chips+$1.33（概率性）| Bloodstone/Lucky Cat |
| Mult Card | +4 Mult（评分时）| 高频出牌型Build |
| Bonus Card | +30 Chips | Chips叠加型Build |
| Wild Card | 花色灵活性 | Flower Pot/Blackboard |
| Steel Card | X1.5 Mult（手持）| Mime + Steel Joker |
| Glass Card | X2 Mult（50%破碎）| 高风险高收益 |
| Gold Card | +$3/轮（手持）| 经济型Build |
| Stone Card | +50 Chips（无条件）| Chips流 |

---

## 3. 星球牌（12张）

> 9张初始可用（正常游戏可获取）；3张**隐藏**（需先在本局打出对应牌型才能在商店出现）。
> 发现全12张解锁 **Astronomer** Joker（所有星球牌和天体礼包免费）。
> 每张星球牌提升对应牌型1级（无上限）。

### 3.1 完整列表

| 名称 | 对应牌型 | 每级+Chips | 每级+Mult | 基础值（Level 1） | 状态 |
|------|----------|-----------|----------|------------------|------|
| Pluto | High Card（单张高牌）| +10 | +1 | Chips 5, Mult 1 | 初始可用 |
| Mercury | Pair（对子）| +15 | +1 | Chips 10, Mult 2 | 初始可用 |
| Uranus | Two Pair（两对）| +20 | +1 | Chips 20, Mult 2 | 初始可用 |
| Venus | Three of a Kind（三条）| +20 | +2 | Chips 30, Mult 3 | 初始可用 |
| Saturn | Straight（顺子）| +30 | +3 | Chips 30, Mult 4 | 初始可用 |
| Jupiter | Flush（同花）| +15 | +2 | Chips 35, Mult 4 | 初始可用 |
| Earth | Full House（葫芦）| +25 | +2 | Chips 40, Mult 4 | 初始可用 |
| Mars | Four of a Kind（四条）| +30 | +3 | Chips 60, Mult 7 | 初始可用 |
| Neptune | Straight Flush（同花顺）| +40 | +4 | Chips 100, Mult 8 | 初始可用 |
| Planet X | Five of a Kind（五条）| +35 | +3 | Chips 35, Mult 3 | **隐藏** |
| Ceres | Flush House（同花葫芦）| +40 | +4 | Chips 40, Mult 4 | **隐藏** |
| Eris | Flush Five（同花五条）| +50 | +3 | Chips 35, Mult 3 | **隐藏** |

### 3.2 升级收益分析

> 规律：**越难打出的牌型，星球牌每级提升越多**

| 难度层级 | 牌型 | 每级Chips增量 | 每级Mult增量 |
|----------|------|--------------|-------------|
| 低（易打）| Pair / High Card | +15 / +10 | +1 / +1 |
| 中 | Three of a Kind / Two Pair | +20 / +20 | +2 / +1 |
| 中高 | Straight / Flush / Full House | +30 / +15 / +25 | +3 / +2 / +2 |
| 高 | Four of a Kind / Straight Flush | +30 / +40 | +3 / +4 |
| 极高（秘密）| Five of a Kind / Flush Five / Flush House | +35 / +50 / +40 | +3 / +3 / +4 |

**关键数值参考**：Flush（同花）初始 35 Chips × 4 Mult = 基础 140 分（在乘法爆炸前）

### 3.3 专属联动

- **Telescope Voucher**：天体礼包永远包含当前最常用牌型对应星球牌
- **Observatory Voucher**（升级版）：消耗品槽中的星球牌给对应牌型X1.5 Mult
- **Constellation Joker**：每使用1张星球牌+X0.1 Mult（多用星球牌 Build 的核心）
- **Blue Seal**：手持此印记的牌，轮末生成本轮最后出手牌型对应星球牌

---

## 4. 幻灵牌（18张）

> 通过 **Spectral Packs**（幻灵礼包）获取，不直接出现在商店（除非使用 Ghost Deck）。
> 效果通常比塔罗强力但伴有代价（销毁牌、缩手牌、归零金钱等）。
> 全18张均初始可用。若18张均在玩家手中，强制生成 Incantation。

### 4.1 牌库强化类（增强牌 + 销毁替换）

| 名称 | 效果 | 代价 | 净效果分析 |
|------|------|------|------------|
| Familiar | 销毁手中随机1张牌，加入3张随机**增强面牌** | 销毁1张 | 净+2张，专攻面牌Build |
| Grim | 销毁手中随机1张牌，加入2张随机**增强A** | 销毁1张 | 净+1张A，Ace-heavy Build |
| Incantation | 销毁手中随机1张牌，加入4张随机**增强数字牌** | 销毁1张 | 净+3张数字牌 |

### 4.2 印记添加类

| 名称 | 效果 | 印记效果回顾 |
|------|------|--------------|
| Talisman | 给手中选1张牌添加**金印记（Gold Seal）** | 手持时每轮+$3 |
| Deja Vu | 给手中选1张牌添加**红印记（Red Seal）** | 该牌追加触发1次 |
| Trance | 给手中选1张牌添加**蓝印记（Blue Seal）** | 轮末手持时生成对应星球牌 |
| Medium | 给手中选1张牌添加**紫印记（Purple Seal）** | 弃牌时生成塔罗牌 |

### 4.3 版本（Edition）添加类

| 名称 | 效果 | 代价 |
|------|------|------|
| Aura | 给手中选1张牌添加 Foil/Holo/Polychrome版本（随机）| 无 |
| Hex | 给随机1张Joker添加Polychrome版本，**销毁所有其他Joker** | 高风险！|
| Ectoplasm | 给随机1张Joker添加Negative版本（+1槽位），**手牌上限-1** | 永久代价 |

### 4.4 牌库操作类

| 名称 | 效果 | 代价 |
|------|------|------|
| Sigil | 将手中所有牌转为随机**单一花色** | 无 |
| Ouija | 将手中所有牌转为随机**单一点数**，手牌上限-1 | 永久-1手牌 |
| Cryptid | 将手中选1张牌复制2份并加入牌库 | 无（但增加牌库体量）|
| Ankh | 复制随机1张Joker，**销毁所有其他Joker**（副本移除Negative）| 极高风险！|
| Immolate | 销毁手中随机5张牌，获得+$20 | 损失牌库 |

### 4.5 全局效果类

| 名称 | 效果 | 代价 |
|------|------|------|
| The Soul | 生成1张随机**传说Joker** | 需有Joker空位；0.3%出现概率 |
| Black Hole | 所有扑克牌型等级+1（全部升级）| 无代价！|
| Wraith | 生成随机1张**Rare Joker**，当前金钱归零 | 金钱清零 |

### 4.6 使用决策参考

| 代价等级 | 幻灵牌 | 使用建议 |
|----------|--------|----------|
| 无代价 | Talisman/Deja Vu/Trance/Medium/Aura/Sigil/Black Hole | 优先使用 |
| 轻代价（销毁1张牌）| Familiar/Grim/Incantation | 用废牌换强牌时使用 |
| 中代价（统一花色/点数）| Sigil/Ouija | 有花色/点数Build时考虑 |
| 高代价（金钱归零）| Wraith | 前期/无钱可花时使用最优 |
| 极高代价（销毁所有Joker）| Hex/Ankh | 仅在特定Joker极强时使用 |
| 持续代价（-1手牌/弃牌）| Ectoplasm/Ouija | 慎用，评估长期影响 |

---

## 5. 数值分析备注

### 5.1 消耗品价值层级

```
Black Hole（全牌型+1级）≈ The Soul（传说Joker）>> Wraith（稀有Joker）
> Hex（Poly Joker + 清场）> Ankh（Joker复制 + 清场）
> Familiar/Grim/Incantation（牌库强化）
> 花色转换系列（3张/次，Build支持）
> 增强添加系列（1-2张）> 点数升高/Strength
```

### 5.2 星球牌使用节奏

| 局面 | 推荐升级顺序 | 原因 |
|------|-------------|------|
| 前期（Ante 1-3）| 最常用牌型（Pair/Flush）| 稳定得分，渡过早期 |
| 中期（Ante 4-6）| 当前Build主力牌型 | 乘法效应开始显现 |
| 后期（Ante 7-8）| 最高倍率牌型（Flush Five等）| 满足指数级得分需求 |

### 5.3 塔罗牌使用优先级（通用型）

1. **The Hanged Man**：瘦牌库核心，销毁废牌提升触发率
2. **Death**：复制关键牌（如已有Steel/Glass牌）
3. **花色转换**（The Star/Moon/Sun/World）：支持同花Build
4. **Strength**：点数+1，适配奇偶数/特定点数Build
5. **Temperance/The Hermit**：经济补充
6. **增强添加类**：视Build方向选择

### 5.4 幻灵牌风险矩阵

| 时机 | 推荐幻灵牌 | 原因 |
|------|-----------|------|
| 游戏开始，Joker弱 | Wraith（清零金钱可接受）| 早期钱少，代价小 |
| 有5张强Joker | Ankh/Hex慎用 | 只有在其中一张远超其他时才考虑 |
| 牌库已有增强牌 | Cryptid（复制关键增强牌）| 无代价扩大强牌数量 |
| 前期空印记牌 | Talisman/Medium | 建立经济/弃牌联动 |

---

*数据来源：[balatrowiki.org/w/Consumables](https://balatrowiki.org/w/Consumables) | [/w/Tarot_Cards](https://balatrowiki.org/w/Tarot_Cards) | [/w/Planet_Cards](https://balatrowiki.org/w/Planet_Cards) | [/w/Spectral_Cards](https://balatrowiki.org/w/Spectral_Cards)*
