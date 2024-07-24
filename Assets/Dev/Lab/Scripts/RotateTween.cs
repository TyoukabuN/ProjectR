using Sirenix.OdinInspector;
using System;
using UnityEngine;
public class RotateTween : MonoBehaviour
{
    public Transform target;

    [LabelText("最大绝对倾斜角")]
    public float maxAbsInclineAngle = 30f;
    [LabelText("倾斜速度(角/帧)")]
    public float inclinePerFrame = 1f;

    public float average = 30f;

    public Vector4 testVec4 = Vector4.zero;

    public bool playing = false;

    [Button()]
    public void Play()
    {
        playing = !playing;
        if(playing)
            Revert();
    }
    [Button()]
    public void Test()
    {
        Debug.Log(Mathf.Repeat(testVec4.x, 360f));
        Debug.Log(Mathf.Repeat(testVec4.y, 180f));
    }
    private void Update()
    {
        if (!playing)
            return;
        //float sign = Mathf.Sign(average) * -1f;
        //float targetRotateZ = Mathf.Repeat(sign * maxAbsInclineAngle, 360f);
        //float inclineSpeed = sign * inclinePerFrame;

        //float eularZ = target.transform.localEulerAngles.z + inclineSpeed;
        //eularZ = Mathf.Repeat(eularZ, 360f);
        //if (sign < 0) { 
        //    if(Mathf.Abs(eularZ) < Mathf.Abs(targetRotateZ))
        //        eularZ = targetRotateZ;
        //}if (sign > 0) {
        //    if (Mathf.Abs(eularZ) > Mathf.Abs(targetRotateZ))
        //        eularZ = targetRotateZ;
        //}
        //target.transform.localEulerAngles = new Vector3(target.transform.localEulerAngles.x, target.transform.localEulerAngles.y, eularZ);

        //
        //float sign = Mathf.Sign(average) * -1f;
        //float targetRotateZ = sign * maxAbsInclineAngle;
        //float inclineSpeed = sign * inclinePerFrame;

        //var eular = target.transform.localRotation.eulerAngles;
        //float eularZ = target.transform.localRotation.eulerAngles.z + inclineSpeed;
        //if (Mathf.Abs(eularZ) > Mathf.Abs(targetRotateZ))
        //    eularZ = targetRotateZ;
        //target.transform.localRotation = Quaternion.Euler(eular.x, eular.y, eularZ);

        float sign = Mathf.Sign(average) * -1f;
        float targetRotateZ = sign * maxAbsInclineAngle;
        float inclineSpeed = sign * inclinePerFrame;
        var currentEuler = target.transform.localRotation.eulerAngles;

        var rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, targetRotateZ);

        target.transform.localRotation = Quaternion.Slerp(target.transform.localRotation, rotation, inclinePerFrame);
    }
    public void Revert()
    { 
        target.transform.localEulerAngles = Vector3.zero;
    }
}
