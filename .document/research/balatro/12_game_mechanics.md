# Balatro 游戏机制全览

*数据来源：balatrowiki.org/w/Category:Game_Mechanics，共15个机制页面*

---

## 1. 评分核心：Chips 与 Mult

### 计分公式
```
得分 = 总 Chips × 总 Mult
```

### Chips 来源
| 来源 | 数值 |
|------|------|
| 牌型基础值 | 由打出的扑克牌型决定（见 scoring 文档）|
| Ace | +11 Chips |
| 面牌（J/Q/K）| +10 Chips |
| 数字牌 | 等于牌面值（2→+2, 9→+9）|
| Bonus Card 强化 | +30 Chips |
| Stone Card 强化 | +50 Chips（无花色/点数）|
| Foil 版本（牌/Joker）| +50 Chips |
| Chips 类 Joker | 见 Joker 图鉴 |

### Mult 来源
| 来源 | 数值 |
|------|------|
| 牌型基础值 | 由扑克牌型决定 |
| Mult Card 强化 | +4 Mult |
| Glass Card 强化 | ×2 Mult（1/4概率自毁）|
| Steel Card 强化 | ×1.5 Mult（持牌时）|
| Lucky Card 强化 | 1/5 概率+20 Mult |
| Holographic 版本 | +10 Mult |
| Polychrome 版本 | ×1.5 Mult |
| 加法 Mult Joker | +Mult |
| 乘法 Mult Joker | ×Mult |
| Planet Cards | 提升特定牌型的基础 Mult |

> **关键设计**：加法在前、乘法在后，触发顺序影响最终得分。On Scored 类 Joker 先于 Independent 类触发，这意味着 On Scored 的 ×Mult 乘的是累加了 Independent +Mult 之前的值。

---

## 2. Joker 触发类型（Activation Type）

Joker 按触发时机分为 9 种类型，顺序从 Priority 1 到最后：

| 触发类型 | 触发时机 | 代表 Joker |
|----------|----------|-----------|
| **On Played** | 打出某手牌时（Priority 1）| Space Joker、DNA、Midas Mask |
| **On Scored** | 每张计分牌计算时 | Greedy Joker、Odd Todd、Dusk（追加触发）|
| **On Held** | 手中持有特定牌时 | Raised Fist、Baron、Mime（追加触发）|
| **Independent** | 所有牌计分后 | Joker、Sly Joker、Steel Joker |
| **On Other Jokers** | 作用于其他 Joker 时 | Baseball Card（唯一）|
| **On Discard** | 弃牌时 | Castle、Mail-In Rebate、Burnt Joker |
| **Mixed** | 属于多个类型 | Runner（Mixed: On Played + Independent）|
| **On Blind Select** | 选择 Blind 时 | Riff-Raff、Cartomancer、Burglar |
| **N/A (Passive)** | 被动效果，不主动触发 | Four Fingers、Golden Joker |

> **策略要点**：
> - On Scored 的 ×Mult 在 Independent 的 +Mult 之前执行——若有 +Mult Independent Joker，其加值不会被 On Scored ×Mult 所乘
> - Passive Joker 无法被 Blueprint/Brainstorm 复制
> - On Blind Select 可以被 Blueprint/Brainstorm 复制

---

## 3. Blind 与 Ante 结构

### 基本架构
- 1 个 Ante = 3 个 Blind（小盲→大盲→Boss盲）
- 共 8 个 Ante，Ante 8 的 Boss 为 **Showdown Blind**（5种之一）
- 通关后可继续 **无尽模式**（Ante 9+，每 8 个 Ante 出现一次 Showdown）

### Blind 对比
| 类型 | 分数要求 | 奖励 | 可跳过？ |
|------|----------|------|---------|
| Small Blind | 1× 基础分 | $3（Gold Stake+ 无奖励）| ✅ 可跳过 |
| Big Blind | 1.5× 基础分 | $4 | ✅ 可跳过 |
| Boss Blind | 2× 基础分（多数）| $5 | ❌ 不可 |
| Showdown Blind | 2× / 4× / 6× | $8 | ❌ 不可 |

### Boss Blind 完整列表（23种 + 5种Showdown）
| Boss Blind | 效果 | 最低出现 Ante |
|-----------|------|--------------|
| The Hook | 每次出牌后随机弃掉手中2张牌 | Any |
| The Ox | 打出本局最常见牌型→金钱清零 | 6 |
| The House | 第一手牌正面朝下 | 2 |
| The Wall | 分数要求 4× 基础 | 2 |
| The Wheel | 每7张牌有1张正面朝下 | 2 |
| The Arm | 打出的扑克牌型等级-1 | 2 |
| The Club | 所有梅花牌被 Debuff | Any |
| The Fish | 每次出牌后新摸牌正面朝下 | 2 |
| The Psychic | 必须出5张牌 | Any |
| The Goad | 所有黑桃牌被 Debuff | Any |
| The Water | 本轮弃牌次数归零 | 2 |
| The Window | 所有方块牌被 Debuff | Any |
| The Manacle | 手牌上限-1 | Any |
| The Eye | 本轮不可重复牌型 | 3 |
| The Mouth | 本轮只能出一种牌型 | 2 |
| The Plant | 所有面牌被 Debuff | 4 |
| The Serpent | 每次出牌/弃牌后只摸3张 | 5 |
| The Pillar | 本 Ante 之前打出过的牌被 Debuff | Any |
| The Needle | 本轮只能出1手牌 | 2 |
| The Head | 所有红心牌被 Debuff | Any |
| The Tooth | 每张出牌消耗$1 | 3 |
| The Flint | 扑克牌型基础 Chips/Mult 减半 | 2 |
| The Mark | 所有面牌正面朝下 | 2 |
| **Amber Acorn** | 翻转并重洗所有 Joker 位置 | 8 |
| **Verdant Leaf** | 所有牌被 Debuff，直到卖出1张Joker | 8 |
| **Violet Vessel** | 分数要求 6× 基础 | 8 |
| **Crimson Heart** | 每手牌随机禁用一张Joker | 8 |
| **Cerulean Bell** | 强制保留1张特定牌在手中 | 8 |

### Ante 分数要求
- 基础：随 Ante 等级递增（约2倍关系）
- 难度加成：Green Stake+ 提升要求；Plasma Deck 双倍要求
- Ante 0：通过 Hieroglyph 凭证（-1 Ante, -1次出牌/轮）达到

---

## 4. 经济系统

### 利息（Interest）
- **规则**：每轮结算时，每持有$5获得$1利息，**上限$5/轮**（即持有$25以上不再增加）
- **扩展**：
  - Seed Money 凭证：利息上限提升至 $10（需持有$50）
  - Money Tree 凭证（升级）：利息上限提升至 $20（需持有$100）
  - To the Moon Joker：每$5额外获得$1利息（可叠加凭证）

| 配置 | 最大利息/轮 | 需持有金额 |
|------|------------|-----------|
| 默认 | $5 | $25 |
| + Seed Money | $10 | $50 |
| + Money Tree | $20 | $100 |
| + To the Moon | $10 | $25 |
| + To the Moon + Money Tree | $40 | $100 |

- **不产生利息**：Green Deck、The Omelette / Mad World 挑战组合

### 剩余手牌收益
- 默认：每剩余1手牌 = $1
- Green Deck：每剩余1手牌 = $2，每剩余1次弃牌 = $1（但不产生利息）
- 某些挑战组合（The Omelette / Mad World）：剩余手牌不产生金钱

---

## 5. 出手次数（Hands）

| 属性 | 默认值 |
|------|--------|
| 每轮出手次数 | 4 |
| 最低保障 | 1（不会因组合降至0）|

### 增加出手
- Blue Deck: +1/轮
- Grabber 凭证: +1（永久）
- Nacho Tong 凭证（升级）: 再+1（永久）
- Burglar Joker: 选盲后+3手，但失去全部弃牌

### 减少出手
- Black Deck: -1/轮
- Troubadour Joker: -1/轮（换来+2手牌数）
- Hieroglyph 凭证: -1/轮（换来-1 Ante）
- The Needle Boss Blind: 本轮只剩1手

---

## 6. 弃牌次数（Discards）

| 属性 | 默认值 |
|------|--------|
| 每轮弃牌次数 | 3（Blue Stake+ 为2）|

### 弃牌方式
1. **标准弃牌**：选最多5张不想要的牌替换，消耗1次弃牌机会
2. **垫手牌（Throwaway Hand）**：手牌耗尽时当手牌用，代价是少赚$1（剩余手牌奖励）
3. **伪弃牌（Pseudo-discard）**：打出包含非计分牌的牌型，非计分牌等效被弃（不消耗弃牌次数）

### 增加弃牌
- Red Deck: +1/轮
- Drunkard Joker: +1/轮
- Merry Andy Joker: +3/轮（但-1手牌数）
- Wasteful 凭证: +1（永久）
- Recyclomancy 凭证（升级）: 再+1（永久）

### 减少弃牌
- Blue Stake 及以上: -1（共2次）
- Burglar Joker: 本轮弃牌归零（换来+3手）
- The Water Boss Blind: 本轮弃牌归零

---

## 7. 手牌数（Hand Size）

- **默认值**：8张
- 手牌数 ≤ 0 → 进入盲注时**直接游戏结束**

### 增加手牌数
- Painted Deck: +2（但-1 Joker 槽位）
- Juggler Joker: +1（永久）
- Troubadour Joker: +2（但-1出手次数/轮）
- Turtle Bean Joker: +5（每轮-1，递减至消耗殆尽）
- Paint Brush 凭证: +1；Palette 凭证（升级）: 再+1
- Juggle Tag: +3（仅下一轮）

### 减少手牌数
- Stuntman Joker: -2（换来+250 Chips）
- Merry Andy Joker: -1（换来+3 弃牌/轮）
- Ectoplasm / Ouija 幻灵牌: -1（永久）
- The Manacle Boss Blind: -1（临时）

---

## 8. Joker 槽位（Joker Slot）

- **默认值**：5个
- 满槽时无法获取新 Joker，除非卖出/摧毁现有 Joker
- **例外**：Negative 版本 Joker 自带+1槽位（可超出上限）

### 增加槽位
- Black Deck: +1（共6个）
- Five-Card Draw 挑战组: +2（共7个）
- Negative 版本（Joker 上）: +1/张
- Antimatter 凭证（升级）: +1

### 减少槽位
- Painted Deck: -1（共4个）
- Blast Off 挑战组: -1（共4个）
- Cruelty 挑战组: -2（共3个）
- Typecast 挑战组: Ante 4 Boss 后强制归零

---

## 9. 消耗品槽位（Consumable Slot）

- **默认值**：2个
- 可容纳：塔罗牌 / 星球牌 / 幻灵牌

### 增加槽位
- Crystal Ball 凭证: +1（共3个）
- Negative 版本（消耗品上）: +1（仅 Perkeo Legendary Joker 可给消耗品加 Negative）

### 减少槽位
- Nebula Deck: -1（共1个）

---

## 10. 负面效果（Negative Effects）

### Debuff（减益）
- 受 Debuff 的牌：**失去所有强化/版本/印记效果**，基础点数/花色保留
- 但仍可参与组牌，不触发逐牌 Joker 效果
- 用于触发 Matador Joker：打出受 Debuff 的牌仍可触发 Boss Blind 能力，获得$8

**触发 Debuff 的 Boss Blind**：The Club / The Goad / The Window / The Head / The Plant / The Pillar / Verdant Leaf

**Debuff Joker**：
- Crimson Heart（Showdown）：每手牌随机 Debuff 一张 Joker
- Perishable 贴纸的 Joker：5轮后永久 Debuff

### Face-down（正面朝下）
- 看不到牌的点数/花色，但排序仍按点数/花色（可推测）
- 打出包含正面朝下的牌，手型显示为 "???"，但效果正常触发

**触发来源**：The House / The Fish / The Mark / The Wheel

### Force-selected（强制选中）
- Cerulean Bell（Showdown）：强制一张牌始终被选中

---

## 11. 跳过 Blind（Skip）

- **可跳过**：Small Blind 和 Big Blind
- **效果**：跳过评分和商店，获得 1 个 Tag
- **代价**：失去：利息收入 / 商店 / 手型升级 / 牌库调整机会

### 跳过奖励：Tags（共24种）
| Tag | 效果 | 可用 Ante |
|-----|------|----------|
| Boss Tag | 重滚 Boss Blind | Any |
| Buffoon Tag | 免费 Mega Buffoon Pack | 2+ |
| Charm Tag | 免费 Mega Arcana Pack | Any |
| Coupon Tag | 下个商店初始卡牌/包免费 | Any |
| D6 Tag | 下个商店重滚从$0开始 | Any |
| Double Tag | 复制下一个选中的 Tag | Any |
| Economy Tag | 金钱翻倍（上限$40）| Any |
| Ethereal Tag | 免费 Spectral Pack | 2+ |
| Foil Tag | 下个商店基础版 Joker 免费并变 Foil | Any |
| Garbage Tag | 本局每次未使用弃牌+$1 | 2+ |
| Handy Tag | 本局每次已出手牌+$1 | 2+ |
| Holographic Tag | 下个商店基础版 Joker 免费并变 Holographic | Any |
| Investment Tag | 击败下个 Boss Blind 后获得$25 | Any |
| Juggle Tag | 下轮+3手牌数 | Any |
| Meteor Tag | 免费 Mega Celestial Pack | 2+ |
| Negative Tag | 下个商店基础版 Joker 免费并变 Negative | 2+ |
| Orbital Tag | 特定牌型提升3级 | 2+ |
| Polychrome Tag | 下个商店基础版 Joker 免费并变 Polychrome | Any |
| Rare Tag | 商店出现免费 Rare Joker | Any |
| Speed Tag | 本局每跳过1个Blind+$5 | Any |
| Standard Tag | 免费 Mega Standard Pack | 2+ |
| Top-up Tag | 生成最多2张 Common Joker（需有空位）| 2+ |
| Uncommon Tag | 商店出现免费 Uncommon Joker | Any |
| Voucher Tag | 下个商店额外出现1张凭证 | Any |

> **注意**：Ante 1 有9种 Tag 不可出现（Negative/Standard/Meteor/Buffoon/Handy/Garbage/Ethereal/Top-up/Orbital）

---

## 12. 贴纸（Stickers）

贴纸分为两类：**游戏内贴纸**（影响局内行为）和**成就贴纸**（展示历史记录）

### 游戏内贴纸（3种）

| 贴纸 | 来源 | 效果 |
|------|------|------|
| **Eternal** | Black Stake+ 时，购买/获取的 Joker 有30%概率附带 | 不可卖出/不可摧毁 |
| **Perishable** | 同上，概率较低 | 5轮后永久 Debuff（无效化）|
| **Rental** | 同上 | 每轮结束扣$1 |

- Eternal 与 Perishable 不可同时存在
- Legendary Joker 不会出现 Eternal（除非特定挑战组）
- **11种自毁型 Joker** 不可获得 Eternal 贴纸（如 Gros Michel、Ice Cream、Seltzer、Mr. Bones 等）

### 成就贴纸（Stake Stickers）
- 记录"持有该 Joker/使用该 Deck 时完成了哪个难度的通关"的历史

---

## 13. 概率系统（Listed Probability）

- 以绿色文字显示的概率均为**列出概率**（Listed Probability）
- **Oops! All 6s Joker**：将所有列出概率翻倍（1 in 3 → 2 in 3）

### 带概率的主要条目
| 条目 | 概率 | 效果 |
|------|------|------|
| Glass Card | 1 in 4 | 计分时有概率自毁 |
| Lucky Card | 1 in 5 / 1 in 15 | +20 Mult / +$20 |
| 8 Ball Joker | 1 in 4 | 8点牌计分时生成塔罗牌 |
| Gros Michel Joker | 1 in 6 | 每轮结束有概率自毁 |
| Cavendish Joker | 1 in 1000 | 每轮结束有概率自毁（最低概率）|
| Space Joker | 1 in 4 | 打出的牌型升1级 |
| Business Card Joker | 1 in 2 | 面牌计分时+$2 |
| Reserved Parking | 1 in 2 | 手中面牌+$1 |
| Hallucination Joker | 1 in 2 | 开包时生成塔罗牌 |
| Bloodstone Joker | 1 in 2 | 红心牌计分时×1.5 Mult |
| The Wheel of Fortune 塔罗 | 1 in 4 | 随机Joker获得Foil/Holo/Polychrome |
| The Wheel Boss Blind | 1 in 7 | 摸牌时有概率正面朝下 |

---

## 设计启示摘要

1. **触发顺序即数值杠杆**：On Scored 类 ×Mult 早于 Independent 类 +Mult，理解顺序是构建强力 Build 的核心
2. **多维资源设计**：游戏同时管理 手牌数/弃牌数/出手数/Joker槽位/消耗品槽位/金钱/利息 七个维度，各维度可互相用 Joker/凭证/卡组置换
3. **跳过≠划算**：跳盲注的机会成本极高（失去商店、利息、升级），仅在特定 Build 中有价值
4. **概率放大机制**：Oops! All 6s 将所有概率翻倍，与高概率卡（如 Bloodstone 1/2）协同效果极强
5. **负面效果层次**：Debuff / Face-down / Force-select 三种负面状态各有针对策略，与 Matador 等 Joker 形成反向利用空间
6. **利息经济飞轮**：存钱→产利息→购更好装备→得更多钱，是核心经济循环；Green Deck 打破此循环换取即时收益
