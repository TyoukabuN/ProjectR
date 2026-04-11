
> 来源：czn.gg 各角色 Build 页面

---

### Haru —— 使用次数累积（Pulverize Stack）流

**核心机制**：Anchor Shot 每使用一次，后续使用时额外伤害 +60～+90%（最多叠5次）；Anchor Pointer 将 Anchor Shot 从牌库/坟墓移入手中

| 核心牌 | 费用 | 关键效果 |
|-------|------|---------|
| Anchor Shot | 2 | 240-320%伤，**每次使用累积+60~90%**（最多5次=+300-450%） |
| Anchor Pointer | 0 | 将 Anchor Shot 从牌库或坟墓移入手牌，+25%暴击率（E5） |
| Lift Anchor | 0 | 手中 Anchor Shot 费用+1、伤害+70%（Retain，增强后打出） |
| Charge Energy | 0/1 | 攻击牌+20～30%伤（Retain，下回合仍有效） |
| Power Charge | 2 | 270%全体伤（AoE补充） |

**Manifest Ego 2 关键效果**：Anchor Shot 可以在 AP 不足时仍能使用，每少1 AP则伤害-40%——"超额使用"机制，允许在穷途末路时倾尽资源

**关键设计**：
- Anchor Shot 不进坟墓后洗牌重循环，通过 Anchor Pointer 直接检索→形成"精准检索型"循环，比随机循环更稳定
- 使用次数本身是计数器，5次满层后 Anchor Shot 就是当局最强单体炸弹

**对本游戏启示**：使用次数作为倍率累积，配合精准检索保证能持续使用核心牌，形成"从低到高"的战斗弧线感。

---

### Nia —— 分贝（Decibel）触发流

**核心机制**：积累 Decibel（分贝）层数，当牌库被洗牌（Shuffle）时，按层数触发额外攻击

| 核心牌 | 费用 | 关键效果 |
|-------|------|---------|
| G Chord | 1 | **1 Decibel** + 查看牌库顶3张，选1张直接激活（不花费AP） |
| Adagio | 0 | 丢弃1张，抽该牌拥有者的2张牌（抽弃循环引擎） |
| Soul Rip | 1 | 治疗120%，本回合每次有牌被丢弃：额外治疗+1 Decibel |
| Mute Accent | 1 | 治疗150%，激活手中费用最低的1张牌 |
| 大招 Rock and Roll | EP5 | 200%全体伤 + 3 Decibel + **丢弃整个牌库**（立刻触发洗牌→Decibel大爆发） |

**Decibel 触发逻辑**：Shuffle 时每层 Decibel = 拥有角色触发1次额外攻击（Nia担当额外攻击输出）
**G Chord 激活机制**：无需花费AP直接执行牌的效果——等于免费行动，是 Nia 的核心效率来源

**Manifest Ego 2**：每次洗牌触发，获得4层 Rhythm Boost（每层+5%伤，最多4层）→ Shuffle 既触发 Decibel 爆发，又获得临时增伤

**对本游戏启示**：
- Decibel = "洗牌触发爆发"的条件计数器，使"牌库洗牌"这一普通操作变成有节奏感的奖励节点
- G Chord 的"激活但不花费AP"机制 = 语言卡的"被动触发语素"概念，可直接参考

---

### Selena —— 标记（Mark）+协调攻击（Coordinated Attack）流

**核心机制**：为敌人施加 Mark 层数，标记状态下己方使用攻击牌时触发协调攻击（Coordinated Attack = Selena 追加攻击）

| 核心牌 | 费用 | 关键效果 |
|-------|------|---------|
| High-Power Scope | 1 | 施加2 Mark（E5：每回合开始对全体施1 Mark） |
| Target Spotted | 1 | 150%伤，Attunement：对随机敌人追加200%伤 |
| Drone Bombing | 1 | 全体120%伤 + 1 Passion Weakness（属性弱点加成） |
| Sniper's Domain | 0 | +40% Mark 伤害（Unique升级牌） |

**Attunement（调音）机制**：Target Spotted 连续使用后触发 Attunement 效果，随着叠加次数增加额外效果

**团队协作循环**：
1. Selena 施加 Mark → Mei Lin 使用攻击牌触发 Coordinated Attack → Selena 追加伤害 → Mei Lin 再次触发...
2. Drone Bombing 附加全体 Passion Weakness → Mei Lin Passion 属性攻击全部×1.25

**对本游戏启示**：Mark 机制 = 给敌人挂上"悬赏标签"，后续攻击自动产生追击；与 Hugo 的 CttH 不同，Mark 挂在敌人身上，任何人攻击都能触发，更适合多角色协作。

---

### Owen —— 风击（Wind Charge）多段叠层

**核心机制**：Wind Charge 给下一张攻击牌附加+3段（代价：伤害-50%），配合高倍率AoE牌实现"低倍率×多段"的总输出

| 核心牌 | 费用 | 关键效果 |
|-------|------|---------|
| Wind Charge | 1 | 下一张攻击牌**+3段，-50%伤** |
| Wind Slash | 1/2 | 220%全体AoE（+3段 = 660%全体伤，-50%后实际330%) |
| Break Armor | 0 | 100%伤 + 2层易伤（辅助提高全队输出） |
| Wind Riding | 1 | 使用 Wind Charge 时：120%护盾 + 120%全体伤（变相补偿损失） |
| Gale Strike | 2 | 250%单体 + 80%护盾（攻防兼备） |

**Wind Charge E2**：改为给任意角色的攻击牌+1段（不再限于Owen自己），成为全队强化工具
**Wind Charge E3**：变为升级牌，30%概率使用攻击牌时自动+1段（被动触发，不消耗行动）

**对本游戏启示**：
- Wind Charge 是"伤害数量 vs 伤害倍率"的取舍机制，对多段攻击型角色（Mei Lin、Rin）增益极大，对单次高倍率型角色增益有限
- Wind Riding 将"使用 Wind Charge 的行为本身"变成附加产出，减少了纯辅助行动的浪费感

---

## 二十、更多角色机制深析（四）

### Lucas — 子弹工厂 + AoE 散弹流（Hunter / Passion）

**核心机制：Launcher Bullet 生成系统**
- **Extended Magazine**（0费 Upgrade，Initiation标签）：每回合开始自动生成1枚 Launcher Bullet
- Launcher Bullet 是一次性临时牌（Ephemeral + Exhaust），五种变体：
  - Armor-Piercing Rounds：击中后+30%对有护盾目标
  - Explosive Shot：打中后对全体1层Weaken
  - Dispersing Shot：对全体1层Impair
  - Cold Shot：对全体+1行动计数（减速敌人）
  - Flame Shot：对全体1层Agony

**输出核心**
- **S.S.S**（1费 Skill）：2回合内+40%子弹牌伤害，自身无攻击效果
- **Flame Thrower**（2费 Attack）：270%全体AOE，暴击时额外150%全体
- **Flashbang**（1费 Attack）：180%全体，对有护盾敌人+75%
- **R.P.G-7**（1费 Upgrade，Unique）：每当Bullet牌被Exhaust时，40点固定伤害

**Epiphany 关键升级**
- Extended Magazine Ep5：消耗型，一次创建5枚子弹（Exhaust），爆发用
- S.S.S Ep3：打出后+60%乘以手牌中子弹数量的伤害——手牌管理有价值
- Flame Thrower Ep4：消耗最多2枚子弹，AOE+50%/枚——弹量×伤害
- Flashbang Ep3：2层Passion Weakness（Retain）——持久属性削弱

**潜能解锁**
- Potential 3-1：给Machine Gun添加Bullet标签，S.S.S因此能buff基础攻击
- Potential 7：子弹牌+5%伤害，暴伤>240%再+10%——堆暴伤有意义

**设计模型**：「弹药资源管理」——自动生成临时弹药消耗，每枚有不同效果，构成牌面分配决策；R.P.G-7把消耗动作本身变成固定伤害源，形成"打弹药=被动AOE"的复利机制。

---

### Magna — 防御基弹流（Vanguard / Justice）

**核心机制：Crystallization + Counterattack 叠层**
- **Ice Fragment**（0费 Upgrade）：施加2层Crystallization + 每回合开始+1层Counterattack
  - Crystallization：敌方攻击时触发自动反击（次数消耗）
  - Counterattack：状态层数 = 剩余自动反击次数
- **Ice Wall**（2费 Skill）：100%护盾 + 2层Counterattack + 1回合全体共享Counterattack效果

**攻防兼备**
- **Glacial Iron Fist**（2费 Attack）：210%防御基础伤害，伤害的50%转化为固定护盾（上限20%最大HP）
- **Frost Charge**（1费 Skill）：100%护盾 + 对全体2层Vulnerable（脆弱，受伤害+%）

**Ego技能**：Steel Resolve的灵感对象 Absolute Zero — 300%防御基础全体伤害，每层Counterattack+20%伤害（6 EP）

**Epiphany 关键升级**
- Ice Fragment Ep5：立刻给4层Counterattack + 下一次Counterattack+30%伤害
- Ice Wall Ep4：Counterattack期间对全体造成护盾量等值伤害——护盾即攻击
- Frost Charge Ep1：每个敌人额外+50%护盾增益——多敌场景护盾暴增

**设计模型**：「防御即攻击」——护盾/Crystallization和Counterattack形成双重保护，同时Counterattack是乘算输出系数，防御堆叠越高爆发越强；Glacial Iron Fist的"伤害反哺护盾"形成攻防增强闭环。

---

### Amir — 金属化（Metalization）蓄势爆发流（Vanguard / Order）

**核心机制：Metalization 层数 + 防御基础伤害**
- **Metalization（金属化）**：Amir身上叠加的层数状态，不同卡牌消耗层数时造成额外防御基础伤害
- **Hovering Metal**（1费 Skill）：3层Damage Reduction + 4层Metalize
- **Metal Extraction**（1费 Skill）：2层Metalization + 1回合Skill牌护盾+30%
- **Full Metal Hurricane**（2费 Attack）：200%防御基础全体伤害，1回合内全体敌人被Metalization效果瞄准，+60%伤害

**Unique牌**
- **Iron Skin**（1费 Skill）：1回合-20%受伤害，消耗Metalization层数，每消耗1层再-20%受伤 —— 防御缩减与层数的权衡

**Ego技能**：Steel Resolve — 按Metalize层数获得护盾（120%/层），清除所有层数，下回合开始重新获得3层（4 EP）

**潜能**
- Potential 3-1：使用Steel Barrier时获得1层Metallicize（基础牌触发层数积累）
- Potential 7：+10点Metalization伤害；防御>300时再+10%

**搭配逻辑**：与Khalipe搭配，Khalipe提供护盾触发Metalization的护盾条件；Mika提供Wave持续AP

**设计模型**：「双态切换蓄势」—— 蓄积Metalization层数是输入，Full Metal Hurricane是释放；Iron Skin是"消耗层数换防御"的风险选项，形成层数分配决策；Ego的清空重置机制是"强制结算"设计，避免层数无限积压。

---

### Maribell — 护盾转化固定伤害流（Vanguard / Passion）

**核心机制：Shelter Strike — 护盾转伤害**
- **Shelter Strike**（1费 Attack，Unique）：造成等于当前护盾量的**固定伤害**
  - 固定伤害不受防御减免，是核心输出技
  - 护盾量 = 伤害量：堆护盾即堆伤害

**护盾积累体系**
- **Shelter Defense / Shelter Hold**：基础护盾牌，Potential 3-1后使用时+1层Counterattack
- **Resolute Blitz**（1费 Attack）：100%防御基础伤害 + 1层Counterattack
- **Wolves' Dome**（1费 Upgrade）：2层Counterattack + 每回合开始+1层Counterattack

**Ego技能**：Unbreakable — 350%护盾（4 EP）

**Epiphany 关键升级**
- Wolves' Dome Ep3：每回合开始+2层Counterattack（替代2+1）
- Wolves' Dome Ep5：每回合+2层Counterattack + Counterattack伤害+20%
- Resolute Blitz Ep3：150%防御基础全体伤害，按命中目标数获得对应Counterattack层数
- Oh... I See. Ep4：回合结束时，护盾量×50%固定伤害打击HP最低敌人——被动AOE

**与Magna协同**：两者都以护盾+Counterattack为核心，Magna的Frost Charge提供Vulnerable进一步放大Shelter Strike输出

**设计模型**：「护盾即货币」—— 护盾不只是生存工具，而是直接等价于输出的"战斗资源"；每张护盾牌同时完成防御+输出两个目标，实现零浪费；Counterattack叠层与护盾量共同构成"时间越长越强"的滚雪球结构。

---

## 二十一、更多角色机制深析（五）

### Tressa — 暗影匕首（Shadow Dagger）+苦痛（Agony）持续伤害流（Psionic / Void）

**核心机制：Shadow Dagger 生成 + Agony 叠层**
- **Unsheathe Dagger**（0费 Skill）：创建2张 Shadow Dagger 临时牌
- **Shadow Dagger**（0费 Attack，生成牌）：50%伤害 + 施加2层Agony
- **Agony（苦痛）**：持续伤害状态，每层在敌人回合结束/行动时持续造成伤害
- **Cursed Gouge**（1费 Skill，Unique）：2层 Engrave Agony（深度刻印，永久性或高层Agony）+ 2层Weaken

**核心输出牌**
- **Vital Attack**（2费 Attack）：80%×3段伤害，目标处于Agony状态时+50%伤害
- **Curse**（主动）：使用攻击牌时对随机敌人施加Agony（持续2回合）

**Epiphany 关键升级**
- Unsheathe Dagger Ep2：创建3张Shadow Dagger（每回合0费产出3张）
- Shadow Reload Ep4：当Skill牌治疗时，创建1张Shadow Dagger（与Mika的治疗触发连动）
- Shadow Reload Ep1：消耗手中所有Shadow Dagger，对全体80%AOE+2层Agony，每消耗1张追加1次
- Vital Attack Ep4：200%×2段伤害，目标Agony≥3时额外触发1次（Agony门槛触发）
- Vital Attack Ep5：+10%伤害/层Agony——鼓励最大化Agony叠层
- Curse Ep4：回合开始对全体施加2层Agony，再用主动再叠——被动AOE开幕

**与Hugo协同**：每次打出Shadow Dagger触发Hugo的Commence the Hunt追击，0费牌变成免费追击触发器

**与Cassius协同**：Cassius Triple 0产出大量0费Shadow Dagger→同一回合完成10+次触发

**设计模型**：「低费高频 DOT 引擎」——0费牌批量产生施加了Agony的攻击，配合伤害随Agony层数提升的放大牌形成"持续施毒+爆发收割"两阶段结构；与追击系统（Hugo）的天然契合验证了"0费牌=免费追击机会"的联动逻辑。

---

### Rei — 攻击牌增幅（Attack Buff）控制型辅助（Controller / Void）

**核心机制：单回合攻击力放大**
- **Strike of Darkness**（1费 Attack，Lead标签）：150%伤害 + 本回合内+150%基础攻击牌伤害
  - Lead标签：回合开始时处于初始位置，可优先出牌放大后续攻击
- **Dark Condensation**（1费 Skill）：选择1张攻击牌，该牌本回合+150%伤害
- **Resonating Darkness**（1费 Upgrade）：Void属性牌+40%伤害
- **Predator's Blade**（1费 Attack，Unique）：250%伤害 + 2层Morale（1回合）

**辅助工具**
- **Snack Time**（0费 Skill）：选1张牌消耗，治疗150%，摸1张——牌组清理+治疗
  - Ep1：治疗150% + 摸2张（Retain）——每回合免费摸牌
  - Ep2：最多消耗2张，摸等量牌

**Ego技能**：Final Shadow — 300%伤害 + 获得1 AP（4 EP）——在burst回合恢复费用续接操作

**Epiphany 关键升级**
- Strike of Darkness Ep3：Upgrade形式，直接+80%基础攻击牌伤害（队友也受益）
- Strike of Darkness Ep4：激活手中所有基础牌（0费，Retain）——多次激活+免费使用
- Resonating Darkness Ep1：1费以下卡牌+60%伤害（覆盖所有0费和1费牌）
- Resonating Darkness Ep5：0费Upgrade形式，1费牌+80%伤害持续1回合

**与Rin搭配**：Rin的Dark Mist Blade Art每次技能牌使用增加击数，Rei的攻击倍率放大每一击的伤害——叠击数×高倍率

**设计模型**：「弹弓式增幅」——Rei本身的攻击牌不强，但通过Attack前置Buff（Strike of Darkness）、指定牌增幅（Dark Condensation）、被动属性增益（Resonating Darkness）三层叠加，把队友的一次攻击变成数倍伤害；Lead标签确保增幅先于伤害触发，是时序控制的具象化设计。


---

## 二十二、新增角色机制深析（Season 2）

### Sereniel — 归巢激光（Homing Laser）+Ravage 反复回收流（Hunter / Instinct）

**核心机制：Homing Laser 自动返回手牌**
- **Homing Laser**（0费 Attack）：100%伤害 + 1层Afterglow；在 **Ravage** 状态下或回合开始时，自动从弃牌堆移回手牌
  - 即：每回合开始自动补充激光，Ravage额外再补一次——0费弹药永远有
  - Ep4（最推荐）：改为"Ravage或回合开始时移回手牌"——每回合稳定持有4+张激光

**核心输出体系**
- **Shining Core**（1费 Upgrade）：创建2张Homing Laser L；Ravage时再创建2张
  - Ep3（最推荐）：添加Initiation标签变升级牌，同时Ravage后创建2张升级激光——单回合超过7张激光
- **Cobalt Light**（1费 Attack）：对随机敌人造成120%伤害，手牌中每有1张Homing Laser+1击
  - Ep4：击数 = 手中Homing Laser数量——6+张激光=6+击全伤
- **Pale Shooting Star**（2费 Attack，Unique）：150%伤害 + 激活弃牌堆/抽牌堆中3张Homing Laser

**Ego技能**：Death Halo — 对全体400%伤害 + 将消耗牌堆中所有Homing Laser L移回手牌（6 EP）

**设计模型**：「自动弹药回收引擎」——0费激光通过Ravage状态实现自我补充，无需额外资源；Ravage→激光回手→大量激光在手→Cobalt Light打击次数爆炸→再次触发Ravage，构成自增强循环。Afterglow是副产物，激光数量才是核心。

---

### Narja — 贪食（Voracity）+猎食（Predation）蓄势大招流（Controller / Instinct）

**核心机制：Voracity 层数 + Mealtime 大招**
- **Voracity（贪食）**：层数资源，每次"Voracity被激活"触发效果（类似Mika的Wave）
- **Mealtime**（6费 Attack，Unique，Retain）：Retain状态，每次Voracity减少时费用-1、治疗量+30%（最多-10费），最终发动时160%全体防御基础伤害+治疗100%+4层Predation
  - 即：通过大量激活Voracity把6费大招降到0费发动

**蓄积工具**
- **Voluntary Control**（1费 Skill）：Voracity<6时获得6层，≥6时获得2层Predation
- **Bottomless Hunger**（0费 Skill）：下2次Voracity激活时，随机降低1张手牌费用1点（1回合）
- **Shackles of Hunger**（1费 Skill，Retain）：消耗最多5层Voracity，下1张攻击牌+40%伤/层；消耗5层→3层Predation

**Epiphany 关键升级**
- Bottomless Hunger Ep5（Lead）：7层Voracity + 1回合内每次Voracity激活，下次Mealtime+40%伤害
- Voluntary Control Ep4（Unique）：10层Voracity + 选择1张攻击牌，每次该牌进入手牌时+1层Predation
- Voluntary Control Ep5（Unique）：基础攻击牌每次命中+1层Voracity + 基础攻击牌+50%伤害
- Domain of Voracity Ep4（Retain）：320%防御基础全体伤害，若同时持有Voracity和Predation则再触发1次

**Ego技能**：Predator's Hunting Method — 10层Voracity + 250%防御基础伤害；若Mealtime在弃牌堆则+5层Predation（5 EP）

**设计模型**：「降费大招蓄力」——Mealtime是一张初始6费的Retain大招，通过每次减少Voracity降费，玩家目标是：在Retain期间尽可能多次触发Voracity减少，将大招免费化；Shackles是"提前消耗换收益"的取舍选项，营造层数分配张力。

---

### Nine — 焚化（Incinerate）升级连锁大招流（Vanguard / Order）

**核心机制：Hew 连续升级链**
- **Hew**（3费 Attack，Unique，Exhaust，Ephemeral，Initiation）：350%防御基础伤害；**Incinerate：创建1张Hew Lv.1**
  - 每次打出Hew后自动创建升级版，形成链式升级：
  - Hew → Hew Lv.1 → Hew Lv.2 → ... → Hew (Extreme)（420%+，最强形态）
  - Ep2（推荐）：升级为Hew (Extreme)路线，300%→Extreme

**支援体系**
- **Fighting Spirit**（1费 Skill）：激活手中费用最高的攻击牌，然后消耗那些牌——免费发动Hew
- **Counterblade**（0费 Upgrade，Unique）：每当有牌被Exhaust时，获得1层Honed Edge
- **Experienced Strike**（1费 Attack）：210%防御基础伤害 + 20%伤害护盾 + 1回合内Exhaust牌伤害+30%
- **Fatal Strike**（1费 Attack）：210%伤害 + 按手中最高费消耗牌费用×30%额外伤害，然后消耗它

**Ego技能**：Reflection: Zero — 对全体50%防御基础伤害 + 手中费用最高2张牌按总费用×50%额外伤害并消耗它们（6 EP）——触发Hew的Incinerate

**Epiphany 关键**
- Hew Ep2：Incinerate创建Hew (Extreme) Lv.1，更快到达最强形态
- Hew Ep1：400%伤害 + 200%护盾 + Incinerate创建Hew (Ironclad)——防御兼顾型
- Hew Ep4：400%全体AOE（每目标-10%）——多敌模式
- Counterblade Ep5：回合开始激活手中Hew 1次——被动触发Incinerate链

**设计模型**：「升级链增压」——Hew本身不是最强形态，每次使用都在为下次打出更强版本铺路，形成"当下吃亏、未来爆发"的延迟满足节奏；Fighting Spirit把激活与消耗分开，让Incinerate触发不消耗操作次数。

---

### Tiphera — 原型牌（Archetype）全能辅助流（Controller / Order）

**核心机制：Archetype 三种牌的全队辅助**
- **Quantum Seed**（0费 Skill，Unique）：治疗100% + 在抽牌堆创建2张Archetype牌 + 摸其中1张
- Archetype 三类牌（创建后从牌堆抽取）：
  - **Archetype: ○**（AP回复类）：给予队友AP
  - **Archetype: △**（伤害增幅类）：增加伤害倍率
  - **Archetype: □**（摸牌/护盾类）：给予摸牌或防御效果

**核心牌组**
- **Form Convergence**（1费 Skill，Retain）：摸2张Archetype牌，随机1张效果翻倍持续1回合
- **Creation and Destruction**（1费 Attack）：120%防御基础伤害 + 80%护盾 + 治疗80%；**Harmonization：所有效果翻倍**（当各种Archetype效果叠加后Harmonize触发）
- **Dual Creation**（1费 Upgrade，Unique）：每当2张Archetype牌被创建时，在弃牌堆再创建1张相同的
- **Event Horizon**（2费 Skill，Unique，Form Upgrade 0/3）：+1 AP + 摸1张 + 下张攻击牌首击+40%伤

**Epiphany 关键**
- Quantum Seed Ep1：创建3张Archetype牌，治疗150%
- Quantum Seed Ep5（Upgrade）：AP=0时自动创建全套3种Archetype牌（每回合1次）——被动资源生成
- Form Convergence Ep5：持续摸Archetype牌直到连续两次相同（最多6次）——极端摸牌
- Creation and Destruction Ep3（Retain）：每次Retain效果激活，所有效果+100%（最多4次）——等待4回合=效果×4

**设计模型**：「多态资源生成辅助」——三种Archetype牌代表AP/伤害/摸牌三类不同资源，Tiphera通过选择抽取哪种来动态匹配队伍当前需求；Creation and Destruction的Harmonization是"全面收益"的奖励触发，鼓励多类Archetype同时维持。

---

### Rita — 编年史（Chronicle）+时序上升（Chronal Ascension）AP自给高费爆发流（Psionic / Justice）

**核心机制：Chronicle 蓄积 → Chronal Ascension 转化 → 自给AP打高费牌**
- **Chrono Archon**（1费 Upgrade，Unique，Initiation）：每当有牌被激活时，按该牌费用获得等量Chronicle层数
  - 例：激活2费牌→+2层Chronicle；激活3费牌→+3层
- **Time Paradox**（0费 Skill，Unique）：清空所有Chronicle层，每减少1层获得1层**Chronal Ascension**
  - Chronal Ascension（时序上升）：Rita的个人资源，用于打高费牌和其他效果

**核心输出**
- **Time Axis Collapse**（2费 Attack）：300%伤害 + 手牌中所有牌**总费用×20%额外伤害**（最多+200%）——手牌越贵伤害越高
- **Future Decreed**（1费 Skill）：将手中1张2费以下的牌费用+1，激活1张随机3费牌——消耗低费换激活高费
- **Chrono Circle**（1费 Skill）：创建2张Fate Imprint

**生成牌**
- **Fate Imprint**（2费 Attack，Exhaust，Retain）：按此牌当前费用×120%伤害；使用后手中所有Fate Imprint费用+1，离开手牌时重置为2费
  - 即：第1张=2费×120%=240%；第2张变3费=3×120%=360%；以此类推——连打Fate Imprint越来越贵越来越强
- **Fate Distortion**（0费 Skill，Ephemeral，Exhaust）：下1张攻击牌+30%伤害，每费再+10%

**Ego技能**：Hour of Glory — 6层Chronal Ascension + 下1张攻击牌+10%伤害/本场累计激活牌总费用（最多+200%）（5 EP）——本场打得越多，Ego爆发越强

**Epiphany 关键**
- Time Axis Collapse Ep1：450%伤害（同样按手牌总费用×20%加成）
- Time Axis Collapse Ep4（3费，Retain）：400%伤害+2层Vulnerable；获得Chronal Ascension时若在手中自动激活——被动爆发
- Future Decreed Ep1（0费）：0费激活随机3费牌——免费触发高费牌
- Chrono Circle Ep4：消耗手中所有Exhaust牌，按总费用创建等量Fate Imprint——废牌变弹药
- Chrono Archon Ep2：获得Chronal Ascension时，按层数对随机敌人造成100%伤害——时序上升=被动攻击

**设计模型**：「费用经济飞轮」——Chrono Archon把"激活高费牌"这个行为本身变成Chronicle资源的输入；Time Paradox把Chronicle转化为可以打高费牌的Chronal Ascension；Time Axis Collapse让手牌里囤积的高费牌直接转化为伤害系数。三个环节形成闭环：激活高费牌→Chronicle→Chronal Ascension→维持高费手牌→更高伤害。

---

## 二十二、新增角色机制深析（Season 2）

### Sereniel — 归巢激光（Homing Laser）+Ravage 反复回收流（Hunter / Instinct）

**核心机制：Homing Laser 自动返回手牌**
- **Homing Laser**（0费 Attack）：100%伤害 + 1层Afterglow；在 **Ravage** 状态下或回合开始时，自动从弃牌堆移回手牌
  - 即：每回合开始自动补充激光，Ravage额外再补一次——0费弹药永远有
  - Ep4（最推荐）：改为"Ravage或回合开始时移回手牌"——每回合稳定持有4+张激光

**核心输出体系**
- **Shining Core**（1费 Upgrade）：创建2张Homing Laser L；Ravage时再创建2张
  - Ep3（最推荐）：添加Initiation标签变升级牌，同时Ravage后创建2张升级激光——单回合超过7张激光
- **Cobalt Light**（1费 Attack）：对随机敌人造成120%伤害，手牌中每有1张Homing Laser+1击
  - Ep4：击数 = 手中Homing Laser数量——6+张激光=6+击全伤
- **Pale Shooting Star**（2费 Attack，Unique）：150%伤害 + 激活弃牌堆/抽牌堆中3张Homing Laser

**Ego技能**：Death Halo — 对全体400%伤害 + 将消耗牌堆中所有Homing Laser L移回手牌（6 EP）

**设计模型**：「自动弹药回收引擎」——0费激光通过Ravage状态实现自我补充，无需额外资源；Ravage→激光回手→大量激光在手→Cobalt Light打击次数爆炸→再次触发Ravage，构成自增强循环。Afterglow是副产物，激光数量才是核心。

---

### Narja — 贪食（Voracity）+猎食（Predation）蓄势大招流（Controller / Instinct）

**核心机制：Voracity 层数 + Mealtime 大招**
- **Voracity（贪食）**：层数资源，每次"Voracity被激活"触发效果（类似Mika的Wave）
- **Mealtime**（6费 Attack，Unique，Retain）：Retain状态，每次Voracity减少时费用-1、治疗量+30%（最多-10费），最终发动时160%全体防御基础伤害+治疗100%+4层Predation
  - 即：通过大量激活Voracity把6费大招降到0费发动

**蓄积工具**
- **Voluntary Control**（1费 Skill）：Voracity<6时获得6层，≥6时获得2层Predation
- **Bottomless Hunger**（0费 Skill）：下2次Voracity激活时，随机降低1张手牌费用1点（1回合）
- **Shackles of Hunger**（1费 Skill，Retain）：消耗最多5层Voracity，下1张攻击牌+40%伤/层；消耗5层→3层Predation

**Epiphany 关键升级**
- Bottomless Hunger Ep5（Lead）：7层Voracity + 1回合内每次Voracity激活，下次Mealtime+40%伤害
- Voluntary Control Ep4（Unique）：10层Voracity + 选择1张攻击牌，每次该牌进入手牌时+1层Predation
- Voluntary Control Ep5（Unique）：基础攻击牌每次命中+1层Voracity + 基础攻击牌+50%伤害
- Domain of Voracity Ep4（Retain）：320%防御基础全体伤害，若同时持有Voracity和Predation则再触发1次

**Ego技能**：Predator's Hunting Method — 10层Voracity + 250%防御基础伤害；若Mealtime在弃牌堆则+5层Predation（5 EP）

**设计模型**：「降费大招蓄力」——Mealtime是一张初始6费的Retain大招，通过每次减少Voracity降费，玩家目标是：在Retain期间尽可能多次触发Voracity减少，将大招免费化；Shackles是"提前消耗换收益"的取舍选项，营造层数分配张力。

---

### Nine — 焚化（Incinerate）升级连锁大招流（Vanguard / Order）

**核心机制：Hew 连续升级链**
- **Hew**（3费 Attack，Unique，Exhaust，Ephemeral，Initiation）：350%防御基础伤害；**Incinerate：创建1张Hew Lv.1**
  - 每次打出Hew后自动创建升级版，形成链式升级：
  - Hew → Hew Lv.1 → Hew Lv.2 → ... → Hew (Extreme)（420%+，最强形态）
  - Ep2（推荐）：升级为Hew (Extreme)路线，300%→Extreme

**支援体系**
- **Fighting Spirit**（1费 Skill）：激活手中费用最高的攻击牌，然后消耗那些牌——免费发动Hew
- **Counterblade**（0费 Upgrade，Unique）：每当有牌被Exhaust时，获得1层Honed Edge
- **Experienced Strike**（1费 Attack）：210%防御基础伤害 + 20%伤害护盾 + 1回合内Exhaust牌伤害+30%
- **Fatal Strike**（1费 Attack）：210%伤害 + 按手中最高费消耗牌费用×30%额外伤害，然后消耗它

**Ego技能**：Reflection: Zero — 对全体50%防御基础伤害 + 手中费用最高2张牌按总费用×50%额外伤害并消耗它们（6 EP）——触发Hew的Incinerate

**Epiphany 关键**
- Hew Ep2：Incinerate创建Hew (Extreme) Lv.1，更快到达最强形态
- Hew Ep1：400%伤害 + 200%护盾 + Incinerate创建Hew (Ironclad)——防御兼顾型
- Hew Ep4：400%全体AOE（每目标-10%）——多敌模式
- Counterblade Ep5：回合开始激活手中Hew 1次——被动触发Incinerate链

**设计模型**：「升级链增压」——Hew本身不是最强形态，每次使用都在为下次打出更强版本铺路，形成"当下吃亏、未来爆发"的延迟满足节奏；Fighting Spirit把激活与消耗分开，让Incinerate触发不消耗操作次数。

---

### Tiphera — 原型牌（Archetype）全能辅助流（Controller / Order）

**核心机制：Archetype 三种牌的全队辅助**
- **Quantum Seed**（0费 Skill，Unique）：治疗100% + 在抽牌堆创建2张Archetype牌 + 摸其中1张
- Archetype 三类牌（创建后从牌堆抽取）：
  - **Archetype: ○**（AP回复类）：给予队友AP
  - **Archetype: △**（伤害增幅类）：增加伤害倍率
  - **Archetype: □**（摸牌/护盾类）：给予摸牌或防御效果

**核心牌组**
- **Form Convergence**（1费 Skill，Retain）：摸2张Archetype牌，随机1张效果翻倍持续1回合
- **Creation and Destruction**（1费 Attack）：120%防御基础伤害 + 80%护盾 + 治疗80%；**Harmonization：所有效果翻倍**（当各种Archetype效果叠加后Harmonize触发）
- **Dual Creation**（1费 Upgrade，Unique）：每当2张Archetype牌被创建时，在弃牌堆再创建1张相同的
- **Event Horizon**（2费 Skill，Unique，Form Upgrade 0/3）：+1 AP + 摸1张 + 下张攻击牌首击+40%伤

**Epiphany 关键**
- Quantum Seed Ep1：创建3张Archetype牌，治疗150%
- Quantum Seed Ep5（Upgrade）：AP=0时自动创建全套3种Archetype牌（每回合1次）——被动资源生成
- Form Convergence Ep5：持续摸Archetype牌直到连续两次相同（最多6次）——极端摸牌
- Creation and Destruction Ep3（Retain）：每次Retain效果激活，所有效果+100%（最多4次）——等待4回合=效果×4

**设计模型**：「多态资源生成辅助」——三种Archetype牌代表AP/伤害/摸牌三类不同资源，Tiphera通过选择抽取哪种来动态匹配队伍当前需求；Creation and Destruction的Harmonization是"全面收益"的奖励触发，鼓励多类Archetype同时维持。

---

### Rita — 编年史（Chronicle）+时序上升（Chronal Ascension）AP自给高费爆发流（Psionic / Justice）

**核心机制：Chronicle 蓄积 → Chronal Ascension 转化 → 自给AP打高费牌**
- **Chrono Archon**（1费 Upgrade，Unique，Initiation）：每当有牌被激活时，按该牌费用获得等量Chronicle层数
  - 例：激活2费牌→+2层Chronicle；激活3费牌→+3层
- **Time Paradox**（0费 Skill，Unique）：清空所有Chronicle层，每减少1层获得1层**Chronal Ascension**
  - Chronal Ascension（时序上升）：Rita的个人资源，用于打高费牌和其他效果

**核心输出**
- **Time Axis Collapse**（2费 Attack）：300%伤害 + 手牌中所有牌**总费用×20%额外伤害**（最多+200%）——手牌越贵伤害越高
- **Future Decreed**（1费 Skill）：将手中1张2费以下的牌费用+1，激活1张随机3费牌——消耗低费换激活高费
- **Chrono Circle**（1费 Skill）：创建2张Fate Imprint

**生成牌**
- **Fate Imprint**（2费 Attack，Exhaust，Retain）：按此牌当前费用×120%伤害；使用后手中所有Fate Imprint费用+1，离开手牌时重置为2费
  - 即：第1张=2费×120%=240%；第2张变3费=3×120%=360%；以此类推——连打Fate Imprint越来越贵越来越强
- **Fate Distortion**（0费 Skill，Ephemeral，Exhaust）：下1张攻击牌+30%伤害，每费再+10%

**Ego技能**：Hour of Glory — 6层Chronal Ascension + 下1张攻击牌+10%伤害/本场累计激活牌总费用（最多+200%）（5 EP）——本场打得越多，Ego爆发越强

**Epiphany 关键**
- Time Axis Collapse Ep1：450%伤害（同样按手牌总费用×20%加成）
- Time Axis Collapse Ep4（3费，Retain）：400%伤害+2层Vulnerable；获得Chronal Ascension时若在手中自动激活——被动爆发
- Future Decreed Ep1（0费）：0费激活随机3费牌——免费触发高费牌
- Chrono Circle Ep4：消耗手中所有Exhaust牌，按总费用创建等量Fate Imprint——废牌变弹药
- Chrono Archon Ep2：获得Chronal Ascension时，按层数对随机敌人造成100%伤害——时序上升=被动攻击

**设计模型**：「费用经济飞轮」——Chrono Archon把"激活高费牌"这个行为本身变成Chronicle资源的输入；Time Paradox把Chronicle转化为可以打高费牌的Chronal Ascension；Time Axis Collapse让手牌里囤积的高费牌直接转化为伤害系数。三个环节形成闭环：激活高费牌→Chronicle→Chronal Ascension→维持高费手牌→更高伤害。

*数据来源：gamekee wiki、bwiki、哈瓦游攻略站，整理日期 2026.04.06*
