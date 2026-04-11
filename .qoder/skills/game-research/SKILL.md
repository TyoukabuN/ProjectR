---
name: game-research
description: 对竞品游戏 Wiki/攻略站进行全站爬取，并将内容结构化整理为本地设计参考文档（md格式）。当用户要求调研某款游戏、爬取 Wiki、整理竞品参考资料时使用。
---

# 竞品游戏 Wiki 调研与设计文档整合

## 核心原则

- **排除新闻/公告类内容**，只爬取 Wiki 数据页和 Guides 攻略类文章
- **直接写 md 文件**，小文件（< 300行）用 create_file / search_replace，大文件用 Python 脚本
- **按模块拆分**，不要把所有内容堆进一个大文件

---

## 工作流

### 第零步：向用户确认目标

在开始任何爬取之前，**必须先询问用户**：

1. 要调研的游戏名称
2. Wiki/攻略站的 URL（如果用户没有提供）
3. 输出文件的存放目录（默认 `.document/gameDesign/<游戏缩写>/`）

```
示例提问：
- "请问要调研的游戏 Wiki 地址是什么？"
- "调研文件存放在哪个目录？"
```

如果用户在消息中已经明确提供了 URL 和存放位置，可跳过本步骤直接进入第一步。

---

### 第一步：摸清站点结构

1. 抓取首页，识别导航分类（Wiki / Guides / News 等）
2. 确认哪些 URL 对应 Wiki 数据，哪些对应攻略文章
3. 确认站点是否静态渲染（fetch_content 是否直接可用）

```
fetch_content(url="https://目标站首页")
```

### 第二步：规划文件结构

在目标目录下建 `czn/`（或以游戏缩写命名）子目录，规划文件：

| 文件 | 内容 |
|---|---|
| `00_index.md` | 索引，所有文件的入口 |
| `01_overview.md` | 游戏基本定位、核心机制 |
| `02_xxx.md` | 按主题拆分... |

**分文件原则**：每个文件 100~300 行，最大不超过 500 行。

### 第三步：并行爬取，批量写入

- 同类页面**并行** fetch（如所有角色 Build 页同时抓）
- 每次 fetch 后立即整理成 md 内容写入，不要积压

```python
# 大量角色页：并行抓取
fetch_content(url="https://czn.gg/veronica/")
fetch_content(url="https://czn.gg/cassius/")
# ...同时发出
```

### 第四步：写入文件

**小文件**（< 300行，纯英文或短中文）：直接用 search_replace 或 create_file

**大文件/长中文内容**：用 Python 脚本写入（避免 search_replace 的 40400 错误）

```python
# patch_xxx.py 模板
import pathlib
target = pathlib.Path(r"目标文件路径")
marker = "## 下一章标题"  # 在此标记前插入
new_content = """
## 新章节标题
...内容...
"""
txt = target.read_text(encoding="utf-8")
txt = txt.replace(marker, new_content + "\n" + marker, 1)
target.write_text(txt, encoding="utf-8")
print("OK")
```

**注意**：marker 要确保在文件中唯一出现，否则会重复插入。

### 第五步：更新索引

每次新增文件后，立即更新 `00_index.md` 的表格。

---

## 内容整理规范

### Wiki 数据页（角色/卡牌/装备等）

- 提取：数值、效果描述、属性/类型分类
- 提炼：设计规律和设计启示（对我方游戏的参考意义）
- 格式：表格优先，复杂机制用小节展开

### 攻略/Build 页

重点提炼：
1. **核心机制**：该角色/卡组依赖什么关键循环
2. **关键牌/技能**：核心卡牌的效果和触发条件
3. **搭配逻辑**：推荐队友/装备及原因
4. **强度来源**：为什么强（飞轮/无限叠层/资源生成等）

### 设计参考要点

每章末尾加"**设计启示**"段落，将竞品机制转化为对自己游戏设计的启发。

---

## 常见问题

| 问题 | 解决方案 |
|---|---|
| fetch_content 返回空/乱码 | 该站点可能需要 JS 渲染，尝试 search_web 找到正确 URL |
| search_replace 报 40400 错误 | 内容含长中文，改用 Python 脚本写入 |
| patch 脚本写入了两遍 | marker 在文件中出现了多次，改用更唯一的 marker |
| 找不到某个页面 URL | 用 `search_web("site:目标域名 关键词")` 定位 |

---

## 参考案例

CZN（卡厄思梦境）调研产出文件结构：

```
.document/gameDesign/czn/
├── 00_index.md           # 索引
├── 01_overview.md        # 基本定位/战斗核心机制
├── 02_cards.md           # 卡牌类型/关键字/状态词条
├── 03_characters.md      # 角色系统/全名单/Banner机制
├── 04_systems.md         # 局外系统/装备词条/进阶解析
├── 05_formula_ep.md      # 伤害公式/EP系统
├── 06_equipment_memory.md# 装备/记忆碎片系统
├── 07_char_builds_1.md   # 角色机制深析（前14角色）
├── 08_char_builds_2.md   # 角色机制深析（后15角色）
├── 09_partners.md        # 伙伴系统完整数据
├── 10_cards_reference.md # 卡牌数据库设计参考
└── 11_reroll_beginner.md # 新手引导/Reroll分析
```

*数据来源：czn.gg，整理日期 2026.04.06*
