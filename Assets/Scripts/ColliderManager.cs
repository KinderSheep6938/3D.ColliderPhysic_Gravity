/// -----------------------------------------------------------------
/// ColliderEngine.cs�@Collider���萧��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public const int rightUp = 0;   //�E��
        public const int rightDown = 1; //�E��
        public const int leftUp = 2;    //����
        public const int leftDown = 3;  //����
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

        //���L���X�g����SCollider�����擾���A�Փˌ������s���܂�
        foreach(OriginalCollider target in _worldInColliders)
        {
            //�����Ώۂ� ���g �ł���
            if(target == collider)
            {
                //�������Ȃ�
                continue;
            }

            //���g�̒��_���W ���� �ł��߂����_���W ���i�[
            nearEdge = GetNearEdgeByCollider(target, collider.Data.edgePos);
            //�ł��߂����_���W �� �����Ώۂ�Collider �̓����ɂ���
            if (CheckPointInCollider(nearEdge, target))
            {
                Debug.Log("MyCollision");
                //�Փ˔��肪����
                return true;
            }

            //�����Ώۂ̒��_���W ���� �ł��߂����_���W ���i�[
            nearEdge = GetNearEdgeByCollider(collider, target.Data.edgePos);
            //�ł��߂����_���W �� ���g��Collider �̓����ɂ���
            if(CheckPointInCollider(nearEdge, collider))
            {
                Debug.Log("Collision");
                return true;
            }

            //���g�̒��S���W �� �����Ώۂ�Collider �̓����ɂ���
            if(CheckPointInCollider(collider.Data.position, target))
            {
                Debug.Log("MyCenterCollision");
                //�Փ˔��肪����
                return true;
            }

            //�����Ώ��̒��S���W �� ���g��Collider �̓����ɂ���
            if(CheckPointInCollider(target.Data.position, collider))
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
    /// <para>CheckLineOverlapByCollider</para>
    /// <para>����Collider�ɏd�Ȃ邩�������܂�</para>
    /// </summary>
    /// <returns></returns>
    private static bool CheckLineOverlapByCollider(Vector3 startPoint, Vector3 endPoint, OriginalCollider collider)
    {
        //���̎n�_�ƏI�_�����[�J���ϊ�
        Vector3 localStart = collider.MyTransform.InverseTransformPoint(startPoint);
        Vector3 localEnd = collider.MyTransform.InverseTransformPoint(endPoint);

        //�e�������Ƃ̖ʂŔ�����s��
        //Z�ʁiX�� �� Y���j
        Vector2 vec2Start = _vector3Right * localStart.x + _vector3Up * localStart.y;
        Vector2 vec2End = _vector3Right * localEnd.x + _vector3Up * localEnd.y;
        //�ʂɏd�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
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
            returnEdges[0] = _planeEdge[EdgeByPlane.leftUp];
            returnEdges[1] = _planeEdge[EdgeByPlane.rightDown];
        }
        //�������W�� �E�� �܂��� ���� �ɂ���
        else if((_collisionRange.x < point.x && point.y < -_collisionRange.y)
            || (point.x < -_collisionRange.x && _collisionRange.y < point.y))
        {
            //�E��ƍ�����ԋp
            returnEdges[0] = _planeEdge[EdgeByPlane.rightUp];
            returnEdges[1] = _planeEdge[EdgeByPlane.leftDown];
        }
        //�����Ώۂ� �����͈͓̔� �ł���
        else if(-_collisionRange.x <= point.x && point.x <= _collisionRange.x)
        {
            //�����Ώۂ� �㑤 �ɂ���
            if(_collisionRange.y < point.y)
            {
                //�E��ƍ����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rightUp];
                returnEdges[1] = _planeEdge[EdgeByPlane.leftUp];
            }
            //���� �ɂ���
            else
            {
                //�E���ƍ�����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rightDown];
                returnEdges[1] = _planeEdge[EdgeByPlane.leftDown];
            }
        }
        //�����Ώۂ� �c���͈͓̔� �ł���
        else
        {
            //�����Ώۂ� �E�� �ɂ���
            if (_collisionRange.x < point.x)
            {
                //�E��ƉE����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rightUp];
                returnEdges[1] = _planeEdge[EdgeByPlane.rightDown];
            }
            //���� �ɂ���
            else
            {
                //����ƍ�����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.leftUp];
                returnEdges[1] = _planeEdge[EdgeByPlane.leftDown];
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

        //�e�x�N�g�������𐳋K��
        lineSlope = lineSlope.normalized;
        edgeMaxSlope = edgeMaxSlope.normalized;
        edgeMinSlope = edgeMinSlope.normalized;

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
