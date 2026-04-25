# 00 术语表（Glossary）

> Balatro 游戏内全术语汇总，按类别分组。中英对照，附简要说明。  
> 可作为阅读其他章节时的快速参考。

**导航**：[返回索引](00_index.md) · [01 游戏概述](01_overview.md)

---

## 目录

1. [核心计分术语](#1-核心计分术语)
2. [Joker 相关术语](#2-joker-相关术语)
3. [卡牌修改器术语](#3-卡牌修改器术语)
4. [消耗品术语](#4-消耗品术语)
5. [游戏流程术语](#5-游戏流程术语)
6. [经济术语](#6-经济术语)
7. [效果触发术语](#7-效果触发术语)
8. [负面效果术语](#8-负面效果术语)

---

## 1. 核心计分术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Chips** | 筹码 | 计分乘式的被乘数。牌型基础值 + 牌点 + 修改器加成累加而成 |
| **Mult** | 倍率（乘数）| 计分乘式的乘数。+Mult 为加法，×Mult 为乘法 |
| **Score** | 分数 | `Chips × Mult` 的最终结果，每手累加 |
| **+Mult** | 加法倍率 | 直接累加到当前 Mult 值 |
| **×Mult / xMult** | 乘法倍率 | 将当前 Mult 值整体乘以该系数，在 +Mult 后计算 |
| **Base Chips** | 基础筹码 | 牌型自带的初始 Chips 值（如 Flush 基础 35） |
| **Base Mult** | 基础倍率 | 牌型自带的初始 Mult 值（如 Flush 基础 4） |
| **Hand Level** | 手牌等级 | 每种牌型的升级等级，使用星球牌提升，无上限 |
| **Poker Hand** | 扑克牌型 | 对子/两对/三条/顺子/同花等标准牌型，共 10 + 3 种秘密牌型 |
| **Scored Card** | 计分牌 | 构成牌型的那几张牌，默认仅这些牌触发 On Scored 效果 |
| **Played Hand** | 打出的手牌 | 玩家本次出牌选择的 1–5 张牌 |
| **High Card** | 高牌 | 最基础牌型，无组合，仅最高单张计分 |
| **Pair** | 对子 | 2 张相同点数 |
| **Two Pair** | 两对 | 2 组对子 |
| **Three of a Kind** | 三条 | 3 张相同点数 |
| **Straight** | 顺子 | 5 张连续点数 |
| **Flush** | 同花 | 5 张相同花色 |
| **Full House** | 葫芦（满堂红）| 三条 + 对子 |
| **Four of a Kind** | 四条 | 4 张相同点数 |
| **Straight Flush** | 同花顺 | 5 张同花且连续点数 |
| **Royal Flush** | 同花大顺 | A K Q J 10 同花 |
| **Five of a Kind** | 五条 | 5 张相同点数（秘密牌型）|
| **Flush House** | 同花葫芦 | 同花的葫芦（秘密牌型）|
| **Flush Five** | 同花五条 | 5 张同花同点（秘密牌型，最高牌型）|
| **Face Card** | 面牌 | J / Q / K，Chips 值均为 10 |
| **Odd Rank** | 奇数点数 | A / 9 / 7 / 5 / 3 |
| **Even Rank** | 偶数点数 | 10 / 8 / 6 / 4 / 2 |

---

## 2. Joker 相关术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Joker** | 小丑牌 | 提供被动效果的核心功能牌，放在专用槽位，不参与出牌 |
| **Joker Slot** | Joker 槽位 | 存放 Joker 的格子，默认 5 个 |
| **Common** | 普通 | Joker 稀有度，商店出现概率 70%，共 61 张 |
| **Uncommon** | 罕见 | Joker 稀有度，商店出现概率 25%，共 64 张 |
| **Rare** | 稀有 | Joker 稀有度，商店出现概率 5%，共 20 张 |
| **Legendary** | 传说 | Joker 稀有度，仅通过 The Soul 幻灵牌获得，共 5 张 |
| **Rarity** | 稀有度 | Joker 的等级分类，影响出现概率和价格 |
| **Edition（Joker）** | 版本 | Joker 上的附加属性：Foil / Holographic / Polychrome / Negative |
| **Foil** | 铂金箔 | +50 Chips（Joker 版本）|
| **Holographic / Holo** | 全息 | +10 Mult（Joker 版本）|
| **Polychrome / Poly** | 多彩 | ×1.5 Mult，在所有效果后触发（Joker 版本）|
| **Negative** | 负面版本 | +1 Joker 槽位（Joker 版本）；+1 消耗品槽位（消耗品版本）|
| **Sticker** | 贴纸 | 附在 Joker 上的运行内属性（Eternal / Perishable / Rental）|
| **Eternal** | 永久贴纸 | Joker 无法被卖出或摧毁 |
| **Perishable** | 易腐贴纸 | 5 轮后 Joker 被永久 Debuff（失效）|
| **Rental** | 租赁贴纸 | 每轮结束花费 $1（购买时 $1，否则每轮 $3 扣除）|
| **Stake Sticker** | 难度贴纸 | 记录持有该 Joker 时通关的最高难度颜色 |
| **Retrigger / Retriggered** | 追加触发 | 让一张已计分的牌再次触发所有 On Scored 效果一次 |
| **Sell Value** | 卖出价值 | 卖出 Joker 获得的金额，默认为买入价的一半 |
| **Blueprint** | 蓝图 | 复制右侧 Joker 效果的 Rare Joker |
| **Brainstorm** | 头脑风暴 | 复制最左侧 Joker 效果的 Rare Joker |
| **Perkeo** | 佩尔科 | 离开商店时复制随机消耗品（Negative 版本）的 Legendary Joker |

---

## 3. 卡牌修改器术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Enhancement** | 强化 | 附加在普通扑克牌上的属性，共 8 种 |
| **Bonus Card** | 加成牌 | 计分时 +30 Chips（蓝色强化）|
| **Mult Card** | 倍率牌 | 计分时 +4 Mult（红色强化）|
| **Wild Card** | 万能牌 | 可视为任意花色（多色强化）|
| **Glass Card** | 玻璃牌 | 计分时 ×2 Mult；1/4 概率自毁（透明强化）|
| **Steel Card** | 钢铁牌 | 手持时 ×1.5 Mult（不需要打出）（银色强化）|
| **Stone Card** | 石头牌 | +50 Chips，无花色/点数，始终参与计分（灰色强化）|
| **Gold Card** | 黄金牌 | 轮末若持在手中 +$3（金色强化）|
| **Lucky Card** | 幸运牌 | 1/5 概率 +20 Mult；1/15 概率 +$20（绿色强化）|
| **Seal** | 印记 | 附加在普通扑克牌上的特殊触发属性，共 4 种 |
| **Gold Seal** | 金印记 | 计分时 +$3 |
| **Red Seal** | 红印记 | 该牌所有效果追加触发 1 次 |
| **Blue Seal** | 蓝印记 | 轮末手持时生成最后打出牌型对应的星球牌 |
| **Purple Seal** | 紫印记 | 弃牌时生成塔罗牌（需有消耗品槽位）|
| **Edition（Card）** | 版本（普通牌）| 附加在普通牌上：Foil / Holographic / Polychrome |
| **Deck** | 牌组 / 牌库 | 游戏使用的一副牌，起始 52 张标准扑克，可增删 |
| **Full Deck** | 完整牌库 | 所有位置的牌（包括手牌、弃牌堆、牌库）|
| **In Deck** | 在牌库中 | 仅指当前牌库（不含手中或已弃出的牌）|

---

## 4. 消耗品术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Consumable** | 消耗品 | 使用后即消失的单次效果牌，存于消耗品槽 |
| **Consumable Slot** | 消耗品槽位 | 存放消耗品的格子，默认 2 个 |
| **Tarot Card** | 塔罗牌 | 修改扑克牌 / 给钱 / 生成其他牌，共 22 张 |
| **Planet Card** | 星球牌 | 升级对应扑克牌型等级 1 级，共 12 张（含 3 张隐藏）|
| **Spectral Card** | 幻灵牌 | 高风险高收益效果，共 18 张，仅从幻灵包获得 |
| **Booster Pack** | 增益包 | 商店购买后展示多张牌供选择的包，共 5 种 × 3 大小 |
| **Arcana Pack** | 秘术包 | 包含塔罗牌的增益包 |
| **Celestial Pack** | 天体包 | 包含星球牌的增益包 |
| **Spectral Pack** | 幻灵包 | 包含幻灵牌的增益包 |
| **Buffoon Pack** | 小丑包 | 包含 Joker 的增益包 |
| **Standard Pack** | 标准包 | 包含可加入牌库的普通扑克牌的增益包 |
| **The Soul** | 灵魂 | 生成传说 Joker 的幻灵牌（出现概率约 0.3%）|
| **Black Hole** | 黑洞 | 所有扑克牌型等级 +1（无代价）的幻灵牌 |
| **The Fool** | 愚者 | 复制本局上一张使用的塔罗/星球牌的塔罗牌 |
| **The Hanged Man** | 倒吊人 | 销毁选中最多 2 张牌（永久移除）的塔罗牌 |
| **Death** | 死神 | 将左牌复制为右牌（可拖拽调序）的塔罗牌 |
| **Judgement** | 审判 | 生成 1 张随机 Joker（需有空位）的塔罗牌 |
| **Temperance** | 节制 | 获得当前所有 Joker 卖出价值之和（上限 $50）的塔罗牌 |
| **Wraith** | 幽灵 | 生成随机 Rare Joker，金钱归零的幻灵牌 |
| **Hex** | 六芒星 | 给随机 Joker 加 Polychrome，销毁所有其他 Joker 的幻灵牌 |
| **Ankh** | 权杖十字 | 复制随机 Joker，销毁所有其他 Joker 的幻灵牌 |

---

## 5. 游戏流程术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Ante** | 关卡轮次 | 每个 Ante 包含 3 个 Blind，共 8 个 Ante（Ante 8 通关）|
| **Blind** | 盲注 | 每关需达到的分数挑战，共 3 种：Small / Big / Boss |
| **Small Blind** | 小盲注 | 无特殊效果，分数需求 1×，可跳过 |
| **Big Blind** | 大盲注 | 无特殊效果，分数需求 1.5×，可跳过 |
| **Boss Blind** | Boss 盲注 | 有特殊负面效果，分数需求 2×，不可跳过 |
| **Showdown Blind** | 决战盲注 | 仅 Ante 8 出现的特殊 Boss，5 种，分数需求 2–6× |
| **Blind Select** | 选择盲注 | 进入盲注前的选择界面，可决定跳过 Small/Big Blind |
| **Skip** | 跳过 | 跳过 Small 或 Big Blind，获得 Tag 奖励，放弃该轮收益 |
| **Tag** | 彩签/奖励标签 | 跳过盲注时获得，共 24 种，提供各类即时或延迟奖励 |
| **Cash Out** | 结算 | 打完盲注后的金钱结算阶段 |
| **The Shop** | 商店 | 每轮结算后进入，购买 Joker / 消耗品 / Voucher / 增益包 |
| **Voucher** | 凭证 | 商店中购买的永久升级，每局最多各买 1 次（部分有升级版）|
| **Round** | 回合 | 一次盲注挑战中的每次出牌/弃牌操作 |
| **Endless Mode** | 无尽模式 | 通过 Ante 8 后继续游玩，难度持续升级 |
| **Stake** | 难度档次 | 8 个难度级别（白→红→绿→黑→蓝→紫→橙→金），逐步叠加限制 |
| **Seed** | 随机种子 | 输入后可复现完整局，但禁用成就 |
| **Run** | 局 | 一次完整游戏流程（从开始到通关或失败）|
| **Collection** | 图鉴 | 记录已发现的所有 Joker / 消耗品 / 牌组的界面 |
| **Reroll** | 重滚 | 刷新商店当前展示的内容，每次花费 $1（初始），可增加 |

---

## 6. 经济术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Interest** | 利息 | 每持有 $5 在轮末获得 $1，基础上限 $5/轮 |
| **Economy** | 经济 | 泛指金钱获取与消费的整体策略 |
| **Sell** | 卖出 | 将 Joker 卖掉获得金钱，售价 = 买入价 / 2 |
| **Destroy** | 摧毁 | 永久移除一张牌或 Joker，不获得金钱 |
| **Rent** | 租赁 | Rental 贴纸效果，每轮末扣费 |
| **$** | 金钱 | 游戏内唯一货币，用于购买、重滚等 |
| **Cash Out Bonus** | 结算奖励 | 过关后获得的基础金钱（Small $3 / Big $4 / Boss $5）|
| **Remaining Hands Bonus** | 剩余手数奖励 | 每剩余 1 次出牌机会 +$1 |
| **Seed Money / Money Tree** | 存钱凭证 | 提升利息上限的 Voucher，Seed Money → $10，Money Tree → $20 |
| **To the Moon** | 奔月 | 每 $5 额外 +$1 利息的 Uncommon Joker |

---

## 7. 效果触发术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **On Played** | 出牌时 | 出牌行为发生时触发（Priority 1，最先执行）|
| **On Scored** | 计分时 | 每张参与计分的牌单独触发 |
| **On Held** | 手持时 | 持在手牌中的牌触发（不需打出）|
| **Independent** | 独立触发 | 所有牌计分完成后触发，不受单牌影响 |
| **On Other Jokers** | 针对其他 Joker | 对其他 Joker 触发效果（仅 Baseball Card）|
| **On Discard** | 弃牌时 | 弃牌行为发生时触发 |
| **On Blind Select** | 选盲注时 | 进入盲注时触发（跳过则不触发）|
| **Passive / N/A** | 被动 | 全程生效，无特定触发时机，不可被 Blueprint 复制 |
| **Mixed** | 混合触发 | 同时属于多种触发类型的 Joker |
| **When Scored** | 被计分时 | 该牌被计入牌型时触发，Debuff 状态不触发 |
| **Contains** | 包含 | 打出的手牌中包含指定牌型（如三条包含对子）|
| **Is** | 恰好是 | 整手牌恰好是指定牌型，不包含更高级别 |
| **In Full Deck** | 在完整牌库 | 含所有位置的牌（手中 + 牌库 + 弃牌堆）|
| **Currently** | 当前值 | Joker 上显示的当前累积数值 |
| **Listed Probability** | 列出概率 | 绿色标注的触发概率，可被 Oops! All 6s 翻倍 |
| **Retrigger** | 追加触发 | 让已计分的牌再次触发其所有效果一次 |
| **Activation Sequence** | 触发顺序 | Joker 效果的计算顺序，决定加法与乘法的先后 |

---

## 8. 负面效果术语

| 英文 | 中文 | 说明 |
|------|------|------|
| **Debuff** | 减益 / 失效状态 | 被标记红色×，强化/版本/印记全部失效，基础点数保留 |
| **Debuffed Joker** | Joker 失效 | Joker 版本和效果全部失效，由 Crimson Heart 或 Perishable 触发 |
| **Face-down** | 正面朝下 | 看不到牌的点数/花色，但仍可参与计分 |
| **Force-selected** | 强制选中 | 某张牌被强制始终保持选中状态（Cerulean Bell）|
| **Negative Effect** | 负面效果 | 游戏中的不利状态，多由 Boss Blind 施加 |
| **The Hook** | 钩子 | Boss Blind：每次出牌后随机丢弃手中 2 张牌 |
| **The Needle** | 针 | Boss Blind：本轮只能出 1 手牌 |
| **The Flint** | 打火石 | Boss Blind：所有牌型基础 Chips 和 Mult 减半 |
| **Verdant Leaf** | 翠叶 | Showdown Boss：所有牌被 Debuff，直到卖出 1 张 Joker |
| **Crimson Heart** | 红心 | Showdown Boss：每手牌随机 Debuff 一张 Joker |
| **Amber Acorn** | 琥珀橡果 | Showdown Boss：翻转并重洗所有 Joker 位置 |

---

## 附：缩写与符号速查

| 符号 / 缩写 | 含义 |
|------------|------|
| **C** | Chips（筹码）|
| **M** | Mult（倍率）|
| **$** | 金钱 |
| **+C / +Chips** | 添加到当前筹码值 |
| **+M / +Mult** | 添加到当前倍率值 |
| **×M / xMult / XMult** | 将当前倍率值乘以该系数 |
| **Lv.N** | 牌型等级 N |
| **Build** | 本局构建方案（Joker + 牌型 + 策略的组合）|
| **ROI** | 投资回报率（Return on Investment，用于评估购买性价比）|
| **OP** | 过于强力（Overpowered）|
| **RNG** | 随机数生成（Random Number Generation，泛指随机性）|

---

*参考来源：balatrowiki.org 各章节 | 整理日期：2026.04.26*
