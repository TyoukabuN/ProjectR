# 卡牌系统与角色卡

[<< 上一章](01_overview.md) · [返回索引](00_index.md) · [下一章 >>](03_cards_attack.md)

## 卡牌类型总览

| 类型 | 颜色 | 说明 |
|---|---|---|
| Character（角色） | 青色 | 场上 Crawler，按颜色触发效果 |
| Attack（攻击） | 红色 | 主要伤害来源 |
| Defense（防御） | 蓝色 | 通常生成护甲 |
| Stat Boost（属性增强） | 黄色 | 临时或永久提升属性 |
| Mana（法力） | 紫色 | 生成法力 |
| Wild（万能） | 白色 | 可在任何时刻打出，不中断Combo |
| Temporary（临时） | 黑色 | Boss插入的debuff卡 |

- **W费用**：部分卡牌费用显示为 W，表示为万能卡费用，不中断Combo
- **Destroy**：打出后销毁，从牌组移除
- **Retain**：抽到时保留在手牌直到使用

## 角色卡系统

角色在村庄的 Gorton Bell Inn 中购买和装备。每个角色有：
- **Card text**：打出时的即时效果
- **Trigger**：场上Crawler触发条件与效果
- **Duration**：触发次数

### 迪斯科模式

购买5个Crawler后，Inn有10%概率触发迪斯科模式，后续购买享5%折扣。离开Inn后不可再次触发，需完成一次地牢后才可重试。

## 角色卡完整数据

| 角色 | 费用 | 解锁条件 | 卡牌效果 | 触发条件与效果 | Duration |
|---|---|---|---|---|---|
| Antonio | 0 | 初始 | 加3护甲 | 红卡打出时伤害+10% | 5 |
| Arca | 0 | 500金币 | 加3法力 | 紫卡打出时加1法力 | 5 |
| Cavallo | 0 | 750金币 | 发射2额外投射物(2Duration) | 黄卡打出时加1 Amount | 2 |
| Christine | 0 | 打出Pentagram | 手牌费用-1(9Duration) | 紫卡打出时缴械1敌人 | 3 |
| Clerici | 0 | 恢复1000HP | 治疗3 | 蓝卡打出时战后治疗1 | 6 |
| Concetta | 1 | 1840金币 | 攻击10%溅射 | 红卡打出时攻击5%溅射 | 10 |
| Dommario | 1 | 5000金币 | Crawler触发2额外次数 | 紫卡打出时40伤害+击退 | 7 |
| Gallo | 1 | 5200金币 | 贪婪+25%金币 | 万能卡打出时+10%金币 | 8 |
| Gennaro | 1 | 600金币 | 发射2额外投射物 | 红卡打出时90伤害 | 5 |
| Giovanna | 1 | Library Sanctum棺材 | 加20%幸运 | 紫卡打出时抽1牌 | 6 |
| Imelda | 0 | 10金币 | 加18经验 | 黄卡打出时+1%成长 | 5 |
| Krochi | 1 | 击败6666敌人 | 加10%复活 | 万能卡打出时+5%复活 | 10 |
| Lama | 1 | 10%诅咒完成地牢 | 伤害+50% | 蓝卡打出时伤害+15% | 5 |
| MissingN0 | 0 | 6666金币 | 加4护甲 | 红卡打出时抽2牌 | 170 |
| Mortaccio | 0 | 击败444骷髅 | 发射2额外投射物 | 蓝卡打出时+1投射物 | 7 |
| O'Sole | 1 | Gallo Tower击败50龙虾 | 发射3额外投射物 | 红卡打出时+5%幸运 | 4 |
| Pasqualina | 1 | 1100金币 | 攻击10%溅射 | 紫卡打出时手牌+1 | 2 |
| Poe | 1 | 500金币 | 攻击20%溅射 | 蓝卡打出时抽1牌 | 3 |
| Poppea | 1 | Milk Factory棺材 | 手牌+1 | 黄卡打出时Crawler+1触发 | 9 |
| Porta | 0 | Lightning Ring打出100次 | 投射物50%多命中(11Duration) | 红卡打出时加1法力 | - |
| Pugnala | 1 | Berserk Wood棺材 | 伤害+20% | 黄卡打出时抽1牌 | 3 |
| Ramba | 1 | 击败Milk Elemental | 发射3额外投射物 | 紫卡打出时+1 Amount | - |

### 角色设计规律

1. **0费角色**：12个，多为初始或低门槛解锁，效果偏基础（护甲/法力/投射物）
2. **1费角色**：10个，效果更强或更专精（溅射/幸运/手牌/Crawler触发）
3. **触发颜色分布**：
   - 红卡触发：7个（Antonio/Gennaro/O'Sole/Porta/Concetta/MissingN0/...）
   - 蓝卡触发：4个（Lama/Poe/Mortaccio/Clerici）
   - 紫卡触发：6个（Arca/Christine/Giovanna/Pasqualina/Dommario/Ramba）
   - 黄卡触发：4个（Cavallo/Imelda/Poppea/Pugnala）
   - 万能卡触发：2个（Gallo/Krochi）
4. **MissingN0**：特殊角色，Duration高达170，明显是彩蛋/隐藏角色
5. **Divano**：在Wiki角色列表中标为 Unused，未实装的角色

---

[<< 上一章](01_overview.md) · [返回索引](00_index.md) · [下一章 >>](03_cards_attack.md)
