# 宝石与进化系统

[<< 上一章](05_dungeons_enemies.md) · [返回索引](00_index.md) · [下一章 >>](07_relics_arcanas.md)

## 宝石系统概述

宝石（Gems）是可镶嵌到卡牌上的增益道具，来源为升级选择和宝箱。需先获取 Gem Hammer 遗物才能使用。

### 宝石稀有度

#### 升级来源

| 稀有度 | 权重 | 幸运系数 |
|---|---|---|
| Common | 60 | 0 |
| Uncommon | 30 | 0.1 |
| Rare | 20 | 0.25 |
| Very Rare | 2 | 0.5 |
| Ultra Rare | 0 | 0 |

- Luck 属性影响升级时宝石稀有度，对高稀有度加成更大
- Ultra Rare 宝石不会在升级中出现

#### 宝箱来源

| 稀有度 | 宝箱类型 | 权重 |
|---|---|---|
| Common | T1（普通宝箱） | 100 |
| Uncommon | T1 | 80 |
| Rare | T1+T2 | 50 |
| Very Rare | T2+T3 | 60 |
| Ultra Rare | T3（Boss宝箱） | 40 |

- T2 = 稀有紫色锁箱（需牺牲卡牌开启）
- T3 = Boss/迷你Boss击杀后装饰宝箱
- 若有可进化武器，Evolution宝石出现概率提升

## 宝石完整数据

### 属性类宝石

| 宝石 | 稀有度 | 效果 |
|---|---|---|
| Area | Common | 增加Area属性 |
| Armor | Common | 加护甲 |
| Double Damage | Common | 伤害翻倍 |
| Increase Mana Cost (+1) | Common | 法力费用+1 |
| Luck | Common | 增加幸运 |
| Might | Common | 增加Might |
| Mug | Common | 击杀敌人时获得金币 |
| Reduce Mana Cost (-1) | Common | 法力费用-1 |
| Restore Health | Common | 战后回复生命 |
| Duration | Uncommon | 增加Duration属性 |
| Greed | Uncommon | 增加贪婪属性 |
| Growth | Uncommon | 增加成长属性 |
| Nduja | Uncommon | 造成辣味伤害 |
| Quick Draw | Uncommon | 遭遇开始时优先抽出 |
| Refund | Uncommon | 退还法力 |
| Retain | Uncommon | 抽到时保留在手牌 |
| Blue Trigger | Uncommon | 添加蓝色触发 |
| Purple Trigger | Uncommon | 添加紫色触发 |
| Red Trigger | Uncommon | 添加红色触发 |
| Yellow Trigger | Uncommon | 添加黄色触发 |
| Increase Mana Cost (+2) | Uncommon | 法力费用+2 |
| Amount | Rare | 增加Amount属性 |
| Bombard | Rare | 樱桃炸弹连锁 |
| Calcium | Rare | 造成骨系伤害 |
| Coin Card | Rare | 生成金币卡 |
| Destroy | Rare | 卡牌销毁但给金币 |
| Drain | Rare | 击杀时治疗 |
| Draw | Rare | 抽1牌 |
| Freeze | Rare | 冻结随机敌人 |
| Magic Hat | Rare | 从上方召唤鸽子 |
| Remote | Rare | 召唤矿车 |
| Return | Rare | 打出后返回手牌 |
| Reverse Combo | Rare | 允许下张牌向上或向下延续Combo |
| Triple Damage | Rare | 伤害三倍 |
| Yin Yang | Rare | 偶数法力+1法力，奇数法力抽1牌 |
| Rainbow | Rare | 添加所有颜色触发 |
| Recycle | Rare | 跳过弃牌堆 |
| Crawler Caller | Rare | 召唤所有Crawler到手牌 |
| Reduce Mana Cost (-2) | Rare | 法力费用-2 |
| Armor Strike | Very Rare | 打出时以护甲值造成伤害 |
| Copy | Very Rare | 使用时复制此牌 |
| Countdown | Very Rare | 未使用时法力费用递减 |
| Easy Combo | Very Rare | 此牌始终享受Combo加成 |
| Kill Count | Very Rare | 以当前击杀总数对随机敌人造成伤害 |
| Leader | Very Rare | 在手牌中时攻击造成额外伤害 |
| Magnetic | Very Rare | 抽取另一张同名卡 |
| Midas | Very Rare | 在手牌中时打出的牌获得金币 |
| Coin Count | Ultra Rare | 以金币数量造成伤害 |
| Decimate | Ultra Rare | 造成敌人10%血量的伤害 |
| Echo | Ultra Rare | 重放上一张牌的效果 |
| Evolution | Ultra Rare | 进化为更强武器 |
| Fireproof | Ultra Rare | 移除销毁效果 |
| Free To Play | Ultra Rare | 打出不消耗法力 |
| Mana Rebate | Ultra Rare | 法力降至0时获得双倍法力 |
| Mild | Ultra Rare | 永不离开手牌但始终打断Combo |
| Uncrackable | Ultra Rare | 卡牌更不易碎裂 |
| Wild | Ultra Rare | 使卡牌变为万能 |
| X Mana | Ultra Rare | 费用等于剩余法力 |

### 宝石分类速查

| 分类 | 宝石 |
|---|---|
| 属性增强 | Area/Armor/Draw/Duration/Greed/Growth/Luck/Might/Restore Health/Amount |
| 伤害增幅 | Armor Strike/Bombard/Calcium/Coin Count/Decimate/Double Damage/Freeze/Kill Count/Magic Hat/Nduja/Remote/Triple Damage |
| 法力操控 | Countdown/Easy Combo/Free To Play/Increase Mana Cost/Mana Rebate/Mild/Reduce Mana Cost/Refund/Reverse Combo/Wild/X Mana/Yin Yang |
| 效果类 | Coin Card/Copy/Crawler Caller/Destroy/Echo/Fireproof/Magnetic/Quick Draw/Recycle/Retain/Return/Uncrackable |
| 击杀触发 | Drain/Mug |
| 手牌触发 | Leader/Midas |
| 颜色触发 | Red Trigger/Blue Trigger/Yellow Trigger/Purple Trigger/Rainbow |
| 特殊 | Evolution |

## 进化系统

进化需将 Evolution 宝石镶嵌到主卡上，且牌组中存在对应的副卡。进化后主卡宝石保留，副卡宝石丢失。

### 进化组合表

| 主卡 | 副卡 | 进化结果 |
|---|---|---|
| Whip | Hollow Heart / Forever Heart | Bloody Tear |
| Axe | Candle / Candella / Candelabrador | Death Spiral |
| Pentagram | Crown / Crystal Crown | Gorgeous Moon |
| Cross | Clover | Heaven Sword |
| Fire Wand | Spinach / Sprig o' Spinach | Hellfire |
| Magic Wand | Empty Tome / Light Tome / Weighty Tome / Ancient Tome | Holy Wand |
| Santa Water | Attractorb | La Borra |
| Song of Mana | Skull O'Maniac | Mannajja |
| Runetracer | Armor / Golden Armor / Rainbow Armor / Hero's Armor | NO FUTURE |
| Eight The Sparrow + Phiera Der Tuphello | Tirajisú | Phieraggi |
| Garlic | Pummarola / Pummadora | Soul Eater |
| Knife | Bracer | Thousand Edge |
| Lightning Ring | Duplicator / Du-Duplicator | Thunder Loop |
| King Bible | Spellbinder | Unholy Vespers |
| Shadow Pinion | Wings | Valkyrie Turner |
| Peachone + Ebony Wings | - | Vandalier（Union，无需Evolution宝石） |

### 进化设计规律

1. **副卡多选一**：多数进化有2-4个可选副卡，降低构建门槛
2. **主题对应**：
   - Whip(鞭打) + Heart(心脏) → Bloody Tear(血泪)
   - Fire Wand(火杖) + Spinach(菠菜/力量) → Hellfire(地狱火)
   - Garlic(大蒜) + Pummarola(番茄酱) → Soul Eater(噬魂者)
3. **Union特殊进化**：Peachone + Ebony Wings 直接合一为 Vandalier，无需Evolution宝石
4. **Phieraggi三合一**：Eight The Sparrow + Phiera Der Tuphello + Tirajisú，唯一三卡进化
5. **副卡类型规律**：攻击卡进化通常需要属性增强卡作为副卡

---

[<< 上一章](05_dungeons_enemies.md) · [返回索引](00_index.md) · [下一章 >>](07_relics_arcanas.md)
