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
            //Debug.Log(nearEdgeIndex + "nearindex");
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
            //Debug.Log(nearEdgeIndex + "nearindex");
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
        float minDistance = -1;

        //�I�u�W�F�N�g�ۑ��p
        Transform localObj = target.MyTransform;
        //�I�u�W�F�N�g�̒��S���W
        Vector3 origin = target.Data.position;
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
            distance = Vector3.Distance(origin, localEdge);
            //Debug.Log(distance + "dis " + localEdge + "local " + edge + "edge");
            //�Z�o���ʂ��ۑ�����Ă��鋗�����傫�� �܂��� ����łȂ� �ꍇ�͉������Ȃ�
            if(minDistance < distance && minDistance != -1)
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
            //Debug.Log(lineEdge + "lineEdge");
            //���_�Ɗe�Ώۂ̒��_�����Ԑ� �� Collider �ɏd�Ȃ邩����
            if(CheckLineOverlapByCollider(edgePos[edge], edgePos[lineEdge], collider))
            {
                //�d�Ȃ��Ă���Ɣ���
                return true;
            }
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
        if (CheckLineSlopeByEdge(startPoint, endPoint, edges))
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
    /// <para>�n�_�ƏI�_�����Ԑ��̌X�� �� �n�_�Ɗe���_���W�����Ԑ��̌X���� �ł��邩�������܂�</para>
    /// </summary>
    /// <param name="start">���̎n�_</param>
    /// <param name="end">���̏I�_</param>
    /// <param name="edges">���_���W���X�g</param>
    /// <returns>�͈͓�����</returns>
    private static bool CheckLineSlopeByEdge(Vector2 start, Vector2 end, Vector2[] edges)
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

        //���_�x�N�g�����������ɑ��� ���� ���̎��Ƃ͕ʂ̐����x�N�g���̌X�����������_�x�N�g���̌X�������͈͓̔��ł��邩
        bool vecJustX = (edges[0].x == edges[1].x) && (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y);
        bool vecJustY = (edges[0].y == edges[1].y) && (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x);

        //���_�x�N�g���̌X���̋����̐������擾
        bool vecXSign = edgeMinSlope.x < edgeMaxSlope.x;
        bool vecYSign = edgeMinSlope.y < edgeMaxSlope.y;

        //���̏󋵉��ɂāA���̎��̐����x�N�g���̌X�����������_�x�N�g���̌X�������ȏ�ł��邩
        bool vecOverX = (vecXSign && edgeMinSlope.x <= lineSlope.x) || (!vecXSign && lineSlope.x <= edgeMinSlope.x);
        bool vecOverY = (vecYSign && edgeMinSlope.y <= lineSlope.y) || (!vecYSign && lineSlope.y <= edgeMinSlope.y);
        Debug.Log(edges[0] +":" + edges[1] + ":" + vecJustX + " " + vecXSign +" " + vecOverX + "|" + vecJustY + " " + vecXSign + " " + vecOverY);
        if((vecJustX && vecOverX) || (vecJustY && vecOverY))
        {
            //�͈͓��ł���
            return true;
        }
        
        //�����x�N�g���̌X�������_���W�x�N�g���̌X���͈͓̔��ł��邩
        if (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x
            && edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y)
        {
            //�͈͓��ł���
            return true;
        }

        //�͈͊O�ł���
        return false;
    }
    #endregion
}
