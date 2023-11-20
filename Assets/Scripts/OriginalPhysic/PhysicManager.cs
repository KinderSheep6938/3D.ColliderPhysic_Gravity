/// -----------------------------------------------------------------
/// PhysicManager.cs
/// 
/// �쐬���F2023/11/17
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace PhysicLibrary.Manager
{
    using UnityEngine;
    using ColliderLibrary;
    using VectorMath;

    public class PhysicManager
    {
        #region �ϐ�
        private static readonly Vector3 _gravityScale = new(0f, -0.981f, 0f);
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector3 _vectorRight = Vector3.right;
        private static readonly Vector3 _vectorUp = Vector3.up;
        private static readonly Vector3 _vectorForward = Vector3.forward;
        #endregion

        #region �v���p�e�B

        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>Gravity</para>
        /// <para>�Ώۂ̕����ɑ΂���d�͉����x���Z�o���܂�</para>
        /// </summary>
        /// <param name="physic">�Ώۂ�PhysicData</param>
        /// <returns>�Ώۂ̏d�͉����x</returns>
        public static Vector3 Gravity(PhysicData physic)
        {
            //�d�͉����x���Z�o : ���� x �d��
            Vector3 acceleration = physic.mass * _gravityScale;
            //�d�͉����x�����Ԑϕ� : �d�͉����x x �o�ߎ���
            Vector3 gravityForce = physic.force + (acceleration * Time.fixedDeltaTime);
            //�ԋp
            return gravityForce;
        }

        /// <summary>
        /// <para>ForceToVelocityByCollider</para>
        /// <para>�Ώۂ̗͂�^����ꂽCollider���������������x�ɕϊ����܂�</para>
        /// </summary>
        /// <param name="physic">�Ώۂ̕���</param>
        /// <param name="collision">�Փ˂���Collider</param>
        /// <returns>���ۂ̑��x</returns>
        public static Vector3 RepulsionForceByCollider(PhysicData physic, CollisionData collision)
        {
            //�e���̗͂�@���x�N�g���Ɏˉe
            //X��
            Vector3 normalX = ForceByNormal(_vectorRight * physic.force.x, collision);
            //Y��
            Vector3 normalY = ForceByNormal(_vectorUp * physic.force.y, collision);
            //Z��
            Vector3 normalZ = ForceByNormal(_vectorForward * physic.force.z, collision);

            //���v�l
            Debug.Log("no" + normalX + ":" + normalY + ":" + normalZ);
            Vector3 sumNormal = normalX + normalY + normalZ;

            //�����͂��Z�o���܂�
            Debug.Log(sumNormal + ":" + VerticalForceBySurface(collision));
            //������
            Vector3 repulsionForce = -(physic.reboundRatio * GetTo.V3Projection(sumNormal,VerticalForceBySurface(collision)));
            Debug.Log("Re" + repulsionForce);
            //�����͂����Z
            sumNormal = repulsionForce;

            return sumNormal;
        }

        /// <summary>
        /// <para>ForceByNormal</para>
        /// <para>�͂��e�@���x�N�g���Ɏˉe���A</para>
        /// </summary>
        /// <param name="force">�ˉe�����</param>
        /// <param name="collision">�@���x�N�g��</param>
        /// <returns></returns>
        private static Vector3 ForceByNormal(Vector3 force, CollisionData collision)
        {
            //�͂��Ȃ��ꍇ�� 0 ��Ԃ�
            if(force.x == 0 && force.y == 0 && force.z == 0)
            {
                return _vectorZero;
            }

            Debug.Log("trVe:" + collision.collider.transform.up + ":" + collision.collider.transform.right + ":" + collision.collider.transform.forward);
            //�͂��e���̖@���x�N�g���Ɏˉe
            Vector3 normalForceUD = GetTo.V3Projection(force, collision.collider.transform.up);
            Vector3 normalForceRL = GetTo.V3Projection(force, collision.collider.transform.right);
            Vector3 normalForceFB = GetTo.V3Projection(force, collision.collider.transform.forward);

            Debug.Log("no^" + normalForceUD + ":" + normalForceRL + ":" + normalForceFB);
            //�ˉe�x�N�g�������v
            Vector3 sumForce = normalForceUD + normalForceRL + normalForceFB;

            return sumForce;
        }

        /// <summary>
        /// <para>VerticalForceBySurface</para>
        /// <para>�ʂɑ΂��鐂���������擾���܂�</para>
        /// </summary>
        /// <param name="collision">�Ώۂ�Collider</param>
        /// <returns>�ʂɑ΂��鐂������</returns>
        private static Vector3 VerticalForceBySurface(CollisionData collision)
        {
            //�Փ˒n�_�𐳋K��
            Vector3 norVector = collision.collider.transform.InverseTransformPoint(collision.point).normalized;

            //�����֌W�Ȃ��Ɉ�ԍ��������𔻒肷��
            //�e�����̐�Βl���擾
            float norX = Mathf.Abs(norVector.x);
            float norY = Mathf.Abs(norVector.y);
            float norZ = Mathf.Abs(norVector.z);
            //�e��Βl�̍ő�l���擾
            float maxNor = Mathf.Max(norX, norY, norZ);

            //X������ԍ���
            if(maxNor == norX)
            {
                //���̒l�ł���
                if(0 < norVector.x)
                {
                    return -collision.collider.transform.right;
                }
                //���̒l�ł���
                else
                {
                    return collision.collider.transform.right;
                }
            }
            //Y������ԍ���
            else if(maxNor == norY)
            {
                //���̒l�ł���
                if (0 < norVector.y)
                {
                    return -collision.collider.transform.up;
                }
                //���̒l�ł���
                else
                {
                    return collision.collider.transform.up;
                }
            }
            //Z������ԍ���
            else
            {
                //���̒l�ł���
                if (0 < norVector.z)
                {
                    return -collision.collider.transform.forward;
                }
                //���̒l�ł���
                else
                {
                    return collision.collider.transform.forward;
                }
            }
        }

        
        #endregion
    }
}
