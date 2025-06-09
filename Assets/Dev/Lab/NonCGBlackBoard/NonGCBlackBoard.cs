using System;
using PJR.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector;

public class NonGCBlackBoard : SerializedMonoBehaviour
{
    public CachedValueBoard CachedValueBoard;

    [Button]
    public void Test()
    {
        string key = "测试Key";
        if (CachedValueBoard.Key2Value.ContainsKey(key))
            return;
        Type genericType = typeof(CachedValue<>).MakeGenericType(typeof(float));
        ICachedValue obj = Activator.CreateInstance(genericType) as ICachedValue;
        CachedValueBoard.Key2Value.Add(key,obj);
    }
}
