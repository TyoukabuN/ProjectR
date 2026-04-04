# 代码写作规范

## 1. 控制流规范

### 1.1 避免 if 多层嵌套，优先 early return

if 作用域嵌套层数尽量控制在最少，遇到前置条件检查时，优先提前 return 退出，而不是将主逻辑包裹在 if 块内。

**Bad:**
```csharp
void DoSomething(object obj)
{
    if (obj != null)
    {
        if (obj.IsValid)
        {
            // 主逻辑...
        }
    }
}
```

**Good:**
```csharp
void DoSomething(object obj)
{
    if (obj == null)
        return;
    if (!obj.IsValid)
        return;

    // 主逻辑...
}
```

## 2. 命名规范

### 2.1 变量名尽可能不使用缩写

变量、字段、参数的命名应使用完整单词，避免使用缩写，保持可读性。

**Bad:**
```csharp
var initCtx = new UpdateContext { ... };
var spf = GetSecondPerFrame();
var obj = GetComponent<Rigidbody>();
```

**Good:**
```csharp
var initContext = new UpdateContext { ... };
var secondPerFrame = GetSecondPerFrame();
var rigidbody = GetComponent<Rigidbody>();
```

> 常见例外：循环计数 `i`/`j`/`k`，数学约定缩写（`dt` 在物理计算中可接受），以及行业内极其通用的缩写（如 `id`、`ui`）。

---

### 1.2 early return 中 return 语句必须换行

early return 的 `return` 不写在 if 同一行，必须另起一行。

**Bad:**
```csharp
if (obj == null) return;
```

**Good:**
```csharp
if (obj == null)
    return;
```
