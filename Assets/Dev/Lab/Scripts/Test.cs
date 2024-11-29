using Sirenix.OdinInspector;
using UnityEngine;
using PJR;

public class Test : MonoBehaviour
{
    //[Button("LogProjPaths")]
    //public void LogProjPaths()
    //{
    //    LogSystem.Log(Application.dataPath);
    //    LogSystem.Log(Application.persistentDataPath);
    //    LogSystem.Log(Application.consoleLogPath);
    //    LogSystem.Log(Application.streamingAssetsPath);
    //}
    //[Button("GetComp")]
    //public void Test321()
    //{
    //    Debug.Log(gameObject.GetComponentInChildren<Collider>());
    //}

    //private LogicEntity player = null;
    //[Button("GenPlayer")]
    //public void Test1()
    //{
    //    //Process.Start(Application.persistentDataPath);
    //    //Debug.Log(GUI.skin.FindStyle("ToolbarSeachTextField"));
    //    if (player != null)
    //    {
    //        EntitySystem.DestroyEntity(player);
    //        player = null;
    //    }
    //    player = EntitySystem.CreatePlayer();
    //}

    //public string key = string.Empty;
    //public Vector3 Force = Vector3.zero;
    //public float Duration = 0.333f;
    //public float Damp = -1f;
    //[ShowInInspector]
    //public Easing.Ease easing = Easing.Ease.Linear;

    //public GameObject gobjTest;

    //[Button("AddForce")]
    //public void AddForce()
    //{
    //    if (player != null)
    //    {
    //        var controller = player.AddExtraVelocity(key,Force, Duration, Damp);
    //        controller.easeType = easing;
    //    }
    //}

    [FoldoutGroup("CrossTest")]
    public Vector3 Vec1;
    [FoldoutGroup("CrossTest")]
    public Vector3 Vec2;
    [Button("CrossTest"), FoldoutGroup("CrossTest")]
    public void CrossTest()
    {
        Debug.Log(Vector3.Cross(Vec1, Vec2));
    }

    [FoldoutGroup("GetObstructionNormalTest")]
    public Vector3 HitNormal;
    [FoldoutGroup("GetObstructionNormalTest")]
    public Vector3 GroundNormal;
    [FoldoutGroup("GetObstructionNormalTest")]
    public Vector3 _characterUp;

    [Button("Test"), FoldoutGroup("GetObstructionNormalTest")]
    public void GetObstructionNormalTest()
    {
        Debug.Log(GetObstructionNormal(HitNormal, true));
    }
    private Vector3 GetObstructionNormal(Vector3 hitNormal, bool stableOnHit)
    {
        // Find hit/obstruction/offset normal
        Vector3 obstructionNormal = hitNormal;
        if (true)
        {
            Vector3 obstructionLeftAlongGround = Vector3.Cross(GroundNormal, obstructionNormal).normalized;
            obstructionNormal = Vector3.Cross(obstructionLeftAlongGround, _characterUp).normalized;
        }

        // Catch cases where cross product between parallel normals returned 0
        if (obstructionNormal.sqrMagnitude == 0f)
        {
            obstructionNormal = hitNormal;
        }

        return obstructionNormal;
    }
}
