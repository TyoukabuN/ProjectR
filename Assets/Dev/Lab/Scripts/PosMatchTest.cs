using Animancer.Examples.StateMachines;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class PosMatchTest : MonoBehaviour
{
    [InfoBox("这个实验是将一把带anchor(可以理解成握把)的武器模型\n按照anchor的rotation去贴合玩家的手部武器挂点")]
    public Transform hand;

    public Transform weapon;

    private Transform _weapon;
    private Transform _handle;

    [Button("Test3_SetParent(hand, true) [LocalSpace]")]
    public void Test3()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        if (_weapon != null)
            GameObject.DestroyImmediate(_weapon.gameObject);

        _weapon = weapon.Duplicate();
        _handle = _weapon.Find("handle");
        Debug.DrawLine(hand.position, _handle.position, Color.yellow, 1f);
        Debug.DrawLine(_weapon.position, _handle.position, Color.red, 1f);

        var offset = _weapon.position - _handle.position;

        Debug.DrawLine(_weapon.position, _weapon.position + (Quaternion.Inverse(_handle.rotation) * offset), Color.green, 1f);

        var rotInv_hand = Quaternion.Inverse(hand.rotation);
        var rotInv_weapon = Quaternion.Inverse(_weapon.rotation);
        var rotInv_handle = Quaternion.Inverse(_handle.rotation);

        //worldPositionStays == true的时候
        //相当于在计算local旋转的最后乘上parent的rotation的Inverve来保持物体原来在世界空间的旋转
        //从下面的Test4中可以看出在worldPositionStays == false的时候乘上rotInv_hand就可以获得同样的结果
        _weapon.SetParent(hand, worldPositionStays:true);
        _weapon.localRotation = rotInv_handle * _weapon.rotation;
        _weapon.localPosition = rotInv_handle * offset;
    }

    [Button("Test3_SetParent(hand, false) [LocalSpace]")]
    public void Test4()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        if (_weapon != null)
            GameObject.DestroyImmediate(_weapon.gameObject);

        _weapon = weapon.Duplicate();
        _handle = _weapon.Find("handle");
        Debug.DrawLine(hand.position, _handle.position, Color.yellow, 1f);
        Debug.DrawLine(_weapon.position, _handle.position, Color.red, 1f);

        var offset = _weapon.position - _handle.position;

        Debug.DrawLine(_weapon.position, _weapon.position + (Quaternion.Inverse(_handle.rotation) * offset), Color.green, 1f);

        var rotInv_hand = Quaternion.Inverse(hand.rotation);
        var rotInv_weapon = Quaternion.Inverse(_weapon.rotation);
        var rotInv_handle = Quaternion.Inverse(_handle.rotation);

        _weapon.SetParent(hand, false);
        _weapon.localRotation = rotInv_hand * rotInv_handle * _weapon.rotation;
        _weapon.localPosition = rotInv_handle * offset;
    }

    [Button("Test5")]
    public void Test5()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        if (_weapon != null)
            GameObject.DestroyImmediate(_weapon.gameObject);

        _weapon = weapon.Duplicate();
        _handle = _weapon.Find("handle");

        var rotInv_weapon = Quaternion.Inverse(_weapon.rotation);
        var rotInv_handle = Quaternion.Inverse(_handle.rotation);

        _weapon.localRotation = hand.rotation * rotInv_handle * _weapon.rotation;
        _weapon.localPosition = Vector3.zero;
    }

    [Button("Test6_SetParent(hand, true) [WorldSpace]")]
    public void Test6()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        if (_weapon != null)
            GameObject.DestroyImmediate(_weapon.gameObject);

        _weapon = weapon.Duplicate();
        _handle = _weapon.Find("handle");
        Debug.DrawLine(hand.position, _handle.position, Color.yellow, 1f);
        Debug.DrawLine(_weapon.position, _handle.position, Color.red, 1f);

        var offset = _weapon.position - _handle.position;

        Debug.DrawLine(_weapon.position, _weapon.position + (Quaternion.Inverse(_handle.rotation) * offset), Color.green, 1f);

        var rotInv_hand = Quaternion.Inverse(hand.rotation);
        var rotInv_weapon = Quaternion.Inverse(_weapon.rotation);
        var rotInv_handle = Quaternion.Inverse(_handle.rotation);

        _weapon.SetParent(hand, worldPositionStays: true);
        //_weapon.localRotation = Quaternion.identity;
        //_weapon.localPosition = Vector3.zero;
        //_weapon.localScale = Vector3.one;

        //这次是直接修改WorldSpace的旋转和位置
        //worldPositionStays = true的时候位置计算还是不直观
        //因为SetParent的原因_weapon和_handle的transform信息已经改变了
        _weapon.rotation = hand.rotation * rotInv_handle * _weapon.rotation;
        _weapon.position = hand.position + (_weapon.position - _handle.position);
    }
    [Button("[*] Test7_SetParent(hand, false) [WorldSpace]")]
    public void Test7()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        if (_weapon != null)
            GameObject.DestroyImmediate(_weapon.gameObject);

        _weapon = weapon.Duplicate();
        _handle = _weapon.Find("handle");
        Debug.DrawLine(hand.position, _handle.position, Color.yellow, 1f);
        Debug.DrawLine(_weapon.position, _handle.position, Color.red, 1f);

        var offset = _weapon.position - _handle.position;

        Debug.DrawLine(_weapon.position, _weapon.position + (Quaternion.Inverse(_handle.rotation) * offset), Color.green, 1f);

        var rotInv_hand = Quaternion.Inverse(hand.rotation);
        var rotInv_weapon = Quaternion.Inverse(_weapon.rotation);
        var rotInv_handle = Quaternion.Inverse(_handle.rotation);

        //直接
        //_weapon.rotation =  rotInv_handle * _weapon.rotation;
        //_weapon.position = _weapon.position - _handle.position;
        //_weapon.SetParent(hand, worldPositionStays: false);//相当于乘hand.localToWorldMatrix

        //可以获得同样的结果
        //_weapon.rotation = rotInv_handle * _weapon.rotation;
        //_weapon.SetParent(hand, worldPositionStays: false);//相当于乘hand.rotation
        //_weapon.position = hand.position + (_weapon.position - _handle.position);

        //可以获得同样的结果
        //**这种方法比较直观
        //在计算玩世界坐标之后在通过 worldPositionStays = true的方式来SetParent
        //SetParent还可以设置其他offset
        _weapon.rotation = hand.rotation * rotInv_handle * _weapon.rotation;
        _weapon.position = hand.position + _weapon.position - _handle.position;
        _weapon.SetParent(hand, worldPositionStays: true);
    }
    [Button("Test8_Not SetParent[WorldSpace]")]
    public void Test8()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        if (_weapon != null)
            GameObject.DestroyImmediate(_weapon.gameObject);

        _weapon = weapon.Duplicate();
        _handle = _weapon.Find("handle");
        Debug.DrawLine(hand.position, _handle.position, Color.yellow, 1f);
        Debug.DrawLine(_weapon.position, _handle.position, Color.red, 1f);

        var offset = _weapon.position - _handle.position;

        Debug.DrawLine(_weapon.position, _weapon.position + (Quaternion.Inverse(_handle.rotation) * offset), Color.green, 1f);

        var rotInv_hand = Quaternion.Inverse(hand.rotation);
        var rotInv_weapon = Quaternion.Inverse(_weapon.rotation);
        var rotInv_handle = Quaternion.Inverse(_handle.rotation);

        //直接转换
        _weapon.rotation = hand.rotation * rotInv_handle * _weapon.rotation;
        _weapon.position += hand.position - _handle.position;
    }
}

public static class PosMatchTestHelper
{
    public static GameObject Duplicate(this GameObject gobj)
    {
        var dup = GameObject.Instantiate(gobj);
        return dup;
    }
    public static Transform Duplicate(this Transform gobj)
    {
        var dup = GameObject.Instantiate(gobj.gameObject);
        return dup.transform;
    }
}
