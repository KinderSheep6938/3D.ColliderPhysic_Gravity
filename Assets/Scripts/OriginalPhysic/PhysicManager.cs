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
        /// <para>�Ώۂ̗͂�^����ꂽCollider�������������͂ɕϊ����܂�</para>
        /// </summary>
        /// <param name="physic">�Ώۂ̕���</param>
        /// <param name="collision">�Փ˂���Collider</param>
        /// <returns>���ۂ̗�</returns>
        public static Vector3 RepulsionForceByCollider(PhysicData physic, CollisionData collision)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //�e���̗͂�@���x�N�g���Ɏˉe
            //X��
            Vector3 normalX = ForceByNormal(_vectorRight * physic.force.x, collision);
            //Y��
            Vector3 normalY = ForceByNormal(_vectorUp * physic.force.y, collision);
            //Z��
            Vector3 normalZ = ForceByNormal(_vectorForward * physic.force.z, collision);

            //���v�l�����Ƃ��đ��
            //Debug.Log("no" + normalX + ":" + normalY + ":" + normalZ);
            Vector3 returnForce = normalX + normalY + normalZ;

            //�����͂��Z�o���܂�
            //Debug.Log(returnForce + ":" + VerticalForceBySurface(collision));
            //������
            Vector3 repulsionForce = -(physic.reboundRatio * GetTo.V3Projection(returnForce,VerticalForceBySurface(collision)));
            //Debug.Log("Re" + repulsionForce);

            //�����R�͂��Z�o
            float verticalResistance = GetTo.V3Projection(Gravity(physic), VerticalForceBySurface(collision)).magnitude;
            //���C�͂��Z�o
            float drugPower = verticalResistance * physic.drug;

            //�ŏI
            returnForce = AddRepulsionForce(returnForce, repulsionForce, drugPower);

            return returnForce;
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

            //Debug.Log("trVe:" + collision.collider.transform.up + ":" + collision.collider.transform.right + ":" + collision.collider.transform.forward);
            //�͂��e���̖@���x�N�g���Ɏˉe
            Vector3 normalForceUD = GetTo.V3Projection(force, collision.collider.transform.up);
            Vector3 normalForceRL = GetTo.V3Projection(force, collision.collider.transform.right);
            Vector3 normalForceFB = GetTo.V3Projection(force, collision.collider.transform.forward);

            //Debug.Log("no^" + normalForceUD + ":" + normalForceRL + ":" + normalForceFB);
            //�ˉe�x�N�g�������v
            Vector3 sumForce = normalForceUD + normalForceRL + normalForceFB;

            return sumForce;
        }

        /// <summary>
        /// <para>AddRepulsion</para>
        /// <para>�����͂������������̗̂͂��Z�o���܂�</para>
        /// </summary>
        /// <param name="force">���݂̕��̗̂�</param>
        /// <param name="repulsion">������</param>
        /// <returns>�����͂������������̗̂�</returns>
        private static Vector3 AddRepulsionForce(Vector3 force, Vector3 repulsion, float moveDrugScale)
        {
            //�ԋp�p
            Vector3 returnVector = _vectorZero;

            //�����͂̊e���ɗ͂�����ꍇ�́A���̎��̗͂𔽔��͂ɒu��������
            //�Ȃ��ꍇ�́A���̂܂܂̗͂��i�[����

            //�������������͂��Ȃ�
            if(repulsion == _vectorZero)
            {
                //���̏�Ŏ~�܂�
                return returnVector;
            }

            //X���ɑ΂��Ĕ����͂�����
            if(repulsion.x != 0)
            {
                //�u������
                returnVector += _vectorRight * repulsion.x;
            }
            //�����͂��Ȃ�
            else
            {
                //���̂܂�
                returnVector += _vectorRight * force.x;

                //�����C�͂����Z������
                returnVector += MoveDrugToVector3(force.x, _vectorRight, moveDrugScale);
            }

            //Y���ɑ΂��Ĕ����͂�����
            if(repulsion.y != 0)
            {
                //�u������
                returnVector += _vectorUp * repulsion.y;
            }
            //�����͂��Ȃ�
            else
            {
                //���̂܂�
                returnVector += _vectorUp * force.y;

                //�����C�͂����Z������
                returnVector += MoveDrugToVector3(force.y,_vectorUp, moveDrugScale);
            }

            //Z���ɑ΂��Ĕ����͂�����
            if(repulsion.z != 0)
            {
                //�u������
                returnVector += _vectorForward * repulsion.z;
            }
            //�����͂��Ȃ�
            else
            {
                //���̂܂�
                returnVector += _vectorForward * force.z;

                //�����C�͂����Z������
                returnVector += MoveDrugToVector3(force.z, _vectorForward, moveDrugScale);
            }

            //�ԋp
            return returnVector;
        }

        /// <summary>
        /// <para>MoveDrugToVector3</para>
        /// <para>�����C�͂�Vector3�ɕϊ����܂�</para>
        /// </summary>
        /// <param name="nowForceScale">���݂̕��̂ɂ������</param>
        /// <param name="vector">����</param>
        /// <param name="moveDrugScale">�����C��</param>
        /// <returns>Vector3�ɕϊ����ꂽ�����C��</returns>
        private static Vector3 MoveDrugToVector3(float nowForceScale,Vector3 vector, float moveDrugScale)
        {
            //�ԋp�p
            Vector3 returnVector;

            Debug.Log("dr:" + nowForceScale + ":" + vector + ":" + moveDrugScale);

            //���݂̗͂����C�͂��Ⴂ
            if(Mathf.Abs(nowForceScale) <= moveDrugScale)
            {
                //��̎Z�o���O�ƂȂ�悤�ɐݒ�
                returnVector = -vector * nowForceScale;
            }
            //���̕����ɓ����Ă���
            else if (0 < nowForceScale)
            {
                //���C�͂�ݒ�
                returnVector = -vector * moveDrugScale;
            }
            //���̕����ɓ����Ă���
            else
            {
                //���C�͂�ݒ�
                returnVector = vector * moveDrugScale;
            }

            return returnVector;
        }

        /// <summary>
        /// <para>VerticalForceBySurface</para>
        /// <para>�ʂɑ΂��鐂���������擾���܂�</para>
        /// </summary>
        /// <param name="collision">�Ώۂ�Collider</param>
        /// <returns>�ʂɑ΂��鐂������</returns>
        private static Vector3 VerticalForceBySurface(CollisionData collision)
        {
            //�ȈՏՓ˒n�_���擾
            Vector3 collsionPoint = collision.collider.transform.InverseTransformPoint(collision.point - collision.interpolate);

            //�����֌W�Ȃ��Ɉ�ԍ��������𔻒肷��
            //�e�����̐�Βl���擾
            float norX = Mathf.Abs(collsionPoint.x);
            float norY = Mathf.Abs(collsionPoint.y);
            float norZ = Mathf.Abs(collsionPoint.z);
            //�e��Βl�̍ő�l���擾
            float maxNor = Mathf.Max(norX, norY, norZ);
            //Debug.Log(collsionPoint + "xyz" + norX + ":" + norY + ":" + norZ);

            //X������ԍ���
            if(maxNor == norX)
            {
                //���̒l�ł���
                if(0 < collsionPoint.x)
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
                if (0 < collsionPoint.y)
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
                if (0 < collsionPoint.z)
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
