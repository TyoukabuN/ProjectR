# Dominion 竞品调研 — 目录索引

> Dominion（多米尼恩）是 2008 年发布的 DBG（卡牌构筑游戏）鼻祖。
> 本调研从游戏设计视角整理其核心机制与策略体系，供 ProjectR 开发参考。
>
> 数据来源：wiki.dominionstrategy.com  
> 整理日期：2026.04.30

---

## 文档目录

| 文件 | 内容 |
|------|------|
| [01_overview.md](01_overview.md) | 游戏基本信息、核心矛盾、回合流程、Supply区、牌库循环、扩展一览 |
| [02_cards.md](02_cards.md) | 卡牌类型体系（Action/Treasure/Victory/Curse/Attack/Reaction/Duration/Kingdom）|
| [03_kingdom.md](03_kingdom.md) | 基础版全 25 张王国卡图鉴与策略分析 |
| [04_strategy.md](04_strategy.md) | 五大策略类型（Engine/BigMoney/Combo/Rush/Slog）的定义、构建方法、对阵分析 |
| [05_design_ref.md](05_design_ref.md) | 设计启示总结：Dominion 机制对 ProjectR 的参考价值 |

---

## 核心概念速查

| 概念 | 说明 | 详见 |
|------|------|------|
| **DBG 核心循环** | 购买卡 → 进弃牌堆 → 洗牌 → 抽牌 → 变强 | [01_overview.md#1.7](01_overview.md) |
| **死牌矛盾** | Victory 卡是死牌，但必须购买；何时"变绿"是核心决策 | [01_overview.md#1.4](01_overview.md) |
| **回合四阶段** | Action → Buy → Night（可选）→ Clean-up | [01_overview.md#1.5](01_overview.md) |
| **Supply 供应区** | 所有玩家共享的购买来源；牌堆打空即消失 | [01_overview.md#1.6](01_overview.md) |
| **Terminal/Non-terminal** | 终端卡（消耗行动点）vs 非终端卡（维持行动链）| [02_cards.md#2.2](02_cards.md) |
| **Cantrip** | +1 Card +1 Action 的等量替换卡，不消耗行动点净值 | [02_cards.md#2.2](02_cards.md) |
| **Trash 清牌** | 将卡永久移出游戏，净化牌库的核心手段 | [03_kingdom.md#Chapel](03_kingdom.md) |
| **Engine 引擎** | 行动链构建策略：村庄+终端抽牌+有效载荷 | [04_strategy.md#4.2](04_strategy.md) |
| **Big Money 大钱** | 主买 Treasure，稳定积累购买力 | [04_strategy.md#4.3](04_strategy.md) |
| **Rush 冲刺** | 尽快打空三个 Supply 堆结束游戏 | [04_strategy.md#4.5](04_strategy.md) |
| **Pile Split** | 多玩家争夺同一 Supply 堆的分割博弈 | [04_strategy.md#4.4](04_strategy.md) |
| **金钱密度** | 牌库平均每张牌的金钱产出，衡量购买效率的指标 | [04_strategy.md#4.3](04_strategy.md) |

---
