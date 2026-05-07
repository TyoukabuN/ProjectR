# 属性增强卡 / 法力卡 / 万能卡 / 临时卡

[<< 上一章](03_cards_attack.md) · [返回索引](00_index.md) · [下一章 >>](05_dungeons_enemies.md)

## 属性增强卡（黄色）

属性增强卡临时或永久提升特定属性，部分在当前遭遇持续，部分在地牢剩余时间内持续。19张。

| 卡牌 | 费用 | 效果 |
|---|---|---|
| Bracer | 0 | 手牌+1 |
| Candle | 0 | Area：攻击5%溅射 |
| Spellbinder | 0 | Duration：Crawler触发2额外次数 |
| Skull O'Maniac | 0 | 敌人强度+10%，获得额外经验 |
| Spinach | 1 | Might：伤害+10% |
| Attractorb | 1 | 抽1牌 |
| Candella | 1 | Area：攻击10%溅射 |
| Crown | 1 | Growth：+15%经验成长。销毁 |
| Duplicator | 1 | Amount：发射1额外投射物 |
| Hollow Heart | 1 | +3最大生命。销毁 |
| Stone Mask | 1 | Greed：+10%金币。销毁 |
| Candelabrador | 2 | 攻击15%溅射 |
| Clover | 2 | +10%幸运 |
| Crystal Crown | 2 | Growth：+20%经验成长。销毁 |
| Du-Duplicator | 2 | 发射2额外投射物 |
| Forever Heart | 2 | +1最大生命 |
| Sprig o' Spinach | 2 | 伤害+20% |
| Tirajisú | 2 | Revival：+10%复活。销毁 |
| Friendship Amulet | W | 增加Combo |

### 属性增强卡设计规律

1. **销毁(Destroy)机制**：Crown/Crystal Crown/Hollow Heart/Stone Mask/Tirajisú 打出后销毁，一次性强力效果
2. **递进设计**：Candle(0费/5%) → Candella(1费/10%) → Candelabrador(2费/15%)，Area三档
3. **Spinach双档**：Spinach(1费/10%) → Sprig o' Spinach(2费/20%)
4. **Duplicator双档**：Duplicator(1费/+1) → Du-Duplicator(2费/+2)

## 法力卡（紫色）

法力卡生成法力值。6张。

| 卡牌 | 费用 | 效果 |
|---|---|---|
| Empty Tome | 0 | 加1法力 |
| Light Tome | 1 | 加2法力 |
| Weighty Tome | 2 | 加3法力 |
| Ancient Tome | 3 | 加4法力 |
| Song of Mana | 4 | 对多个敌人造成10伤害。原始法力。法力缩放 |
| Wings | W | 下张牌法力费用-1 |

### 法力卡设计规律

1. **Tome系列**：Empty(0/1) → Light(1/2) → Weighty(2/3) → Ancient(3/4)，每级+1费+1法力
2. **净收益**：Tome系列净收益始终为+1法力（付出N费获得N+1法力）
3. **Song of Mana**：攻击+法力混合卡，4费高成本但有伤害和法力缩放
4. **Wings**：W费用的法力减费卡，不中断Combo

## 万能卡（白色/灰色）

万能卡可在任何时刻打出而不中断Combo。多数为一次性使用。14张。

| 卡牌 | 原始类型 | 效果 |
|---|---|---|
| Big Coin Bag | 万能 | 加25金币。销毁 |
| Clover Petal | 万能 | +5%幸运。抽1牌。销毁 |
| Coin Purse | 万能 | 加10金币。抽1牌。销毁 |
| Little Clover | 万能 | +10%幸运。销毁 |
| Little Heart | 万能 | 治疗1。销毁 |
| Orologion | 万能 | 前排施加1冻结。销毁 |
| Raw Mana | 万能 | 加3法力。销毁。临时 |
| Rich Coin Bag | 万能 | （高额金币）销毁 |
| Rosary | 万能 | 消灭1行。销毁 |
| Vacuum | 万能 | 抽1牌。销毁 |
| Bracelet | 攻击 | 造成100伤害 |
| Parm Aegis | 防御 | 加1护甲 |
| Friendship Amulet | 属性 | 增加Combo |
| Wings | 法力 | 下张牌法力费用-1 |

### 万能卡核心价值

1. **Combo衔接器**：万能卡不中断Combo，可用于在Combo链中"插入"任意效果
2. **紧急工具箱**：Little Heart(急救)/Orologion(控制)/Rosary(清场) 等应急手段
3. **经济辅助**：Coin Bag系列提供金币资源
4. **14张中有10张纯万能卡**，4张是其他类型的W费用变体

## 临时卡（黑色）

临时卡通常由Boss在战斗中插入玩家牌组，带有debuff效果。7张。

| 卡牌 | 费用 | 效果 |
|---|---|---|
| Confuse | 0 | 法力费用随机化。临时 |
| Cursed Lancet | 0 | 冻结下张抽到的牌并增加其法力费用。临时 |
| Muddle | 0 | Crawler触发类型随机化。临时 |
| Shatter | 0 | （碎裂效果）临时 |
| Junk | 1 | 使用时销毁。抽1牌。临时 |
| Angelo Spietato | 5 | 治疗3。切割。销毁。临时。保留 |
| Mana Bomb | 5 | 受0伤害。对前排造成0伤害。法力炸弹 |

### 临时卡设计规律

1. **0费debuff**：Confuse/Cursed Lancet/Muddle 都是不消耗法力的干扰效果
2. **Confuse vs Muddle**：一个干扰费用（Mana），一个干扰触发条件（Crawler）
3. **Junk**：1费"堵牌"，占用手牌空间但可抽牌回收
4. **Mana Bomb**：看似0伤害0受伤，实际可能是法力相关的特殊机制触发器
5. **Angelo Spietato**：5费高额，治疗+切割效果，但作为debuff插入不划算

---

[<< 上一章](03_cards_attack.md) · [返回索引](00_index.md) · [下一章 >>](05_dungeons_enemies.md)
