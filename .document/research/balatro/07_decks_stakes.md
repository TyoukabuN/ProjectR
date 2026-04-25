# 07 卡组与难度系统

[<< 上一章](06_shop_economy.md) · [返回索引](00_index.md) · [下一章 >>](08_design_insights.md)

---

## 卡组（Decks）概述

共 **15 种普通卡组** + **20 种挑战卡组**，在开局时选择，决定整局的初始条件和特殊规则。

高难度（Stake）的解锁与卡组绑定——每个卡组需要**单独**从白注逐级完成，才能解锁更高难度。

---

## 15 种普通卡组

| 编号 | 卡组名 | 特殊效果 | 解锁条件 |
|---|---|---|---|
| 1 | **Red Deck** | 每轮 +1 弃牌数 | 初始解锁 |
| 2 | **Blue Deck** | 每轮 +1 手数 | 收集 20 项收藏 |
| 3 | **Yellow Deck** | 开局 +$10 | 收集 50 项收藏 |
| 4 | **Green Deck** | 每轮结束：每剩 1 手 +$2，每剩 1 弃 +$1；无利息 | 收集 75 项收藏 |
| 5 | **Black Deck** | +1 Joker 槽位，每轮 -1 手数 | 收集 100 项收藏 |
| 6 | **Magic Deck** | 开局持有 Crystal Ball Voucher + 2 张 The Fool | 用任意卡组赢一局 |
| 7 | **Nebula Deck** | 开局持有 Telescope Voucher；-1 消耗品槽 | 用 Blue Deck 赢一局 |
| 8 | **Ghost Deck** | 商店可出现幻灵牌；开局持有 Hex | 用 Yellow Deck 赢一局 |
| 9 | **Abandoned Deck** | 开局牌组无面牌（无 J/Q/K）| 用 Green Deck 赢一局 |
| 10 | **Checkered Deck** | 开局 26 张黑桃 + 26 张红心（去掉方块梅花）| 用 Black Deck 赢一局 |
| 11 | **Zodiac Deck** | 开局持有 Tarot Merchant + Planet Merchant + Overstock | 在红注或更高难度赢一局 |
| 12 | **Painted Deck** | +2 手牌上限，-1 Joker 槽位 | 在绿注或更高难度赢一局 |
| 13 | **Anaglyph Deck** | 击败每个 Boss Blind 后获得 Double Tag | 在黑注或更高难度赢一局 |
| 14 | **Plasma Deck** | 计算分数时平衡 Chips 和 Mult（取均值）；盲注需求 ×2 | 在蓝注或更高难度赢一局 |
| 15 | **Erratic Deck** | 开局所有牌的点数和花色随机化 | 在橙注或更高难度赢一局 |

---

## 卡组设计分析

### 资源导向卡组

| 卡组 | 资源优势 | 代价 |
|---|---|---|
| Blue Deck | +1 手数（更多容错）| 无 |
| Red Deck | +1 弃牌（更多过牌）| 无 |
| Yellow Deck | +$10 开局 | 无 |
| Black Deck | +1 Joker 槽（多装一个 Joker）| -1 手数 |

### 策略导向卡组

| 卡组 | 强制策略方向 |
|---|---|
| Magic Deck | The Fool 复制 → 消耗品循环流 |
| Nebula Deck | Telescope → 专精单一牌型流 |
| Ghost Deck | 幻灵牌高风险流 |
| Abandoned Deck | 无面牌 → 数牌/A 特化 |
| Checkered Deck | 双花色 → Flush 特化 |
| Zodiac Deck | 消耗品强化流 |
| Painted Deck | 大手牌 → 宽牌型 |
| Anaglyph Deck | Double Tag 组合爆发 |
| Plasma Deck | Chips/Mult 平衡型（x√(Chips×Mult)）|
| Erratic Deck | 随机挑战，高难体验 |

---

## 8 种难度（Stakes）

难度是**累加**的——每个更高难度包含前面所有难度的限制。

| 难度 | 颜色 | 追加效果 | 解锁卡组奖励 |
|---|---|---|---|
| 1 - White Stake | 白 | 基础难度，无额外限制 | — |
| 2 - Red Stake | 红 | Small Blind 不给钱 | Zodiac Deck |
| 3 - Green Stake | 绿 | 每个 Ante 所需分数增长更快 | Painted Deck |
| 4 - Black Stake | 黑 | Joker 有 30% 概率带 Eternal 贴纸（无法卖/摧毁）| Anaglyph Deck |
| 5 - Blue Stake | 蓝 | 每局 -1 弃牌数 | Plasma Deck |
| 6 - Purple Stake | 紫 | 所需分数增长幅度再次提升 | — |
| 7 - Orange Stake | 橙 | Joker 有 30% 概率带 Perishable 贴纸（5 轮后失效）| Erratic Deck |
| 8 - Gold Stake | 金 | Joker 有 30% 概率带 Rental 贴纸（每轮花 $3）| — |

> 同一 Joker 可以同时有 Eternal + Perishable + Rental 三种贴纸（金注）

### 难度设计逻辑

- **白注**：新手友好，无额外压力
- **红注**：经济压力（Small Blind 无收入）
- **绿/紫注**：分数压力（需求增长加速）
- **黑注**：构建压力（Eternal 让 Joker 无法换手）
- **蓝注**：操作压力（更少弃牌机会）
- **橙注**：持续管理压力（Joker 会过期）
- **金注**：现金流压力（Joker 每轮消耗金钱）

---

## 挑战卡组（20 种）

挑战卡组只能在**白注**游玩，有特殊初始 Joker/规则/限制，前 5 种通过赢 5 个不同牌组解锁：

### 代表性挑战

| 挑战名 | 核心规则 | 策略方向 |
|---|---|---|
| The Omelette | 所有 Blind 无金钱奖励，无利息 | 5 张 Egg（卖出获钱）经济特化 |
| 15 Minute City | 双份面牌，无 A/2/3 | Ride the Bus（不出面牌积累 Mult）|
| Rich get Richer | Chips 不能超过当前金额 | 金钱无上限，Money → Chips 特化 |
| Mad World | 所有牌均视为面牌 | 面牌 Joker 特化 |
| Jokerless | 没有 Joker 槽位，Joker 不出现 | 纯扑克牌型强化流 |
| Non-Perishable | 所有 Joker 为 Eternal | 无法出售，需仔细规划 |
| Luxury Tax | 每持有 $5 手牌上限 -1（初始 10）| 贫穷流 |

---

## 设计启示

1. **卡组作为局内起点差异化**：每种卡组改变的不是底层规则而是"起始配置"，给不同玩家找到不同节奏的入口。
2. **难度通过"叠加限制"设计**：不是单纯提高数值，而是给玩家加各种类型的限制（经济/操作/构建），让高难度有独特的挑战感。
3. **挑战卡组作为高级内容**：挑战模式通过强制限制迫使玩家探索非常规策略，是给熟练玩家的玩法扩展，不影响主线体验。
4. **卡组与 Stake 的解锁绑定**：每个卡组需要独立解锁高难度，是一种有深度的内容分发机制，避免一次性解锁所有内容导致游玩感稀释。

---

[<< 上一章](06_shop_economy.md) · [返回索引](00_index.md) · [下一章 >>](08_design_insights.md)
