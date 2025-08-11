using Sirenix.OdinInspector;
using UnityEngine;

public class SceneEnvironmentSetting : MonoBehaviour
{
    [LabelText("生成怪物")]
    public bool GenSceneMonster = true;
    [LabelText("生成陷阱")]
    public bool GenSceneTrap = true;
    [LabelText("生成物品")]
    public bool GenSceneItem = true;
}
