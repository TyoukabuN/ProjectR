using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrap
{
    void OnStart();
    virtual void OnDestory() { }
}
