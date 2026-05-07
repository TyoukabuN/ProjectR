# 地牢与敌人

[<< 上一章](04_cards_support.md) · [返回索引](00_index.md) · [下一章 >>](06_gems_evolution.md)

## 地牢系统

地牢是主要玩法场所，每个地牢分为多个楼层（Floor），目标是在每层找到铲子挖到下一层。

### 地牢列表

| 地牢 | 阶段 | 难度 | 楼层数 | 描述 |
|---|---|---|---|---|
| Tutorial | 教学 | - | 1 | 教程 |
| Village | 村庄 | - | - | 枢纽区域，Gorton村 |
| Mad Forest | 疯狂森林 | 1 | 4 | 曾是繁盛之地，现为邪恶倾倒场 |
| Furious Forest | 狂怒森林 | 2 | 5 | 森林深处怪物愈发疯狂 |
| Berserk Wood | 狂暴之林 | 3 | 5 | 深处无回头路，千翼扑扇间隐约低泣 |
| Inlaid Library | 镶嵌图书馆 | 2 | 5 | 被诅咒的贤者图书馆，石面具低语 |
| Library West Wing | 图书馆西翼 | 3 | 5 | 非人形态在书架间徘徊 |
| Library Sanctum | 图书馆圣所 | 4 | 5 | 昔日魔法师殿堂，今只剩暗黑魔法 |
| Teeny Bridge | 小桥 | 3 | 1 | 看似安全的小桥 |
| Dairy Plant | 乳制品工厂 | 4 | 5 | 牛奶魔法的诞生地 |
| Milk Factory | 牛奶工厂 | 5 | 5 | 管道迷宫，巨大魔像守护 |
| Curd Refinery | 凝乳精炼厂 | 6 | 5 | 乳业综合体的核心 |
| Weeny Bridge | 微桥 | 5 | 1 | 比上座桥更小更安静 |
| Gallo Tower | 加洛塔 | 6 | 6 | 科学与巫术之塔 |
| Meany Bridge | 刻桥 | 7 | 1 | 名字吓人但看起来无害 |
| Cappella Magna | 大礼拜堂 | 8 | 5 | 堕落纯洁的交汇点 |
| Cappella Ultima | 终极礼拜堂 | 9 | 5 | 浮空王座厅，傀儡师垂帘之地 |

### 地牢设计规律

1. **难度1-9递增**，从Mad Forest到Cappella Ultima
2. **桥关卡**：Teeny/Weeny/Meany Bridge 都是1层，作为过渡关卡
3. **多阶段地牢**：Mad Forest(3阶段)/Inlaid Library(3阶段)/Dairy Plant(3阶段)/Cappella Magna(2阶段)
4. **每层5层为主**，Gallo Tower最长(6层)
5. **桥梁 = 过渡Boss战**：1层但高难度

## 敌人数据

### Tutorial 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| Bat_COWARD | 1 | 1 | 0 | 0 |
| Bat Tutorial00 | 21 | 1 | 0 | 0 |
| Bat Tutorial01 | 3 | 1 | 0 | 0 |
| Bat Tutorial02 | 20 | 1 | 0 | 4 |
| Bat Tutorial03 | 38 | 2 | 1 | 0 |
| Bat Tutorial04 | 26 | 3 | 1 | 0 |
| Bat Tutorial05 | 30 | 2 | 1 | 0 |

### Mad Forest 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| Bat | 12 | 1 | 1 | 0 |
| Bat Elite | 400 | 30 | 2 | 0 |
| FlowerWall | 60 | 4 | 1 | 6 |
| FlowerWall2 | 200 | 5 | 4 | 20 |
| Ghoul | 40 | 3 | 2 | 6 |
| GiantMummy | 300 | 6 | 4 | 50 |
| GiantMummy Elite | 3,000 | 60 | 14 | 360 |
| MantisElite | 1,200 | 50 | 12 | 120 |
| MantisElite_Ambush | 2,500 | 55 | 15 | 185 |
| Skeleton3 | 25 | 2 | 1 | 5 |
| Skeleton3 Elite | 600 | 35 | 4 | 60 |
| VenusElite | 8,000 | 65 | 14 | 500 |
| Werewolf | 250 | 5 | 3 | 15 |
| WerewolfElite | 2,200 | 55 | 13 | 220 |
| Zombie | 125 | 4 | 2 | 18 |
| Zombie Elite | 750 | 40 | 7 | 75 |

### Inlaid Library 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| ApprenticeWitch | 300 | 6 | 4 | 30 |
| Ecto | 40 | 2 | 1 | 6 |
| EctoElite | 1,000 | 40 | 8 | 100 |
| Ghost | 100 | 3 | 2 | 15 |
| GhostElite | 1,500 | 45 | 9 | 150 |
| GhostElite_Ambush | 2,200 | 50 | 12 | 220 |
| HagElite | 11,000 | 70 | 10 | 300 |
| LionHead | 170 | 4 | 2 | 34 |
| LionHeadElite | 2,000 | 55 | 10 | 200 |
| MasterWitchElite | 5,500 | 65 | 16 | 550 |
| Mudman | 20 | 1 | 1 | 4 |
| NesuferitElite | 4,000 | 50 | 14 | 200 |
| QueenMedusaElite | 3,500 | 60 | 14 | 350 |
| QueenMedusaElite_Ambush | 4,200 | 65 | 14 | 420 |
| ShadeBomb | 80 | 5 | 1 | 0 |
| SneakyHead | 200 | 5 | 3 | 30 |

### Teeny Bridge 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| BridgeGuardian | 400 | 4 | 2 | 40 |
| GiantBat | 600 | 6 | 6 | 30 |
| Impefinger | 150 | 3 | 1 | 20 |
| Raiju | 440 | 5 | 5 | 80 |
| SwordFlint | 30,000 | 80 | 14 | 300 |
| Swordian | 10,000 | 70 | 10 | 0 |
| SwordiLee | 20,000 | 75 | 12 | 300 |

### Dairy Plant 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| ArchonAscia | 570 | 5 | 6 | 40 |
| Gallotrice | 1,000 | 6 | 5 | 0 |
| GallotriceElite | 12,500 | 70 | 14 | 0 |
| Golem | 420 | 4 | 4 | 70 |
| GolemElite | 9,000 | 60 | 10 | 300 |
| LizardPawn | 240 | 2 | 2 | 30 |
| LizardElite | 6,500 | 45 | 10 | 150 |
| Merman | 160 | 1 | 2 | 0 |
| MermanElite | 5,000 | 40 | 8 | 0 |
| MermanElite_Ambush | 6,000 | 45 | 8 | 55 |
| MilkElemental | 100 | 3 | 2 | 30 |
| MilkElementalElite | 6,000 | 45 | 4 | 250 |
| Minotaur | 320 | 3 | 4 | 25 |
| MinotaurElite | 7,500 | 50 | 12 | 80 |
| MinotaurElite_Ambush | 8,000 | 55 | 14 | 300 |
| TwinDemon | 300 | 6 | 2 | 100 |
| LostTwinElite | 10,000 | 65 | 6 | 666 |
| TritontElite | 8,000 | 75 | 12 | 0 |
| GiantArmouredKnightElite | 10,000 | 65 | 16 | 350 |
| GiantArmouredKnightElite_Ambush | 12,000 | 70 | 14 | 250 |

### Gallo Tower 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| ArchonSpada | 1,300 | 7 | 8 | 250 |
| CollosalFlameElite | 20,000 | 65 | 25 | 600 |
| DevilElite | 8,000 | 50 | 10 | 400 |
| Dragonshrimp | 500 | 5 | 6 | 100 |
| GalloElite | 80,000 | 80 | 22 | 600 |
| Ghiavolo | 440 | 4 | 3 | 30 |
| GiantCrabElite | 35,000 | 70 | 15 | 600 |
| GiantCrabElite_Ambush | 40,000 | 75 | 20 | 300 |
| GiantSkulloneElite | 14,000 | 55 | 12 | 450 |
| GiantSkulloneElite_Ambush | 15,000 | 60 | 16 | 600 |
| Harpy | 700 | 6 | 7 | 120 |
| HarpyElite | 40,000 | 70 | 12 | 300 |
| ManticoreElite | 50,000 | 75 | 22 | 550 |
| Scarleton | 480 | 4 | 3 | 50 |
| Skeleton2 | 330 | 3 | 3 | 80 |
| Skullino | 240 | 2 | 2 | 70 |

### Cappella Magna 敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 |
|---|---|---|---|---|
| ArchDemon | 2,000 | 7 | 9 | 250 |
| ArchDemonElite | 66,000 | 75 | 23 | 666 |
| ArchonOro | 600 | 4 | 6 | 100 |
| BeastDemon | 1,200 | 6 | 8 | 65 |
| BeastDemonElite | 50,000 | 70 | 22 | 0 |
| BeastDemonElite_Ambush | 50,000 | 75 | 22 | 0 |
| Durga | 450 | 3 | 5 | 0 |
| EnderEliteP1 | 225,000 | 0 | 30 | 0 |
| EnderEliteP2 | 115,000 | 0 | 6 | 0 |
| EyeBallElite | 28,000 | 60 | 16 | 0 |
| EyeBallElite_Ambush | 28,000 | 65 | 16 | 0 |
| FallenAngel | 350 | 2 | 3 | 50 |
| FallenArchangel | 150 | 2 | 7 | 10 |
| FallenThrone | 800 | 5 | 8 | 60 |
| GreenKnight | 1,000 | 3 | 20 | 40 |
| GreenKnightElite | 36,000 | 65 | 18 | 550 |
| Kali | 1,800 | 6 | 8 | 90 |
| Succubus | 400 | 4 | 10 | 30 |
| SuccubusElite | 50,000 | 70 | 25 | 150 |
| TraineeRedReaper | 2,400 | 8 | 12 | 0 |
| TrinacriaElite | 100,000 | 80 | 22 | 300 |

### 特殊/隐藏敌人

| 名称 | 血量 | 经验 | 最大伤害 | 最大护甲 | 备注 |
|---|---|---|---|---|---|
| RedDeath | 1,000,000 | 0 | 333 | 0 | 死神，百万血量 |
| Trickster | 30,000 | 0 | 30 | 0 | 骗术师 |
| MoonAtlantean | 25,000 | 500 | 15 | 0 | 月之亚特兰蒂斯 |
| SunAtlantean | 25,000 | 500 | 15 | 0 | 日之亚特兰蒂斯 |
| Drowner | 4,000 | 0 | 11 | 0 | 溺亡者 |

### 敌人设计规律

1. **Elite变体**：每个基础敌人都有Elite版本，血量约5-15倍，护甲大幅提升
2. **_Ambush变体**：比Elite更强，伏击型
3. **Guardian系列**：每个地牢一个守卫（Guardian_1到_5），0经验但高护甲
4. **GlowingBat Elite**：跨地牢通用的高经验蝙蝠（T1-T4四个强度等级）
5. **数值膨胀**：Mad Forest杂兵100血 → Cappella Magna杂兵1000+血，Boss从8000到225000
6. **护甲>血量**：Elite敌人护甲常占总耐久40-60%，需先破甲再输出

---

[<< 上一章](04_cards_support.md) · [返回索引](00_index.md) · [下一章 >>](06_gems_evolution.md)
