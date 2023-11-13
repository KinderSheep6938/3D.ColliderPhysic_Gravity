using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCubeIntersection : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private void Update()
    {
        Debug.DrawLine(pointA.position, pointB.position, Color.red);
        // 立方体の各面を検査
        if (IsLineIntersectingCube(pointA.position, pointB.position, transform.position, transform.lossyScale))
        {
            Debug.Log("直線が立方体の面を通ります。");
        }
        else
        {
            Debug.Log("直線は立方体の面を通りません。");
        }
    }

    bool IsLineIntersectingCube(Vector3 lineStart, Vector3 lineEnd, Vector3 cubeCenter, Vector3 cubeSize)
    {
        // 立方体の各面に対する判定
        return IsLineIntersectingPlane(lineStart, lineEnd, cubeCenter + new Vector3(0, 0, cubeSize.z * 0.5f), Vector3.forward) ||
               IsLineIntersectingPlane(lineStart, lineEnd, cubeCenter - new Vector3(0, 0, cubeSize.z * 0.5f), -Vector3.forward) ||
               IsLineIntersectingPlane(lineStart, lineEnd, cubeCenter + new Vector3(0, cubeSize.y * 0.5f, 0), Vector3.up) ||
               IsLineIntersectingPlane(lineStart, lineEnd, cubeCenter - new Vector3(0, cubeSize.y * 0.5f, 0), -Vector3.up) ||
               IsLineIntersectingPlane(lineStart, lineEnd, cubeCenter + new Vector3(cubeSize.x * 0.5f, 0, 0), Vector3.right) ||
               IsLineIntersectingPlane(lineStart, lineEnd, cubeCenter - new Vector3(cubeSize.x * 0.5f, 0, 0), -Vector3.right);
    }

    bool IsLineIntersectingPlane(Vector3 lineStart, Vector3 lineEnd, Vector3 planePoint, Vector3 planeNormal)
    {
        Vector3 lineDir = (lineEnd - lineStart).normalized;
        float dot = Vector3.Dot(planeNormal, lineDir);

        // 直線が平面に平行であれば交差しない
        if (Mathf.Approximately(dot, 0f))
            return false;

        float t = Vector3.Dot(planeNormal, (planePoint - lineStart)) / dot;

        // 直線上の点が線分の範囲内にあるかどうかを確認
        return t >= 0f && t <= 1f;
    }
}