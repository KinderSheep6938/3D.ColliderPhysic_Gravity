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
        // �����̂̊e�ʂ�����
        if (IsLineIntersectingCube(pointA.position, pointB.position, transform.position, transform.lossyScale))
        {
            Debug.Log("�����������̖̂ʂ�ʂ�܂��B");
        }
        else
        {
            Debug.Log("�����͗����̖̂ʂ�ʂ�܂���B");
        }
    }

    bool IsLineIntersectingCube(Vector3 lineStart, Vector3 lineEnd, Vector3 cubeCenter, Vector3 cubeSize)
    {
        // �����̂̊e�ʂɑ΂��锻��
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

        // ���������ʂɕ��s�ł���Ό������Ȃ�
        if (Mathf.Approximately(dot, 0f))
            return false;

        float t = Vector3.Dot(planeNormal, (planePoint - lineStart)) / dot;

        // ������̓_�������͈͓̔��ɂ��邩�ǂ������m�F
        return t >= 0f && t <= 1f;
    }
}