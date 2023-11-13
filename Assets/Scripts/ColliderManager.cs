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
    //���������̍ŏ�����
    private const float MIN_CHECK_DISTANCE = 0.01f;

    //Collider��񋤗L�p
    private static List<OriginalCollider> _worldInColliders = new();

    //��bVector���ۑ��p
    private static readonly Vector3 _vector3Up    = Vector3.up;
    private static readonly Vector3 _vector3Right = Vector3.right;
    private static readonly Vector3 _vector3Flont = Vector3.forward;

    //Collider�̏Փ˔���͈�
    private static readonly Vector3 _collisionRange = Vector3.one / HALF;

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
            nearEdge = GetNearEdgeByOrigin(target, collider.Data.edgePos);
            //�ł��߂����_���W �� �����Ώۂ�Collider �̓����ɂ���
            if (CheckPointInCollider(nearEdge, target))
            {
                Debug.Log("MyCollision");
                //�Փ˔��肪����
                return true;
            }

            //�����Ώۂ̒��_���W ���� �ł��߂����_���W ���i�[
            nearEdge = GetNearEdgeByOrigin(collider, target.Data.edgePos);
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
    /// <para>GetNearEdgeByOrigin</para>
    /// <para>�Ώۂ̃I�u�W�F�N�g���瑊�ΓI�ɍł��߂����_���W���擾���܂�</para>
    /// </summary>
    /// <param name="target">���_</param>
    /// <param name="edges">���_���W���X�g</param>
    /// <returns>�ł��߂����_���W</returns>
    private static Vector3 GetNearEdgeByOrigin(OriginalCollider target, Vector3[] edges)
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
    #endregion
}
