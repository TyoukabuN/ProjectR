## 卡牌数据库设计参考摘要

[<< 上一章](09_partners.md) · [返回索引](00_index.md) · [下一章 >>](11_reroll_beginner.md)

> 本文件聚焦设计参考价值，归纳 CZN 全卡牌的子类型分布、中性牌设计模式、关键词触发类型。
> 完整卡牌列表见：czn.gg/cards/

---

### 卡牌子类型（Subtype）完整分类

| 子类型 | 说明 | 使用者 |
|---|---|---|
| **Starting Card** | 角色起始牌，构成基础牌组，可通过潜能升级 | 各角色专属 |
| **Unique Card** | 角色专属特殊牌，通过Epiphany强化 | 各角色专属 |
| **Neutral Card** | 中性牌，所有或特定职业类别可用，肉鸽中随机获得 | 按职业/全体 |
| **Construct Card** | 生成牌（临时创建），通常带Ephemeral/Exhaust标签 | 生成型角色 |
| **Monster Card** | 怪物牌，肉鸽地图事件中获取 | 全体 |
| **Ego Card** | Ego技能触发的特殊牌 | 各角色 |
| **Status Ailment Card** | 状态异常牌（如Curse、Contamination） | 全体 |

---

### 中性牌（Neutral Card）设计模式归纳

中性牌是 Roguelite 局内随机奖励的核心——设计时需兼顾通用性和与各角色机制的潜在联动。

**按职业限制分类**：

| 适用范围 | 代表牌 | 效果模式 |
|---|---|---|
| Any Ranger/Hunter | Aimed Fire（2费，400%+击败+20信用）、Ambush（0费，Ravage时从弃牌堆回手） | 远程职业共通：高倍率或Ravage联动 |
| Any Striker/Vanguard | Hand-to-Hand Combat（1费，200%防御基础+2层Crystallization）、Devotion（2费，×2防御基础，每层Resolve+20%） | 近战职业：防御基础伤害+防御增益联动 |
| Any Psionic/Controller | Atomic Decomposition（2费，230%穿透全体）| 辅助职业：全体伤害 |
| Any Class | HA-00（2费，×4随机伤害，命中施加Vulnerable或Weaken）、Infection（2费，创建2张Contamination+固定伤）、Automata Cavalry（1费，150%伤+4层Vulnerable） | 无限制：通用输出+削弱 |

**关键中性牌效果模式**：

| 效果模式 | 代表牌 | 设计价值 |
|---|---|---|
| 弃牌堆回手（Ravage触发） | Ambush（0费） | 强化Ravage机制的通用联动牌 |
| 击败奖励 | Aimed Fire（+20信用）、Dark Grip（击败后对随机敌人再次伤害） | 击败本身成为额外收益触发条件 |
| 防御基础×层数系数 | Devotion（×Resolve层数）、Hand-to-Hand（+Crystallization）| 防坦型角色专用增益牌 |
| 创建异常牌 | Infection（创建Contamination）| 异常牌数量与手牌数成为伤害系数 |
| 多段随机命中+状态 | HA-00（×4随机+Vulnerable/Weaken任选）| 多段命中触发多状态，不确定性设计 |

---

### Construct Card（生成牌）设计模式归纳

生成牌通常由角色技能在战斗中临时产生，本身带 Ephemeral（回合结束消失）或 Exhaust（使用后消耗）标签。

| 角色 | 生成牌 | 费用 | 核心效果 | 设计特征 |
|---|---|---|---|---|
| Veronica | Ballista / Enhanced Ballista / Giant Ballista | 0费（被动） | 回合结束自动追击（单体/随机/全体） | 被动触发，无需手动出牌 |
| Lucas | Armor-Piercing/Explosive/Dispersing/Cold/Flame Shot | 1费 | 100%伤+副效果（各不同） | 5种变体，每种施加不同状态；Exhaust触发R.P.G-7固定伤 |
| Renoa | Dirge Bullet / Dirge Bullet: Finale | 1费 | 伤害+进入弃牌堆时追击 | 弃牌动作本身触发额外伤害 |
| Kayron | Futility（Attack/Skill） | 1费 | 80%伤+80%治疗（Attack版）/ 其他（Skill版） | 废牌设计：消耗Futility积累Exhaust层数 |
| Tressa | Shadow Dagger / Advanced Shadow Dagger | 0费/1费 | 50%/100%伤+2/4层Agony | 0费大量产出，搭配Hugo免费触发追击 |
| Luke | HVAP Round / Finisher Round | 0费 | 保证暴击/200%（5次使用后解锁） | 使用次数作为解锁条件（Unusable→Usable） |
| Sereniel | Homing Laser L（升级版） | 0费 | 自动回手 | 通过Shining Core产生，Ravage后再补 |

**设计规律**：
- 生成牌费用通常为0或1，确保不造成AP负担
- 5种变体（Lucas子弹）：相同框架下差异化效果，增加选择维度
- 弃牌即触发（Renoa的Dirge Bullet）：把负面操作（弃牌）转为收益
- 被动触发（Veronica的Ballista）：无操作成本的持续输出

---

### Ego Card（Ego技能牌）设计模式归纳

Ego技能消耗EP（自我意识点数）触发，通常是角色最强的即时爆发效果。

| 角色 | Ego技能 | EP | 效果 | 设计特点 |
|---|---|---|---|---|
| Veronica | Repose | 5 | 摸3张队友牌 | 摸牌型，为队友出牌创造机会 |
| Cassius | Wild Card | 5 | 摸3张牌，下1张攻击牌-1费（持续到使用） | 降费+摸牌组合 |
| Mei Lin | Blazing Brilliance | 5 | 创建5层Embers（Retain），200%×2全体 | 蓄力型，Embers持续燃烧 |
| Kayron | Apocalypse | 6 | 召唤无限制Fatality（极高伤害） | 高EP门槛+极端爆发 |
| Hugo | Absolute Hunter | 4 | 3层Commence the Hunt | 叠层型，增强追击次数 |
| Rin | Dark Mirror | 5 | 黑暗镜：复制上一张使用的牌 | 复制机制，与连击强联动 |
| Mika | Heaven's Hymn | 4 | 4层Wave，全队治疗100% | 资源恢复型 |
| Beryl | Guilty Pleasure | 5 | 摸3张牌，下1张Charged Shot暴伤+100% | 摸牌+单牌增幅 |
| Tressa | Wave of Darkness | 5 | 创建2张Shadow Dagger | 生成型，补充弹药 |
| Nine | Reflection: Zero | 6 | 全体50%防御基础+消耗2张最高费×50% | 消耗型，消耗越贵爆发越强 |
| Rita | Hour of Glory | 5 | 6层Chronal Ascension+本场累计费用×10%加成（最多+200%） | 累积型，越到后期越强 |
| Narja | Predator's Hunting Method | 5 | 10层Voracity+250%防御基础；Mealtime在弃牌堆时+5层Predation | 双重条件型 |
| Tiphera | The Weight of Creation | 5 | 创建全套3种Archetype牌 | 全局辅助型 |

**Ego技能EP费用分布**：
- 2 EP：Partner Ego（低门槛，频繁使用）
- 4-5 EP：多数角色Ego（中等门槛）
- 6 EP：Kayron、Nine、Magna（高门槛，极端效果）

---

### Monster Card（怪物牌）设计模式

怪物牌从肉鸽地图的怪物事件中获取，通常效果强但有副作用或高费：

| 怪物牌 | 费用 | 效果 | 特点 |
|---|---|---|---|
| Automata Cavalry | 1 | 150%伤+4层Vulnerable | 无副作用，强力中性AOE辅助 |
| Beam Shooter | 3 | 350%伤，目标有护盾+50% | 高费，反护盾收益 |
| Dark Grip | 1 | 120%×2伤，击败后再×2随机伤 | 击败触发连锁 |
| Dark Tendrils | - | 多段随机 | - |

---

### 卡牌费用（AP Cost）分布与设计哲学

| 费用 | 定位 | 设计意图 |
|---|---|---|
| **0费** | 零成本触发牌 | 作为免费追击触发器（Tressa的Shadow Dagger）；自动回手弹药（Homing Laser）；条件性免费（Cassius的Triple 0） |
| **1费** | 标准操作牌 | 主力出牌节奏，构成大多数技能/攻击 |
| **2费** | 重型技能 | 高收益，需要资源管理；通常是核心爆发牌 |
| **3费** | 超重型 | 九的Hew、Khalipe的Greatsword；通常需要降费手段配合 |
| **高费Retain牌** | 蓄势大招 | Narja的Mealtime（初始6费降至0）；Beryl的Charged Shot（2费Retain叠层）；Rita的Fate Imprint（费用动态增长） |
| **0费被动** | 不占行动 | Veronica的Ballista系列（Ø费，自动触发） |

*数据来源：czn.gg/cards/，整理日期 2026.04.06*

---

[<< 上一章](09_partners.md) · [返回索引](00_index.md) · [下一章 >>](11_reroll_beginner.md)
