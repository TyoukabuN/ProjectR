# 05 消耗品系统

[<< 上一章](04_card_modifiers.md) · [返回索引](00_index.md) · [下一章 >>](06_shop_economy.md)

---

## 概述

消耗品（Consumables）是使用后即消失的单次效果牌，存放在消耗品槽位中（默认 2 个，可通过 Voucher 增加）。分为三大类：

| 类型 | 数量 | 主要用途 |
|---|---|---|
| 塔罗牌（Tarot Cards）| 22 张 | 修改普通扑克牌、给钱、生成其他牌 |
| 星球牌（Planet Cards）| 12 张（其中 3 张秘密）| 升级对应牌型的等级 |
| 幻灵牌（Spectral Cards）| 18 张 | 高风险高收益效果（改牌组、获稀有 Joker 等）|

---

## 塔罗牌（Tarot Cards）——22 张

基于塔罗牌大阿卡纳设计，效果覆盖强化牌、改花色、给钱、生成牌等：

### 修改扑克牌类

| 塔罗牌 | 效果 |
|---|---|
| The Magician | 增强选中的 2 张牌为 Lucky Card |
| The Empress | 增强选中的 2 张牌为 Mult Card |
| The Hierophant | 增强选中的 2 张牌为 Bonus Card |
| The Lovers | 增强选中的 1 张牌为 Wild Card |
| The Chariot | 增强选中的 1 张牌为 Steel Card |
| Justice | 增强选中的 1 张牌为 Glass Card |
| The Devil | 增强选中的 1 张牌为 Gold Card |
| The Tower | 增强选中的 1 张牌为 Stone Card |
| Strength | 将选中的最多 2 张牌点数 +1 |
| The Hanged Man | 摧毁选中的最多 2 张牌 |
| Death | 选 2 张牌，将左边的牌变成右边的牌的副本 |
| The Star | 将选中的最多 3 张牌变为红心 |
| The Moon | 将选中的最多 3 张牌变为梅花 |
| The Sun | 将选中的最多 3 张牌变为方块 |
| The World | 将选中的最多 3 张牌变为黑桃 |

### 资源/工具类

| 塔罗牌 | 效果 |
|---|---|
| The Fool | 创建本局最后使用的塔罗/星球牌（The Fool 除外）|
| The High Priestess | 创建最多 2 张随机星球牌（需有空位）|
| The Emperor | 创建最多 2 张随机塔罗牌（需有空位）|
| The Hermit | 将金钱翻倍（最多 +$20）|
| The Wheel of Fortune | 1/4 概率给随机 Joker 附加 Foil/Holo/Poly 版本 |
| Temperance | 获得当前所有 Joker 卖出价值之和（最多 $50）|
| Judgement | 创建一张随机 Joker（需有空位）|

---

## 星球牌（Planet Cards）——12 张

每张星球牌对应一种牌型，使用后该牌型升级 1 级（无上限）：

| 星球牌 | 对应牌型 | 每级加成 |
|---|---|---|
| Pluto | High Card | +1 Mult, +10 Chips |
| Mercury | Pair | +1 Mult, +15 Chips |
| Uranus | Two Pair | +1 Mult, +20 Chips |
| Venus | Three of a Kind | +2 Mult, +20 Chips |
| Saturn | Straight | +3 Mult, +30 Chips |
| Jupiter | Flush | +2 Mult, +15 Chips |
| Earth | Full House | +2 Mult, +25 Chips |
| Mars | Four of a Kind | +3 Mult, +30 Chips |
| Neptune | Straight Flush | +4 Mult, +40 Chips |
| Planet X ⭐ | Five of a Kind | +3 Mult, +35 Chips（秘密）|
| Ceres ⭐ | Flush House | +4 Mult, +40 Chips（秘密）|
| Eris ⭐ | Flush Five | +3 Mult, +50 Chips（秘密）|

> ⭐ 三张秘密星球牌需先在本局打出对应牌型才能在商店出现

---

## 幻灵牌（Spectral Cards）——18 张

高风险效果，来源包括幻灵包、Ghost Deck、特定 Joker（Séance, Sixth Sense 等）：

### 牌组改造类

| 幻灵牌 | 效果 |
|---|---|
| Familiar | 摧毁手中随机 1 张牌，向手牌加入 3 张带强化的随机面牌 |
| Grim | 摧毁手中随机 1 张牌，向手牌加入 2 张带强化的随机 A |
| Incantation | 摧毁手中随机 1 张牌，向手牌加入 4 张带强化的随机数牌 |
| Sigil | 将手中所有牌变为同一随机花色 |
| Ouija | 将手中所有牌变为同一随机点数（手牌 -1）|
| Cryptid | 复制选中的手中 1 张牌，共生成 2 张副本 |
| Ectoplasm | 给随机 Joker 加 Negative 版本，手牌数 -1（永久）|
| Immolate | 摧毁手中随机 5 张牌，获得 $20 |

### Joker 操作类

| 幻灵牌 | 效果 |
|---|---|
| Wraith | 创建一张随机 Rare Joker，金钱变为 $0 |
| Ankh | 复制随机 Joker，摧毁所有其他 Joker（副本去除 Negative 版本）|
| Hex | 给随机 Joker 加 Polychrome 版本，摧毁所有其他 Joker |
| The Soul | 创建一张 Legendary Joker（需有空位）|

### 印记/版本类

| 幻灵牌 | 效果 |
|---|---|
| Talisman | 给手中选定牌加 Gold Seal |
| Deja Vu | 给手中选定牌加 Red Seal |
| Trance | 给手中选定牌加 Blue Seal |
| Medium | 给手中选定牌加 Purple Seal |
| Aura | 给手中选定牌加 Foil/Holo/Poly 版本 |

### 全局升级类

| 幻灵牌 | 效果 |
|---|---|
| Black Hole | 所有牌型等级 +1 |

---

## 增益包（Booster Packs）

增益包是商店里的组合购买选项，打开后即时选取：

| 包名 | 普通版（$4）| Jumbo 版（$6）| Mega 版（$8）|
|---|---|---|---|
| Arcana Pack | 选 1/3 塔罗 | 选 1/5 塔罗 | 选 2/5 塔罗 |
| Celestial Pack | 选 1/3 星球 | 选 1/5 星球 | 选 2/5 星球 |
| Standard Pack | 选 1/3 普通牌（加入牌组）| 选 1/5 | 选 2/5 |
| Buffoon Pack | 选 1/2 Joker | 选 1/4 Joker | 选 2/4 Joker |
| Spectral Pack | 选 1/2 幻灵 | 选 1/4 幻灵 | 选 2/4 幻灵 |

**增益包出现概率**（各自独立）：
- Standard / Arcana / Celestial Pack：普通 17.84% / Jumbo 8.92% / Mega 2.23%
- Buffoon Pack：普通 5.35% / Jumbo 2.68% / Mega 0.67%
- Spectral Pack：普通 2.68% / Jumbo 1.34% / Mega 0.31%

---

## 消耗品 Voucher 加成

| Voucher | 效果 |
|---|---|
| Tarot Merchant | 塔罗牌在商店出现概率 ×2 |
| Tarot Tycoon | 塔罗牌在商店出现概率 ×4（需先买 Tarot Merchant）|
| Planet Merchant | 星球牌在商店出现概率 ×2 |
| Planet Tycoon | 星球牌在商店出现概率 ×4 |
| Crystal Ball | +1 消耗品槽位 |
| Omen Globe | 秘术包中 20% 概率每张牌替换为幻灵牌 |
| Telescope | 天体包始终包含最常打出牌型的星球牌 |
| Observatory | 消耗品区的星球牌对应牌型打出时 ×1.5 Mult |

---

## 设计启示

1. **即时消耗 vs. 储存使用**：消耗品不自动使用，玩家可以在合适时机使用，制造"正确时机"的决策。Observatory Voucher 甚至让玩家把星球牌暂时留在槽位里持续发挥效果。
2. **幻灵牌的高风险设计**：Wraith（稀有 Joker 但清空金钱）、Ankh/Hex（获得强效 Joker 但摧毁其他）都是"舍弃 A 获得更好的 B"的决策，制造戏剧性时刻。
3. **增益包三个大小选项**：$4/$6/$8 三档，成本与收益明确，让玩家自由决定投入。
4. **消耗品槽位限制**：默认只有 2 个槽位是一个有趣的约束，玩家必须取舍，何时使用是策略之一。
5. **The Fool 复制机制**：复制上一张使用的牌，形成"在正确时机使用 The Fool"的额外策略层。

---

[<< 上一章](04_card_modifiers.md) · [返回索引](00_index.md) · [下一章 >>](06_shop_economy.md)
