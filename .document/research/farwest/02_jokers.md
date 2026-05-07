# Joker 系统

[<< 上一章](01_overview.md) · [返回索引](00_index.md) · [下一章 >>](03_spells.md)

---

## 1. Joker 基础机制

Jokers 是 Far Far West 的核心 Build 要素，提供从简单数值加成到全新机制的各类效果。Joker 分为两大类：

- **Hero Jokers**：作用于角色本身（移动、生存、法术等），从 Dr. Spark-Twist 处获取
- **Weapon Jokers**：作用于特定武器，从枪匠 Anvil 处获取

### 1.1 稀有度与槽位

| 稀有度 | 槽位需求 | 获取难度 |
|--------|----------|----------|
| Normal（普通） | 1 槽 | 极易 |
| Fine（优秀） | 2 槽 | 容易 |
| Prime（精良） | 3 槽 | 中等 |
| Mythic（史诗） | 4 槽 | 较难 |
| Legendary（传说） | 5 槽 | 很难 |
| Unique（独特） | 5 槽 | 绑定特定武器，随武器等级解锁 |

### 1.2 装备规则

- Hero Jokers 只能装备在角色槽位上
- Weapon Jokers 只能装备在对应的武器上
- 部分通用 Joker 可同时装备在 Hero 和武器上
- Unique Jokers 是某武器的专属，其他武器无法装备

### 1.3 获取方式

1. **局内 Slot Machine**：花费 250 souls 随机抽取
2. **局外直接购买**：在对应商店用金币购买
3. **局内掉落**：敌人死亡时概率掉落
4. **武器 Unique Joker**：随武器等级提升自动解锁（主武器 35/55 级，副武器 30/40 级）

### 1.4 Joker 槽位解锁

武器初始 2 个 Joker 槽，随等级提升逐步解锁：
```
Lv.4 → Lv.6 → Lv.8 → Lv.10 → Lv.13 → Lv.16 → Lv.19 → Lv.24 → Lv.29 → Lv.35 → Lv.42 → Lv.50
```
共 12 次解锁，加上初始 2 槽，最终可达 14 槽（转生后可进一步扩展）。

---

## 2. Hero Jokers（角色 Joker）

以下 Jokers 仅可装备在角色上（itemHero = Yes）：

| 名称 | 稀有度 | 效果描述 |
|------|--------|----------|
| Acid Mastery | Mythic | 每装备一个 Acid 法术，Acid 冷却 -10% |
| Battlemage | Mythic | 击杀敌人缩短所有法术冷却 1% |
| Bouncing Ball | Fine | 空中时 +15% 伤害 |
| Cactus Mastery | Mythic | 每装备一个 Cactus 法术，Cactus 冷却 -10% |
| Care Package | Fine | +1 工具弹药 |
| Chad | Fine | 显示游荡商人位置，购买折扣 20% |
| Cheapskate | Prime | 每持有 20 金币，+1 HP |
| Chonky | Prime | +20% 伤害，但 +25% 重力 |
| Deadly Strong | Normal | 死亡为幽灵时移速和伤害增加 |
| Disgrace | Prime | +30% 移速（但你的马不想跟你） |
| Dwarf | Legendary | 体型变小，获得移速、射速和低重力 |
| Elec Mastery | Mythic | 每装备一个 Elec 法术，Elec 冷却 -10% |
| Extra Dash | Legendary | +1 次冲刺 |
| Extra Jump | Mythic | +1 次跳跃 |
| Fist Fight | Fine | 近战伤害翻倍 |
| Giant | Legendary | 体型变大，获得生命、武器伤害和法术冷却缩减 |
| Glass Cannon | Legendary | +40% 武器/近战伤害，但 -25% HP |
| Gold Prospector | Fine | 任务结束时获得金币 +20% |
| Headbang | Mythic | 弱点伤害 +20% |
| Healthy Boi | Prime | 满血时 +20% 移速/换弹速度/射速 |
| Heavy Drinker | Normal | 治疗瓶效果 +25% |
| Helping Hand | Normal | 复活队友速度翻倍 |
| Hit Me | Mythic | 被击中后 20 秒内每秒回复 0.5 HP |
| Hoarder | Mythic | 拾取 Joker 时有 15% 概率额外出现一个随机 Joker |
| Joker Dealer | Mythic | 每个激活的 Joker（含武器）+1 HP |
| Last Dance | Normal | 低于 30% HP 时 +30% 移速 |
| Lazy | Mythic | 禁用弹药拾取，改为被动回复弹药 |
| Life Pact | Mythic | 击杀回复 2 HP，但最大 HP -30% |
| Magic Friendship | Prime | 靠近队友时被动回血 |
| Medicard | Mythic | 拾取 Joker 时治疗 +50 HP |
| Moon Gravity | Mythic | 重力 -25% |
| Pyro Mastery | Mythic | 每装备一个 Pyro 法术，Pyro 冷却 -10% |
| ... | ... | 更多数据待补充 |

---

## 3. Weapon Jokers（武器 Joker）

### 3.1 全武器通用 Jokers

以下 Jokers 可装备在所有主武器上：

| 名称 | 稀有度 | 效果描述 |
|------|--------|----------|
| Ammo Supply | Prime | +5% 概率敌人死亡掉落弹药箱 |
| Anti-Gravity Falls | Fine | +2% 概率敌人死亡生成反重力区域 |
| Bell Shot | Fine | +5% 概率弱点命中召唤治疗幽灵钟 |
| Cactus Day | Fine | +3% 概率敌人死亡生成 Cactus Mino |
| Chickshot | Normal | +50% 概率子弹发出鸡叫声（无用效果） |
| Destroyer | Prime | 弹匣前 25% 子弹 +15% 伤害 |
| Explosive Rounds (Pyro) | Prime | +5% 概率发射 Pyro 爆炸弹 |
| Explosive Rounds (Acid) | Prime | +5% 概率发射 Acid 爆炸弹 |
| Explosive Rounds (Elec) | Prime | +5% 概率发射 Elec 爆炸弹 |
| Gold Tooth | Normal | +10% 概率敌人死亡掉落 1 金币 |
| Lucky Strike | Prime | +10% 概率造成双倍伤害 |
| Machine Gun | Legendary | 每造成 50 伤害，本任务射速 +0.3% |
| Party Shot | Normal | +50% 概率弱点命中爆彩带（无用效果） |
| Pick Pick | Fine | +5% 概率命中时生成伤害性镐斧 |
| Pigshot | Normal | +5% 概率命中生成猪，射击猪可引爆 |

### 3.2 特定武器 Unique Jokers

| Joker 名称 | 绑定武器 | 效果 |
|-----------|---------|------|
| Aiming Burst | Quad Cylinder | 瞄准时射击有完美精度 |
| Homing Burst | Quad Cylinder | 每射 20 发子弹，发射 5 发追踪弹 |
| Fanning Ace | Revolver | 换弹中途正确时机开火可连射所有子弹（低精度） |
| Mark Ace | Revolver | 子弹标记敌人 3 秒，标记敌人受到的伤害 +15% |
| Focus Shot | Long Ranger | 每次击杀减少法术冷却 1 秒 |
| Mindshot | Long Ranger | 弱点命中引发爆炸 |
| Elemental Spin | Minigun | 30% 概率发射随机元素子弹 |
| Frenzy Spin | Minigun | 击杀计数 >10 时，射速翻倍 |
| Overdraw | Bow | 蓄力 2 秒发射 3 支扇形箭，拉弓速度减少蓄力时间 |
| Eco Trick | Dual Revolvers | 换弹不再浪费剩余弹药 |
| Jump Star | Sheriff's Star | 持有 Sheriff's Star 时 +2 跳跃 |
| Chonky Throw | Boomerang | 回旋镖体型翻倍 |
| Lingering Throw | Boomerang | 回旋镖滞留时间翻倍 |
| Eagle Lever | Leveredge | 连续命中无 miss，每层 +5% 伤害，最高 +50% |
| Overblast | Shotgun | 弹匣每缺少一发子弹，额外发射一颗弹丸 |

### 3.3 武器兼容性速查

| Joker | Quad | Revolver | LongR | Minigun | Bow | DualRev | Sheriff | Boomerang | Lever | Shotgun |
|-------|------|----------|-------|---------|-----|---------|---------|-----------|-------|---------|
| Clutch | Yes | Yes | Yes | No | No | No | No | No | Yes | No |
| Consumer | Yes | Yes | Yes | No | No | No | No | No | No | No |
| Camper | Yes | Yes | Yes | Yes | Yes | Yes | Yes | No | Yes | Yes |

---

## 4. 设计启示

1. **Joker 作为"被动技能树"**：将传统天赋树转化为可收集、可交易的道具，增加随机性和复玩性
2. **稀有度=槽位成本**：高稀有度 Joker 需要更多槽位，强迫玩家在"少量强力"和"大量普通"之间抉择
3. **Unique Joker 强化武器个性**：每把武器 2 个 Unique Joker，让不同武器有独特的玩法风格
4. **趣味 Joker 的存在**：Chickshot、Party Shot 等纯趣味 Joker 增加了游戏的轻松氛围
5. **Mastery 系列引导 Build**：元素 Mastery Joker 鼓励玩家围绕特定元素法术构建配装

