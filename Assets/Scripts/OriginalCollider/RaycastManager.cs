using System.Collections;
using System.Collections.Generic;


namespace ColliderLibrary.Manager
{
    using UnityEngine;
    using OriginalMath;
    using ColliderLibrary;
    using ColliderLibrary.DataManager;
    using ColliderLibrary.Collision;

    #region Cast��{�f�[�^
    //Cast�̕ԋp�f�[�^�\��
    public struct CastHit
    {
        public bool collision;
        public ColliderData collider;
        public Vector3 point;
    }
    #endregion

    public class RaycastManager
    {
        #region �ϐ�
        //��b�x�N�g��
        private static readonly Vector3 _vectorZero = Vector3.zero;
        #endregion

        #region ���\�b�h
        public static CastHit Raycast(Vector3 origin, Vector3 vector, float length)
        {
            //�����x�N�g���𐳋K��
            Vector3 norVector = vector.normalized;
            //�Ǝ˓_�ɋ������̐��K�����������x�N�g�������Z�������̂��I�_�Ƃ���
            Vector3 lineEnd = origin + (norVector * length);

            //�Փ˔���
            bool collision = false;
            //���_���璸�_�ɑ΂��鋗���x�N�g���ۑ��p
            Vector3 originToEdge = _vectorZero;
            //�Z�o�����ۑ��p
            float colliderLength = 0;
            //�ŒZ�����ۑ��p
            float minLength = float.MaxValue;
            //Collider�ԋp�p�ϐ�
            ColliderData returnCollider = new();
            //�Փ˒n�_�ۑ��p
            Vector3 point = _vectorZero;

            //��ԋ߂�����������Collider���擾
            foreach(ColliderData collider in ColliderDataManager.ColliderInWorld)
            {
                //Raycast�̎n�_�ƏI�_�����Ԑ��ɏd�Ȃ�Ȃ�
                if (!CollisionCheck.CheckLineOverlapByCollider(origin, lineEnd, collider.transform))
                {
                    //�������X�L�b�v
                    continue;
                }

                //���_�ƈ�Ԍ��_�ɋ߂����_�Ƃ̋����x�N�g�����Z�o
                originToEdge = GetNearEdgeByOrigin(origin, collider.edgePos) - origin;
                //�����x�N�g����Raycast�x�N�g���Ɏˉe
                originToEdge = GetTo.V3Projection(originToEdge, lineEnd - origin);

                //�ˉe�x�N�g���̋������Z�o
                colliderLength = Vector3.Distance(origin, origin + originToEdge);
                //�ݒ肳��Ă���ŏ��������Z��
                if (colliderLength <= minLength)
                {
                    //�ŏ�������ݒ�
                    minLength = colliderLength;
                    //Collider����ݒ�
                    returnCollider = collider;
                    //�Փ˒n�_��ݒ�
                    point = origin + originToEdge;
                    //�Փ˔���ݒ�
                    collision = true;
                }
            }

            //�ԋp�p
            CastHit returnHit = new();
            //�ԋp�p�l�ݒ�
            returnHit.collision = collision;
            returnHit.collider = returnCollider;
            returnHit.point = point;

            return returnHit;
        }

        /// <summary>
        /// <para>GetNearEdgeByOrigin</para>
        /// <para>���_����ł��߂����_���W���擾���܂�</para>
        /// </summary>
        /// <param name="origin">���_</param>
        /// <param name="edges">���_���W���X�g</param>
        /// <returns>�ł��߂����_���W</returns>
        private static Vector3 GetNearEdgeByOrigin(Vector3 origin, Vector3[] edges)
        {
            //�Z�o���ʕۑ��p
            float distance;
            //�ł��߂������ۑ��p�@�����l�Ƃ���-1���i�[����
            float minDistance = float.MaxValue;

            //�ԋp�p
            Vector3 returnPos = default;

            //���_����ł��߂����_���W������
            foreach (Vector3 edge in edges)
            {
                //�����Z�o
                distance = Vector3.Distance(origin, edge);

                //�Z�o���ʂ��ۑ�����Ă��鋗�����傫���ꍇ�͉������Ȃ�
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

