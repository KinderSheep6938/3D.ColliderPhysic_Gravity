/// -----------------------------------------------------------------
/// ColliderEngine.cs�@Collider���萧��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;

public static class ColliderManager
{
    #region �ϐ�
    //�񕪊��p�萔
    private const int HALF = 2;

    //��bVector���ۑ��p
    private static readonly Vector3 _vectorZero = Vector3.zero;
    private static readonly Vector3 _vector3Up = Vector3.up;
    private static readonly Vector3 _vector3Right = Vector3.right;
    private static readonly Vector3 _vector3Flont = Vector3.forward;
    private static readonly Vector2 _vector2Up = Vector2.up;
    private static readonly Vector2 _vector2Right = Vector2.right;

    //�Փ˔���͈͂̍ő�l
    private static readonly Vector3 _collisionRange = Vector3.one / HALF;

    //���X�g�������p
    private static readonly Vector2[] _resetReturnList = new Vector2[2];

    //�ʂɂ����钸�_���W
    private static readonly Vector2[] _planeEdge =
    {
        new Vector2(0.5f,0.5f),         //�E��
        new Vector2(0.5f,-0.5f),        //�E��
        new Vector2(-0.5f,0.5f),        //����
        new Vector2(-0.5f,-0.5f)        //����
    };
    //�ʂɂ����钸�_���W��ID�i��L�̒��_���W�ɑΉ��j
    private struct EdgeByPlane
    {
        public const int rU = 0;        //�E��
        public const int rD = 1;        //�E��
        public const int lU = 2;        //����
        public const int lD = 3;        //����
    }

    //Collider��񋤗L�p
    private static List<OriginalCollider> _worldInColliders = new();

    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    /// <summary>
    /// <para>SetColliderToWorld</para>
    /// <para>�Ώۂ�Collider�������L���X�g�ɐݒ肵�܂�</para>
    /// </summary>
    /// <param name="target">Collider���</param>
    public static void SetColliderToWorld(OriginalCollider target)
    {
        //���Ɋi�[����Ă��邩
        if (_worldInColliders.Contains(target))
        {
            //�i�[�����I��
            return;
        }

        //�i�[
        _worldInColliders.Add(target);
    }

    /// <summary>
    /// <para>RemoveColliderToWorld</para>
    /// <para>�Ώۂ�Collider�������L���X�g����폜���܂�</para>
    /// </summary>
    /// <param name="target">Collider���</param>
    public static void RemoveColliderToWorld(OriginalCollider target)
    {
        //���L���X�g����폜
        _worldInColliders.Remove(target);
    }

    /// <summary>
    /// <para>CheckCollision</para>
    /// <para>Collider��񂩂�Փ˂��Ă��邩�������܂�</para>
    /// </summary>
    /// <param name="collider">Collider���</param>
    /// <returns>�Փ˔���</returns>
    public static bool CheckCollision(OriginalCollider collider)
    {
        //�����ΏۂɈ�ԋ߂����_���W
        Vector3 nearEdge;
        //���̒��_���W�̃C���f�b�N�X�ۑ��p
        int nearEdgeIndex;

        //���L���X�g����SCollider�����擾���A�Փˌ������s���܂�
        foreach(OriginalCollider target in _worldInColliders)
        {
            //�����Ώۂ� ���g �ł���
            if(target == collider)
            {
                //�������Ȃ�
                continue;
            }

            //���g�̒��_���W ���� �ł������Ώۂɋ߂����_���W ���i�[
            nearEdge = GetNearEdgeByCollider(target, collider.Data.edgePos);
            //���̒��_���W�̃C���f�b�N�X�擾
            nearEdgeIndex = Array.IndexOf(collider.Data.edgePos, nearEdge);
            Debug.Log(nearEdgeIndex + "myNearIndex");
            //���g�̍ł��߂����_����ʏ�Ɍ��Ԃ��Ƃ̂ł���� �� �����Ώۂ�Collider �ɏd�Ȃ�
            if (CheckPlaneLineOverlap(nearEdgeIndex, collider.Data.edgePos, target))
            {
                Debug.Log("LineCollision ; " + target.name);
                //�Փ˔��肪����
                return true;
            }

            //�����Ώۂ̒��_���W ���� �ł����g�ɋ߂����_���W ���i�[
            nearEdge = GetNearEdgeByCollider(collider, target.Data.edgePos);
            //���̒��_���W�̃C���f�b�N�X�擾
            nearEdgeIndex = Array.IndexOf(target.Data.edgePos, nearEdge);
            Debug.Log(nearEdgeIndex + "NearIndex");
            //���g�̍ł��߂����_����ʏ�Ɍ��Ԃ��Ƃ̂ł���� �� �����Ώۂ�Collider �ɏd�Ȃ�
            if (CheckPlaneLineOverlap(nearEdgeIndex, target.Data.edgePos, collider))
            {
                Debug.Log("ColliderLineCollision : " + target.name);
                //�Փ˔��肪����
                return true;
            }


            ////�ł��߂����_���W �� �����Ώۂ�Collider �̓����ɂ���
            //if (CheckPointInCollider(nearEdge, target))
            //{
            //    Debug.Log("MyCollision");
            //    //�Փ˔��肪����
            //    return true;
            //}

            ////�����Ώۂ̒��_���W ���� �ł��߂����_���W ���i�[
            //nearEdge = GetNearEdgeByCollider(collider, target.Data.edgePos);
            ////�ł��߂����_���W �� ���g��Collider �̓����ɂ���
            //if(CheckPointInCollider(nearEdge, collider))
            //{
            //    Debug.Log("Collision");
            //    return true;
            //}

            //���g�̒��S���W �� �����Ώۂ�Collider �̓����ɂ���
            if (CheckPointInCollider(collider.Data.position, target))
            {
                Debug.Log("MyCenterCollision");
                //�Փ˔��肪����
                return true;
            }

            //�����Ώ��̒��S���W �� ���g��Collider �̓����ɂ���
            if (CheckPointInCollider(target.Data.position, collider))
            {
                Debug.Log("CenterCollision");
                //�Փ˔��肪����
                return true;
            }



        }

        Debug.Log("NoCollision");
        //�Փ˔��肪�Ȃ�
        return false;
    }

    /// <summary>
    /// <para>GetNearEdgeByCollider</para>
    /// <para>�Ώۂ�Collider���瑊�ΓI�ɍł��߂����_���W���擾���܂�</para>
    /// </summary>
    /// <param name="target">�I�u�W�F�N�g���</param>
    /// <param name="edges">���_���W���X�g</param>
    /// <returns>�ł��߂����_���W</returns>
    private static Vector3 GetNearEdgeByCollider(OriginalCollider target, Vector3[] edges)
    {
        //�Z�o���ʕۑ��p
        float distance;
        //�ł��߂������ۑ��p�@�����l�Ƃ���-1���i�[����
        float minDistance = float.MaxValue;

        //�I�u�W�F�N�g�ۑ��p
        Transform localObj = target.MyTransform;
        //���[�J���ϊ��p
        Vector3 localEdge;

        //�ԋp�p
        Vector3 returnPos = default;

        //���_����ł��߂����_���W������
        foreach(Vector3 edge in edges)
        {
            //���[�J���ϊ�
            localEdge = localObj.InverseTransformPoint(edge);
            //�����Z�o
            distance = Vector3.Distance(_vectorZero, localEdge);
            //Debug.Log(edge + ":" + localEdge + ":" + distance);
            //Debug.Log(distance + "dis " + localEdge + "local " + edge + "edge");
            //�Z�o���ʂ��ۑ�����Ă��鋗�����傫�� �܂��� ����łȂ� �ꍇ�͉������Ȃ�
            if(minDistance < distance)
            {
                continue;
            }

            //�����X�V
            minDistance = distance;
            //���W�ݒ�
            returnPos = edge;
        }
        //Debug.Log(returnPos + "edge ");
        //�����I��
        return returnPos;
    }

    /// <summary>
    /// <para>CheckPointInCollider</para>
    /// <para>�����Ώۍ��W��Collider�����ɂ��邩�������܂�</para>
    /// </summary>
    /// <param name="check">�����Ώۍ��W</param>
    /// <param name="collider">�����Ώ�Collider</param>
    /// <returns>��������</returns>
    private static bool CheckPointInCollider(Vector3 check, OriginalCollider collider)
    {
        //�����Ώۖڐ��̃��[�J�����W
        Vector3 localPos = collider.MyTransform.InverseTransformPoint(check);
        //Debug.Log(localPos + "local");

        //Debug.Log(localPos + "localedge");

        //Collider�̊e�������ɊO���ɂ��邩�𔻒肷��
        //Collider�� X�� �ɂ����ĊO���ł���
        if (_collisionRange.x < localPos.x || localPos.x < -_collisionRange.x)
        {
            //�����ɂ��Ȃ�
            return false;
        }
        //Collider�� Y�� �ɂ����ĊO���ł���
        if (_collisionRange.y < localPos.y || localPos.y < -_collisionRange.y)
        {
            //�����ɂ��Ȃ�
            return false;
        }
        //Collider�� Z�� �ɂ����ĊO���ł���
        if (_collisionRange.z < localPos.z || localPos.z < -_collisionRange.z)
        {
            //�����ɂ��Ȃ�
            return false;
        }

        //�����ł���
        return true;
    }

    /// <summary>
    /// <para>CheckPlaneLineOverlap</para>
    /// <para>���_�ɑ΂��Ėʏ�Ō��Ԃ��Ƃ̂ł������Collider�ɏd�Ȃ��Ă��邩�������܂�</para>
    /// </summary>
    /// <param name="edge">�w�肷�钸�_</param>
    /// <param name="edgePos">���_���W</param>
    /// <param name="collider">�Ώ�Collider</param>
    /// <returns></returns>
    private static bool CheckPlaneLineOverlap(int edge, Vector3[] edgePos, OriginalCollider collider)
    {
        //���_�ɑ΂��āA�ʏ�Ɍ��Ԃ��Ƃ̂ł��钸�_���������܂�
        foreach(int lineEdge in EdgeLineManager.GetEdgeFromPlaneLine(edge))
        {
            Debug.Log(lineEdge + "lineEdge");
            //���_�Ɗe�Ώۂ̒��_�����Ԑ� �� Collider �ɏd�Ȃ邩����
            if(CheckLineOverlapByCollider(edgePos[edge], edgePos[lineEdge], collider))
            {
                //�d�Ȃ��Ă���Ɣ���
                return true;
            }
            //Debug.Log("--------------------------");
        }

        //�d�Ȃ��Ă��Ȃ��Ɣ���
        return false;
    }

    /// <summary>
    /// <para>CheckLineOverlapByCollider</para>
    /// <para>����Collider�ɏd�Ȃ邩�������܂�</para>
    /// </summary>
    /// <param name="startPoint">���̎n�_</param>
    /// <param name="endPoint">���̏I�_</param>
    /// <param name="collider">�Ώ�Collider</param>
    /// <returns>�d�Ȃ蔻��</returns>
    private static bool CheckLineOverlapByCollider(Vector3 startPoint, Vector3 endPoint, OriginalCollider collider)
    {
        //Debug.Log(startPoint + " SP" + endPoint + " EP");
        //���̎n�_�ƏI�_�����[�J���ϊ�
        Vector3 localStart = collider.MyTransform.InverseTransformPoint(startPoint);
        Vector3 localEnd = collider.MyTransform.InverseTransformPoint(endPoint);

        //�e�������Ƃ̖ʂŔ�����s��
        //Z�ʁiX�� �� Y���j
        Vector2 vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.y;
        Vector2 vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.y;
        Debug.Log("Z:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
        //�ʂɏd�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
        if (!CheckLineOverlapByPlane(vec2Start, vec2End))
        {
            return false;
        }
        //X�ʁiY�� �� Z���j
        vec2Start = _vector2Right * localStart.z + _vector2Up * localStart.y;
        vec2End = _vector2Right * localEnd.z + _vector2Up * localEnd.y;
        Debug.Log("X:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
        //�ʂ��d�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
        if (!CheckLineOverlapByPlane(vec2Start, vec2End))
        {
            return false;
        }
        //Y�ʁiX�� �� Z���j
        vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.z;
        vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.z;
        Debug.Log("Y:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
        //�ʂ��d�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
        if (!CheckLineOverlapByPlane(vec2Start, vec2End))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// <para>CheckLineOverlapByPlane</para>
    /// <para>�����ʂɏd�Ȃ邩�������܂�</para>
    /// </summary>
    /// <param name="startPoint">���̎n�_</param>
    /// <param name="endPoint">���̏I�_</param>
    /// <returns>�d�Ȃ蔻��</returns>
    private static bool CheckLineOverlapByPlane(Vector2 startPoint, Vector2 endPoint)
    {
        //���̎n�_�ƏI�_�ŁA�Е��ł��ʏ�ɂ���ꍇ�́A�ʂɏd�Ȃ�Ɣ��肷��
        //�n�_���ʏ�ɂ���
        if (CheckPointInPlane(startPoint))
        {
            return true;
        }
        //�I�_���ʏ�ɂ���
        if (CheckPointInPlane(endPoint))
        {
            return true;
        }

        //�����ʂɏd�Ȃ邩�������n�߂�
        //���߂Ɍ����ɕK�v�Ȓ��_���W���擾����
        Vector2[] edges = GetPlaneEdgeByPoint(startPoint);
        //�����̌X�����n�_����e���_���W�����Ԑ��̌X���ȓ��ł���ꍇ�́A�d�Ȃ�Ɣ��肷��
        if (CheckLineSlopeByPlane(startPoint, endPoint, edges))
        {
            return true;
        }

        //�d�Ȃ�Ȃ�
        return false;

    }

    /// <summary>
    /// <para>CheckPointInPlane</para>
    /// <para>�������W���ʏ�ɂ��邩�������܂�</para>
    /// </summary>
    /// <param name="point">�������W</param>
    /// <returns>�ʏ㔻��</returns>
    private static bool CheckPointInPlane(Vector2 point)
    {
        //���W���ʂ̉����O�ł���
        if(point.x < -_collisionRange.x || _collisionRange.x < point.x)
        {
            return false;
        }

        //���W���ʂ̏c���O�ł���
        if (point.y < -_collisionRange.y || _collisionRange.y < point.y)
        {
            return false;
        }

        //�͈͓��ł���
        return true;
    }

    /// <summary>
    /// <para>GetPlaneEdgeByPoint</para>
    /// <para>�ʂ̒��_���W���擾���܂�</para>
    /// </summary>
    /// <param name="point">�������W</param>
    /// <returns>�������W�ɑ΂���K�v�Ȓ��_���W���X�g</returns>
    private static Vector2[] GetPlaneEdgeByPoint(Vector2 point)
    {
        //�ԋp�p
        Vector2[] returnEdges = _resetReturnList;

        //�ʂ��猩���������W���ǂ̈ʒu�ɑ��݂��邩�ɂ���Ē��_���W��ύX����
        
        //�������W�� �E�� �܂��� ���� �ɂ���
        if((_collisionRange.x < point.x && _collisionRange.y < point.y)
            || (point.x < -_collisionRange.x && point.y < -_collisionRange.y ))
        {
            //����ƉE����ԋp
            returnEdges[0] = _planeEdge[EdgeByPlane.lU];
            returnEdges[1] = _planeEdge[EdgeByPlane.rD];
        }
        //�������W�� �E�� �܂��� ���� �ɂ���
        else if((_collisionRange.x < point.x && point.y < -_collisionRange.y)
            || (point.x < -_collisionRange.x && _collisionRange.y < point.y))
        {
            //�E��ƍ�����ԋp
            returnEdges[0] = _planeEdge[EdgeByPlane.rU];
            returnEdges[1] = _planeEdge[EdgeByPlane.lD];
        }
        //�����Ώۂ� �����͈͓̔� �ł���
        else if(-_collisionRange.x <= point.x && point.x <= _collisionRange.x)
        {
            //�����Ώۂ� �㑤 �ɂ���
            if(_collisionRange.y < point.y)
            {
                //�E��ƍ����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                returnEdges[1] = _planeEdge[EdgeByPlane.lU];
            }
            //���� �ɂ���
            else
            {
                //�E���ƍ�����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rD];
                returnEdges[1] = _planeEdge[EdgeByPlane.lD];
            }
        }
        //�����Ώۂ� �c���͈͓̔� �ł���
        else
        {
            //�����Ώۂ� �E�� �ɂ���
            if (_collisionRange.x < point.x)
            {
                //�E��ƉE����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                returnEdges[1] = _planeEdge[EdgeByPlane.rD];
            }
            //���� �ɂ���
            else
            {
                //����ƍ�����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.lU];
                returnEdges[1] = _planeEdge[EdgeByPlane.lD];
            }
        }

        //�ԋp
        return returnEdges;
    }

    /// <summary>
    /// <para>CheckLineSlopeByEdge</para>
    /// <para>�n�_�ƏI�_�����Ԑ��̌X�� �� �ʂɓ�����X�� �ł��邩�������܂�</para>
    /// </summary>
    /// <param name="start">���̎n�_</param>
    /// <param name="end">���̏I�_</param>
    /// <param name="edges">���_���W���X�g</param>
    /// <returns>�͈͓�����</returns>
    private static bool CheckLineSlopeByPlane(Vector2 start, Vector2 end, Vector2[] edges)
    {
        //�X��������ʂ� ���� ���̑傫�����ʂɏd�Ȃ邩
        if (CheckSlopeByEdgeSlope(start,end,edges) && CheckSlopeOverlapPlane(start,end))
        {
            //�͈͓��ł���
            return true;
        }

        //�͈͊O�ł���
        return false;
    }

    /// <summary>
    /// <para>CheckSlopeByEdgeSlope</para>
    /// <para>�Ώۃx�N�g���̌X�������_�x�N�g���̌X���͈͓̔��E�Ԃɑ��݂��邩�������܂�</para>
    /// </summary>
    /// <param name="lineSlope">�Ώۃx�N�g��</param>
    /// <param name="edges">���_���W</param>
    /// <param name="edgeMaxSlope">���_�x�N�g���̍ő�l�̌X��</param>
    /// <param name="edgeMinSlope">���_�x�N�g���̍ŏ��l�̌X��</param>
    /// <returns>�͈͓�����</returns>
    private static bool CheckSlopeByEdgeSlope(Vector2 start, Vector2 end, Vector2[] edges)
    {
        //�X�����Z�o
        Vector2 lineSlope = end - start;
        Vector2 edgeSlope1 = edges[0] - start;
        Vector2 edgeSlope2 = edges[1] - start;
        Debug.Log(lineSlope + " lS" + edgeSlope1 + " eS1" + edgeSlope2 + " eS2");

        //�e�x�N�g�������𐳋K��
        lineSlope = lineSlope.normalized;
        edgeSlope1 = edgeSlope1.normalized;
        edgeSlope2 = edgeSlope2.normalized;

        //�n�_�ƒ��_���W�����Ԑ��̌X���ɂ����� �e�����̍ő�l�E�ŏ��l �����o��
        Vector2 edgeMaxSlope = _vectorZero;
        Vector2 edgeMinSlope = _vectorZero;
        //���_���W�x�N�g���̌X����X������
        if (edgeSlope1.x < edgeSlope2.x)
        {
            edgeMaxSlope += _vector2Right * edgeSlope2.x;
            edgeMinSlope += _vector2Right * edgeSlope1.x;
        }
        else
        {
            edgeMaxSlope += _vector2Right * edgeSlope1.x;
            edgeMinSlope += _vector2Right * edgeSlope2.x;
        }
        //���_���W�x�N�g���̌X����Y������
        if (edgeSlope1.y < edgeSlope2.y)
        {
            edgeMaxSlope += _vector2Up * edgeSlope2.y;
            edgeMinSlope += _vector2Up * edgeSlope1.y;
        }
        else
        {
            edgeMaxSlope += _vector2Up * edgeSlope1.y;
            edgeMinSlope += _vector2Up * edgeSlope2.y;
        }

        Debug.Log("Nomal: " + lineSlope + " lS" + edgeMaxSlope + " eMaS" + edgeMinSlope + " eMiS");

        //�͈͌��� ---------------------------------------------------------------------------------
        ////�����̐�������p�itrue:�� false:���j
        //bool direSign;
        Debug.Log("edge:" + edges[0] + "|" + edges[1]);
        //���_���W�����݂��ɓ���������ɑ��݂��Ȃ���
        if (edges[0].x != edges[1].x && edges[0].y != edges[1].y)
        {
            Debug.Log("NoXY");
            //�Ώۃx�N�g���̌X�����ő�l�E�ŏ��l�͈͓̔��ł���
            if((edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x)
                && (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y))
            {
                //�͈͓��ł���
                return true;
            }
            //�͈͊O�ł���
            return false;
        }
        //���_���W������X����ɑ��݂���
        else if(edges[0].x == edges[1].x)
        {
            Debug.Log("X");
            //�Ώۃx�N�g���̌X����Y�����ő�l�E�ŏ��l�͈͓̔��ł��邩
            if(edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y)
            {
                //�͈͓��ł���
                return true;
            }

            ////�n�_���猩�����S���W�ւ̕����̐�������itrue:�� false:���j
            //direSign = (0 < -edges[0].x);
            //Debug.Log("dire:" + direSign);
            ////�Ώۃx�N�g���̌X����X�����ł��߂��ӂ𒴂��Ă��邩
            //if ((direSign && edgeMinSlope.x <= lineSlope.x)
            //    || (!direSign && lineSlope.x <= edgeMinSlope.x))
            //{
            //    //�͈͓��ł���
            //    return true;
            //}
            //�͈͊O�ł���
            return false;
        }
        //���_���W������Y����ɑ��݂���
        else
        {
            Debug.Log("Y");
            //�Ώۃx�N�g���̌X����Y�����ő�l�E�ŏ��l�͈̔͊O�ł��邩
            if (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x)
            {
                //�͈͓��ł���
                return true;
            }

            ////�n�_���猩�����S���W�ւ̕����̐�������itrue:�� false:���j
            //direSign = (0 < -edges[0].y);
            //Debug.Log("dire:" + direSign);
            ////�Ώۃx�N�g���̌X����X�����ŏ��l�𒴂��Ă��邩
            //if ((direSign && edgeMinSlope.y <= lineSlope.y)
            //    || (!direSign && lineSlope.y <= edgeMinSlope.y))
            //{
            //    //�͈͓��ł���
            //    return true;
            //}
            //�͈͊O�ł���
            return false;
        }
    }

    /// <summary>
    /// <para>CheckSlopeOverlapPlane</para>
    /// <para>�n�_�ƏI�_�����Ԑ��̌X�����ʂɏd�Ȃ邩�������܂�</para>
    /// </summary>
    /// <param name="start">���̎n�_</param>
    /// <param name="end">���̏I�_</param>
    /// <returns>�d�Ȃ蔻��</returns>
    private static bool CheckSlopeOverlapPlane(Vector2 start,Vector2 end)
    {
        Debug.Log(start + "|" + end);
        //�n�_���猩���ʂւ̊�b�x�N�g�����擾
        Vector2 slopeDire = GetSlopeByStartToOrigin(start);
        //�n�_����ʂɓ�����ŏ��X�������Z�o
        Debug.Log(start + " - " + GetProjection(slopeDire, start));
        Vector2 centorMinSlope = start - GetProjection(slopeDire, start);
        //�n�_����I�_�ւ̌X���𒆐S�����֎ˉe
        Vector2 centorSlope = end - start;

        //�n�_���猩�����S�ւ̕�����������
        bool xDire = 0 < -start.x;
        bool yDire = 0 < -start.y;
        //���ꂼ��̌X�����ŏ��l�𒴂��邩�i���l�ł�������Ɣ��肷��j
        bool xSlope = (xDire && centorMinSlope.x <= centorSlope.x) || (!xDire && centorSlope.x <= centorMinSlope.x);
        bool ySlope = (yDire && centorMinSlope.y <= centorSlope.y) || (!yDire && centorSlope.y <= centorMinSlope.y);

        Debug.Log(centorMinSlope + "|" + centorSlope + "|" + GetProjection(_vector2Right, _vector2Right + _vector2Up));
        Debug.Log(xDire + ":" + yDire);
        //�n�_���ʂ��猩�Ď΂߂̈ʒu�ł���
        if(slopeDire.x != 0 && slopeDire.y != 0)
        {
            Debug.Log("toXY");
            //�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
            if (xSlope && ySlope)
            {
                return true;
            }
        }
        //�n�_���ʂ��猩��X�������ɂ���
        else if(slopeDire.x != 0)
        {
            Debug.Log("toX");
            //X���ɂ����āA�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
            if (xSlope)
            {
                return true;
            }
        }
        //�n�_���ʂ��猩��Y�������ɂ���
        else
        {
            Debug.Log("toY");
            //Y���ɂ����āA�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
            if (ySlope)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// <para>GetSlopeByStartToOrigin</para>
    /// <para>�n�_���璆�S�Ɍ�������b�X�����擾���܂�</para>
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private static Vector2 GetSlopeByStartToOrigin(Vector2 start)
    {
        //�ԋp�p
        Vector2 returnSlope = _vectorZero;

        //����
        //�͈͂��ォ
        if (_collisionRange.x < start.x)
        {
            returnSlope += -_vector2Right;
        }
        //�͈͂�艺��
        else if (start.x < -_collisionRange.x)
        {
            returnSlope += _vector2Right;
        }

        //�c��
        //�͈͂��ォ
        if (_collisionRange.y < start.y)
        {
            returnSlope += -_vector2Up;
        }
        //�͈͂�艺��
        else if (start.y < -_collisionRange.y)
        {
            returnSlope += _vector2Up;
        }

        //�ԋp
        return returnSlope;
    }

    /// <summary>
    /// <para>GetProjection</para>
    /// <para>�Ώۃx�N�g����n�ʃx�N�g���ɑ΂��A�ˉe���s�����x�N�g�����o�͂��܂�</para>
    /// </summary>
    /// <param name="target">�Ώۃx�N�g��</param>
    /// <param name="ground">�n�ʃx�N�g��</param>
    /// <returns>�ˉe�x�N�g��</returns>
    private static Vector2 GetProjection(Vector2 target, Vector2 ground)
    {
        //�ˉe���Z�o
        Vector2 multiTG = target * ground;
        Vector2 powG = ground * ground;
        Vector2 projection = multiTG / powG;
        Debug.Log("Pro:" + multiTG + "|" + powG + "|" + projection);
        //�v�Z
        Vector2 returnPro = target * projection;

        return returnPro;
    }

    #endregion
}
