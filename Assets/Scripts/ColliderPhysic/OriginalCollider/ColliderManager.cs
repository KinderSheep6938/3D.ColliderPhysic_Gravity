/// -----------------------------------------------------------------
/// ColliderManager.cs�@Collider���萧��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.Manager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using OriginalMath;
    using ColliderLibrary.DataManager;
    using ColliderLibrary.Collision;

    /// <summary>
    /// <para>ColliderManager</para>
    /// <para>�����蔻��𐧌䂵�܂�</para>
    /// </summary>
    public class ColliderManager
    {
        #region �ϐ�
        //��bVector���ۑ��p
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector2 _vector2Up = Vector2.up;
        private static readonly Vector2 _vector2Right = Vector2.right;

        //�Փ˔���͈͂̍ő�l
        private static readonly float _collisionRange = GetTo.MaxRange;
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>CheckCollision</para>
        /// <para>Collider��񂩂�Փ˂��Ă��邩�������܂�</para>
        /// </summary>
        /// <param name="collider">Collider���</param>
        /// <returns>�Փ˔���</returns>
        public static CollisionData CheckCollision(ColliderData collider)
        {
            //�����ΏۂɈ�ԋ߂����_���W
            Vector3 nearEdge;
            //���̒��_���W�̃C���f�b�N�X�ۑ��p
            int nearEdgeIndex;
            //�ԋp�p�f�[�^�\��
            CollisionData returnData;

            //�f�[�^�ꎞ�ۑ��p
            List<Transform> saveTarget = new List<Transform>();
            saveTarget.Clear();
            List<Vector3> savePoint = new List<Vector3>();

            //���L���X�g����SCollider�����擾���A�Փˌ������s���܂�
            foreach (ColliderData target in ColliderDataManager.ColliderInWorld)
            {
                //�����Ώۂ� ���g �ł���
                if (target.transform == collider.transform)
                {
                    //�������Ȃ�
                    continue;
                }

                //���g�̒��S���W �� �����Ώۂ�Collider �̓����ɂ���
                if (CollisionCheck.CheckPointInCollider(collider.position, target))
                {
                    //�Փˏ���ݒ肷��
                    saveTarget.Add(target.transform); 
                    savePoint.Add(collider.position);
                }

                //�����Ώ��̒��S���W �� ���g��Collider �̓����ɂ���
                if (CollisionCheck.CheckPointInCollider(target.position, collider))
                {
                    //�Փˏ���ݒ肷��
                    saveTarget.Add(target.transform);
                    savePoint.Add(target.position);
                }

                //���g�̒��_���W ���� �ł������Ώۂɋ߂����_���W ���i�[
                nearEdge = GetNearEdgeByCollider(target, collider.edgePos);
                //���̒��_���W�̃C���f�b�N�X�擾
                nearEdgeIndex = Array.IndexOf(collider.edgePos, nearEdge);
                //���̒��_����ʏ�ɕʒ��_�֌��Ԃ��Ƃ̂ł���� �� �����Ώۂ�Collider �ɏd�Ȃ�
                if (CollisionCheck.CheckPlaneLineOverlap(nearEdgeIndex, collider.edgePos, target.transform))
                {
                    //�Փˏ���ݒ肷��
                    saveTarget.Add(target.transform);
                    savePoint.Add(nearEdge);
                }

                //�����Ώۂ̒��_���W ���� �ł����g�ɋ߂����_���W ���i�[
                nearEdge = GetNearEdgeByCollider(collider, target.edgePos);
                //���̒��_���W�̃C���f�b�N�X�擾
                nearEdgeIndex = Array.IndexOf(target.edgePos, nearEdge);
                //���̒��_����ʏ�ɕʒ��_�֌��Ԃ��Ƃ̂ł���� �� ���g��Collider �ɏd�Ȃ�
                if (CollisionCheck.CheckPlaneLineOverlap(nearEdgeIndex, target.edgePos, collider.transform))
                {
                    //�Փˏ���ݒ肷��
                    saveTarget.Add(target.transform);
                    savePoint.Add(GetNearEdgeByCollider(target, collider.edgePos));
                }

            }

            //�ݒ肳�ꂽ��񂪂Ȃ�
            if(saveTarget.Count == 0)
            {
                //�ԋp�p�ݒ�
                returnData.flag = false;
                returnData.collider = saveTarget.ToArray();
                returnData.point = savePoint.ToArray();
                returnData.interpolate = _vectorZero;
                //��̏Փˏ���ԋp����
                return returnData;
            }
            //�ݒ肳�ꂽ��񂪂���
            else
            {
                //�ԋp�p�ݒ�
                returnData.flag = true;
                returnData.collider = saveTarget.ToArray();
                returnData.point = savePoint.ToArray();
                returnData.interpolate = _vectorZero;
                //�Փˏ���ԋp����
                return returnData;
            }
        }

        /// <summary>
        /// <para>GetNearEdgeByCollider</para>
        /// <para>�Ώۂ�Collider���瑊�ΓI�ɍł��߂����_���W���擾���܂�</para>
        /// </summary>
        /// <param name="target">�I�u�W�F�N�g���</param>
        /// <param name="edges">���_���W���X�g</param>
        /// <returns>�ł��߂����_���W</returns>
        private static Vector3 GetNearEdgeByCollider(ColliderData target, Vector3[] edges)
        {
            //�Z�o���ʕۑ��p
            float distance;
            //�ł��߂������ۑ��p�@�����l�Ƃ���-1���i�[����
            float minDistance = float.MaxValue;

            //�I�u�W�F�N�g�ۑ��p
            Transform localObj = target.transform;
            //���[�J���ϊ��p
            Vector3 localEdge;

            //�ԋp�p
            Vector3 returnPos = default;

            //���_����ł��߂����_���W������
            foreach (Vector3 edge in edges)
            {
                //���[�J���ϊ�
                localEdge = localObj.InverseTransformPoint(edge);
                //�����Z�o
                distance = Vector3.Distance(_vectorZero, localEdge);

                //�Z�o���ʂ��ۑ�����Ă��鋗�����傫�� �܂��� ����łȂ� �ꍇ�͉������Ȃ�
                if (minDistance < distance)
                {
                    continue;
                }

                //�����X�V
                minDistance = distance;
                //���W�ݒ�
                returnPos = edge;
            }
            //�����I��
            return returnPos;
        }
        #endregion
    }
}
