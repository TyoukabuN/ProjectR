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
