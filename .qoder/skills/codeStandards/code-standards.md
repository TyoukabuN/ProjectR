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

---

## 3. 设计规范

### 3.1 公开方法用模板方法模式，子类只重写 On 前缀的钩子

对于有固定流程（如状态变更、日志、校验）的公开方法，基类用非虚方法封装主流程，子类通过重写 `OnXxx()` 钩子扩展行为，不允许重写公开方法本身。

**Bad:**
```csharp
// 子类直接重写公开方法，容易忘记调 base，跳过状态变更
public virtual void End()
{
    // 子类各自处理，基类流程可能被绕过
}
```

**Good:**
```csharp
// 基类：公开方法封装固定流程，调用钩子
public void End()
{
    runnerState = ERunnerState.Done;
    OnEnd();
}
// 钩子：子类重写扩展行为
protected virtual void OnEnd() { }

// 同样的模式：Play/OnPlay、Pause/OnPause、Clear/OnClear 等
public void Play()
{
    runnerState = ERunnerState.Running;
    OnPlay();
}
protected virtual void OnPlay() { }
```

**子类：**
```csharp
protected override void OnEnd()
{
    _component.Stop();  // 只写扩展逻辑，基类流程自动执行
}
```

> 适用于所有生命周期方法（`Play`/`Pause`/`End`/`Clear`/`Release` 等），保证基类状态机和公共逻辑不被子类跳过。
