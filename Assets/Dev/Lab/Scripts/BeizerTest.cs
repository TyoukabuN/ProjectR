using PJR;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeizerTest : MonoBehaviour
{
    public List<Transform> transforms = new List<Transform>();

    private List<Vector3> wpoints
    {
        get
        {
            var temp = new List<Vector3>();
            foreach (var trans in transforms)
            {
                temp.Add(trans.position);
            }
            return temp;
        }
    }

    private void Update()
    {
        PrintCurve();
    }

    [Button("PrintCurve")]
    public void PrintCurve()
    {
        var points = BeizerUtil.SampleBeizerCurve(wpoints, 20);
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = points[i];
            Vector3 p1 = points[i + 1];
            Debug.DrawLine(p0, p1, Color.green, Time.deltaTime);
        }
    }
}
