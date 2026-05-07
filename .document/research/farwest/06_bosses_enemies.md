# Boss 与敌人系统

[<< 上一章](05_objectives.md) · [返回索引](00_index.md) · [下一章 >>](07_maps_progression.md)

---

## 1. 敌人派系

Far Far West 的敌人分为两大派系：

| 派系 | 状态 | 特点 |
|------|------|------|
| **Cryptics** | 已实装 | 亡灵/诅咒主题，当前主要敌人 |
| **Zurkers** | 即将推出 | 侵略者主题，高难度专属 |

**特殊规则**：Special 级别敌人仅在 Hard 及以上难度出现。

---

## 2. Cryptic 敌人分级

### 2.1 Trooper（普通兵）

最基础的敌人类型，数量多，伤害较低。

| 名称 | 描述 |
|------|------|
| **Cryptic Grunt** | 基础近战/远程单位 |
| **Cryptic Raven** | 飞行单位 |
| **Cryptic Kamikaze** | 自爆单位 |

### 2.2 Veteran（老兵）

| 名称 | 描述 |
|------|------|
| **Cryptic Prowler** | 游荡型敌人 |
| **Cryptic Bomber** | 投掷爆炸物 |
| **Cryptic Slinger** | 远程投掷 |
| **Cryptic Deathrattle** | 死亡时触发效果 |
| **Cryptic Minigunner** | 高射速远程 |

### 2.3 Elite（精英）

| 名称 | 描述 |
|------|------|
| **Cryptic Bull** | 冲锋型高伤害 |
| **Cryptic Barricade** | 防御型，可能有护盾/障碍 |
| **Cryptic Nuker** | 高爆发范围伤害 |
| **Cryptic Lich** | 法术型敌人 |

### 2.4 Special（特殊）

*仅在 Hard 及以上难度出现*

| 名称 | 描述 |
|------|------|
| **Cryptic Deadeye** | 精准狙击型 |
| **Cryptic Horseman** | 高机动精英 |

---

## 3. 悬赏 Boss（Bounty Bosses）

悬赏 Boss 是每个任务的最终挑战，完成主要目标后可在 Boss Spawner 处召唤。

### 3.1 Cryptic Necromancer（神秘死灵法师）

- **外观**：巨大的骷髅军阀， wielding 大型镰刀
- **攻击方式**：
  - 大范围猛击
  - 诅咒投射物
  - 召唤亡灵小兵 swarm

### 3.2 Cryptic Vulture（神秘秃鹫）

- **外观**：巨大的骷髅鸟类
- **攻击方式**：
  - 冲击波
  - 龙卷风
  - 诅咒魔法弹幕
  - 召唤 Cryptic Ravens 群

### 3.3 Cryptic Train（神秘列车）

- **外观**：被诅咒的列车残骸，在空中飞行
- **攻击方式**：
  - 向下倾泻诅咒魔法
  - 召唤幽灵列车幻影冲撞竞技场
- **特殊机制**：每节车厢有独立的血条，需要分别摧毁

### 3.4 Cryptic Saloon（神秘沙龙）

- **外观**：被愤怒灵魂附身的沙龙建筑
- **攻击方式**：
  - 禁止玩家进入建筑内部
  - 释放冲击波
  - 诅咒漩涡
- **特殊机制**：需要摧毁建筑外墙木板，最后驱赶灵魂并击杀

---

## 4.  minibosses

Wiki 中 minibosses 部分当前为空，可能存在但未在 Wiki 中记录。

---

## 5. 设计启示

### 5.1 敌人分级系统

Cryptic 派系的四级分类（Trooper → Veteran → Elite → Special）提供了清晰难度曲线：

| 级别 | 定位 | 难度影响 |
|------|------|----------|
| Trooper | 杂兵/气氛组 | 低 |
| Veteran | 需要注意力 | 中 |
| Elite | 可能致命 | 高 |
| Special | 极限挑战 | 仅高难度 |

### 5.2 Boss 设计亮点

1. **形态多样性**：人形（Necromancer）、飞行（Vulture）、载具（Train）、建筑（Saloon）四种完全不同的 Boss 形态
2. **多阶段/多目标**：Train 的多车厢独立血条、Saloon 的"拆房 → 驱灵 → 击杀"流程，避免了"站桩输出"
3. **环境互动**：Vulture 的龙卷风、Saloon 的禁入机制，让 Boss 战不只是"打血条"
4. **召唤机制**：Necromancer 和 Vulture 都会召唤小怪，增加场面混乱度和优先级判断

### 5.3 派系设计启示

- **Cryptic** = 亡灵/诅咒主题 = 已实装 = 基础体验
- **Zurkers** = 侵略者主题 = 即将推出 = 未来内容

这种"双派系"设计为未来更新预留了空间，可以通过新增派系敌人来刷新游戏体验。

### 5.4 难度分层

Special 敌人仅在高难度出现的设计，给硬核玩家提供了额外的挑战目标，同时不让普通玩家感到挫败。

