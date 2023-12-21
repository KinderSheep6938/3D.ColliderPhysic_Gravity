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
    using ColliderLibrary.DataManager;
    using ColliderLibrary.Collision;
    using PhysicLibrary.Material;
    using PhysicLibrary.CollisionPhysic;

    /// <summary>
    /// <para>ColliderManager</para>
    /// <para>�����蔻��𐧌䂵�܂�</para>
    /// </summary>
    public class ColliderManager
    {
        #region �ϐ�
        //��bVector���ۑ��p
        private static readonly Vector3 _vectorZero = Vector3.zero;
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>CheckCollision</para>
        /// <para>Collider��񂩂�Փ˂��Ă��邩�������܂�</para>
        /// </summary>
        /// <param name="collider">Collider���</param>
        /// <returns>�Փ˔���</returns>
        public static bool CheckCollision(ColliderData collider, Vector3 interpolate = default, bool saveCollision = true)
        {
            //�����ΏۂɈ�ԋ߂����_���W
            Vector3 nearEdge;
            //���̒��_���W�̃C���f�b�N�X�ۑ��p
            int nearEdgeIndex;

            //�f�[�^�ꎞ�ۑ��p
            List<PhysicMaterials> saveTarget = new();
            saveTarget.Clear();
            List<int> saveEdge = new();
            saveEdge.Clear();
            List<Vector3> savePoint = new();
            savePoint.Clear();

            //���L���X�g����SCollider�����擾���A�Փˌ������s���܂�
            foreach (ColliderData target in ColliderDataManager.GetColliderToWorld())
            {
                //�����Ώۂ��폜����Ă���
                if(target.physic.transform == default)
                {
                    continue;
                }

                //�����Ώۂ� ���g �ł���
                if (target.physic.transform == collider.physic.transform)
                {
                    //�������Ȃ�
                    continue;
                }

                //���g�̒��_���W ���� �ł������Ώۂɋ߂����_���W ���i�[
                nearEdge = GetNearEdgeByCollider(target, collider.edgePos);
                //���̒��_���W�̃C���f�b�N�X�擾
                nearEdgeIndex = Array.IndexOf(collider.edgePos, nearEdge);
                //���g�̒��S���W �� �����Ώۂ�Collider �̓����ɂ���
                if (CollisionCheck.CheckPointInCollider(collider.position, target.physic.transform))
                {
                    //�Փˏ���ݒ肷��
                    Save(target);
                    continue;
                }

                //�����Ώ��̒��S���W �� ���g��Collider �̓����ɂ���
                if (CollisionCheck.CheckPointInCollider(target.position, collider.physic.transform))
                {
                    //�Փˏ���ݒ肷��
                    Save(target);
                    continue;
                }

                //���̒��_����ʏ�ɕʒ��_�֌��Ԃ��Ƃ̂ł���� �� �����Ώۂ�Collider �ɏd�Ȃ�
                if (CollisionCheck.CheckPlaneLineOverlap(nearEdgeIndex, collider.edgePos, target.physic.transform))
                {
                    //�Փˏ���ݒ肷��
                    Save(target);
                    continue;
                }

                //�����Ώۂ̒��_���W ���� �ł����g�ɋ߂����_���W ���i�[
                Vector3 nearTargetEdge = GetNearEdgeByCollider(collider, target.edgePos);
                //���̒��_���W�̃C���f�b�N�X�擾
                int nearTargetEdgeIndex = Array.IndexOf(target.edgePos, nearTargetEdge);
                //���̒��_����ʏ�ɕʒ��_�֌��Ԃ��Ƃ̂ł���� �� ���g��Collider �ɏd�Ȃ�
                if (CollisionCheck.CheckPlaneLineOverlap(nearTargetEdgeIndex, target.edgePos, collider.physic.transform))
                {
                    //�Փˏ���ݒ肷��
                    Save(target);
                    continue;
                }

            }

            //�ݒ肳�ꂽ��񂪂Ȃ�
            if (saveTarget.Count == 0)
            {
                //�Փ˔��肪�Ȃ�
                return false;
            }
            //�ݒ肳�ꂽ��񂪂���
            else
            {
                //���W�⊮������
                if (interpolate != default)
                {
                    RemoveInterpolate(interpolate, ref savePoint);
                }
                //�Փ˃f�[�^��ۑ����邩
                if (saveCollision)
                {
                    //�Փ˃f�[�^���i�[
                    SetCollisionData(collider.physic, saveTarget.ToArray(), saveEdge.ToArray(), savePoint.ToArray(), interpolate);
                }
                //�Փ˔��肪����
                return true;
            }

            //���X�g�ۑ��p���[�J�����\�b�h
            void Save(ColliderData target)
            {
                //�Փˏ���ݒ肷��
                saveTarget.Add(target.physic);
                saveEdge.Add(nearEdgeIndex);
                savePoint.Add(nearEdge);
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
            Transform localObj = target.physic.transform;
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

        /// <summary>
        /// <para>RemoveInterpolate</para>
        /// <para>�Փ˒n�_�̕⊮���������܂�</para>
        /// </summary>
        /// <param name="interpolate">�⊮����</param>
        /// <param name="point">�o�^���ꂽ�Փ˒n�_</param>
        private static void RemoveInterpolate(Vector3 interpolate, ref List<Vector3> point)
        {
            //�Փ˂��P�����Ȃ��ꍇ
            if (point.Count == 1)
            {
                //�⊮�폜
                point[0] -= interpolate;
                return;
            }
            //�����̕⊮������
            for (int i = 0; i < point.Count; i++)
            {
                //�⊮�폜
                point[i] -= interpolate;
            }
            return;
        }

        /// <summary>
        /// <para>SetCollisionData</para>
        /// <para>�Փ˃f�[�^��o�^���܂�</para>
        /// </summary>
        /// <param name="myPhysic">���g��Physic</param>
        /// <param name="collisionPhysic">�Փ˂̂������ePhysic</param>
        /// <param name="point">�e�Փ˒n�_</param>
        private static void SetCollisionData(PhysicMaterials myPhysic, PhysicMaterials[] collisionPhysic,int[] edgeId, Vector3[] point, Vector3 interpolate)
        {
            //�⊮���x�͂��邩
            bool interpolateFlag = (interpolate != _vectorZero);

            //�Փ˂��P�����Ȃ��ꍇ
            if (collisionPhysic.Length == 1)
            {
                //�o�^
                CollisionPhysicManager.SetCollision(myPhysic, collisionPhysic[0],edgeId[0] , point[0], interpolateFlag);
                return;
            }
            //�����̏Փ˂�o�^
            for (int i = 0; i < collisionPhysic.Length; i++)
            {
                //Debug.Log(collisionPhysic[i] + "[" + point[i] + "[" + interpolate);
                //�o�^
                CollisionPhysicManager.SetCollision(myPhysic, collisionPhysic[i],edgeId[i], point[i], interpolateFlag);
                //Debug.Log("1<<");
            }
            return;
        }
        #endregion
    }
}
