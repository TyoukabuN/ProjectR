# 06 商店与经济系统

[<< 上一章](05_consumables.md) · [返回索引](00_index.md) · [下一章 >>](07_decks_stakes.md)

---

## 商店（The Shop）

商店在每次过关后开放，可自由购买。

### 默认商品槽位

| 类型 | 数量 | 补货时机 |
|---|---|---|
| 随机卡（Joker/Tarot/Planet 等）| 2 | 每次进入商店 |
| 增益包（Booster Pack）| 2 | 每次进入商店 |
| Voucher | 1 | 击败 Boss Blind 后 |

> 第一次访问商店时，保证提供 1 个普通 Buffoon Pack

### 商品出现概率

商店随机卡权重：
- **Joker**: 71.4%（权重 20）
- **Tarot**: 14.3%（权重 4）
- **Planet**: 14.3%（权重 4）
- 买了 Magic Trick Voucher 后可出现普通牌；Ghost Deck 可出现幻灵牌

Joker 稀有度（商店内）：Common 70% / Uncommon 25% / Rare 5%

---

## 商品定价系统

### 基础价格

| 商品类型 | 价格 |
|---|---|
| Common Joker | $1–6 |
| Uncommon Joker | $4–8 |
| Rare Joker | $7–10 |
| Legendary Joker | $20 |
| Tarot Card | $3 |
| Planet Card | $3 |
| Spectral Card | $4 |
| Playing Card | $1 |
| Booster Pack（普通）| $4 |
| Booster Pack（Jumbo）| $6 |
| Booster Pack（Mega）| $8 |
| Voucher | $10 |

### 版本附加价格（Joker）

| 版本 | 附加价格 |
|---|---|
| Foil | +$2 |
| Holographic | +$3 |
| Polychrome | +$5 |
| Negative | +$5 |

### 购买价格计算公式

```
买入价 = (基础价 + 版本价) × 折扣系数
（结果向下取半数圆整，最低 $1）

卖出价 = floor(买入价 / 2)
```

### 折扣 Voucher

| Voucher | 效果 |
|---|---|
| Clearance Sale | 所有商品 25% 折扣（系数 0.75）|
| Liquidation | 所有商品 50% 折扣（系数 0.50）（需先买 Clearance Sale）|

---

## 刷新（Reroll）

- 消耗费用可刷新商店的随机卡（2 个槽位）
- 起始价格 $5，每次刷新 +$1，进入新商店重置回 $5
- **注意**：增益包和 Voucher 不随刷新重置
- Reroll Surplus Voucher → 起始价降至 $3
- Reroll Glut Voucher → 起始价再降 $2（最低 $1）

---

## Voucher（优惠券）

共 32 张（16 对），每对有基础版和升级版，永久生效：

### 资源/效率类

| 基础 Voucher | 效果 | 升级 Voucher | 效果 |
|---|---|---|---|
| Overstock | 商店 +1 随机卡槽（3 个）| Overstock Plus | 商店 +1 随机卡槽（4 个）|
| Clearance Sale | 全商品 25% 折扣 | Liquidation | 全商品 50% 折扣 |
| Grabber | 每轮永久 +1 手数 | Nacho Tong | 每轮再 +1 手数 |
| Wasteful | 每轮永久 +1 弃牌数 | Recyclomancy | 每轮再 +1 弃牌数 |
| Reroll Surplus | 刷新费 -$2 | Reroll Glut | 刷新费再 -$2 |
| Crystal Ball | +1 消耗品槽 | Omen Globe | 秘术包中幻灵牌概率 +20% |
| Paint Brush | +1 手牌上限 | Palette | 再 +1 手牌上限 |
| Magic Trick | 商店可出售普通牌 | Illusion | 商店普通牌可带强化/版本 |
| Blank | 没有效果？| Antimatter | +1 Joker 槽位 |

### 消耗品频率类

| 基础 Voucher | 效果 | 升级 Voucher | 效果 |
|---|---|---|---|
| Tarot Merchant | 塔罗出现 ×2 | Tarot Tycoon | 塔罗出现 ×4 |
| Planet Merchant | 星球出现 ×2 | Planet Tycoon | 星球出现 ×4 |
| Telescope | 天体包始终包含最常打出牌型的星球 | Observatory | 消耗品区星球牌给对应牌型 ×1.5 Mult |
| Hone | Foil/Holo/Poly 出现 ×2 | Glow Up | 出现 ×4 |

### 经济/关卡类

| 基础 Voucher | 效果 | 升级 Voucher | 效果 |
|---|---|---|---|
| Seed Money | 利息上限 $10/轮 | Money Tree | 利息上限 $20/轮 |
| Hieroglyph | -1 Ante，每轮 -1 手数 | Petroglyph | 再 -1 Ante，每轮 -1 弃牌数 |
| Director's Cut | 每个 Ante 可花 $10 刷新 Boss Blind 一次 | Retcon | 无限刷新 Boss Blind（每次 $10）|

---

## 彩签（Tags）

跳过 Small/Big Blind 时获得，共 24 种：

| 彩签 | 效果 | 可用 Ante |
|---|---|---|
| Boss Tag | 重新刷新 Boss Blind | 任意 |
| Charm Tag | 获得 Mega Arcana 包（免费）| 任意 |
| Coupon Tag | 下家商店初始商品和包全免费 | 任意 |
| D6 Tag | 下家商店刷新费从 $0 起 | 任意 |
| Double Tag | 复制下一个获得的彩签（Double 本身除外）| 任意 |
| Economy Tag | 金钱翻倍（最多 $40）| 任意 |
| Foil / Holo / Poly / Negative Tag | 下家商店随机一张 Base 版 Joker 免费变为对应版本 | 不同限制 |
| Investment Tag | 击败下个 Boss Blind 后获得 $25 | 任意 |
| Juggle Tag | 本轮 +3 手牌上限 | 任意 |
| Meteor Tag | 获得 Mega Celestial 包（免费）| Ante 2+ |
| Orbital Tag | 随机一个牌型升级 3 级 | Ante 2+ |
| Rare Tag | 商店出现一张免费 Rare Joker | 任意 |
| Speed Tag | 每跳过一个 Blind 本局 +$5（当前 +$5）| 任意 |
| Top-up Tag | 最多生成 2 张 Common Joker（需有空位）| Ante 2+ |
| Uncommon Tag | 商店出现一张免费 Uncommon Joker | 任意 |
| Voucher Tag | 下家商店额外出现一张 Voucher | 任意 |
| Buffoon Tag | 获得 Mega Buffoon 包（免费）| Ante 2+ |
| Ethereal Tag | 获得免费 Spectral 包 | Ante 2+ |
| Garbage Tag | 本局每次未用弃牌 +$1（当前 $0）| Ante 2+ |
| Handy Tag | 本局每次打过手 +$1（当前 $0）| Ante 2+ |
| Standard Tag | 获得 Mega Standard 包（免费）| Ante 2+ |

---

## 利息系统

| 持有金额 | 利息收入/轮 |
|---|---|
| $5 | $1 |
| $10 | $2 |
| $15 | $3 |
| $20 | $4 |
| $25+ | $5（基础上限）|

- Seed Money Voucher → 上限提升至 $10/轮
- Money Tree Voucher → 上限提升至 $20/轮
- **Green Deck** 会关闭利息系统，改为按剩余手/弃牌数给钱

---

## 设计启示

1. **利息 = 滚雪球激励**：持有金钱获得利息，形成"不乱花钱"的经济策略，让攒钱变成有意义的决策。
2. **Voucher 升级对设计**：基础版 + 升级版的结构让玩家有短期和长期目标，同时使商店不过载（同一条线只能先买基础才能买升级）。
3. **刷新费随次数增长**：避免玩家无限刷新，但 D6 Tag / Reroll 系列 Voucher 可以对抗此限制，形成分支策略。
4. **彩签组合效应**：Double Tag 可以复制其他彩签，形成"双份收益"的高潮时刻；玩家在获得 Double Tag 时会特意等待更好的彩签。
5. **商品槽位可扩展**：Overstock 系列 Voucher 可把默认 2 个商品槽扩展到 4 个，让购买机会翻倍，是前期强势投资。

---

[<< 上一章](05_consumables.md) · [返回索引](00_index.md) · [下一章 >>](07_decks_stakes.md)
