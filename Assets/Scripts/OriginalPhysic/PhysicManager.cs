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
    using OriginalMath;
    using PhysicLibrary.Material;
    using PhysicLibrary.DataManager;

    public class PhysicManager
    {
        #region �ϐ�
        //�d��
        private static readonly Vector3 _gravityScale = new(0f, -9.81f, 0f);
        //��b�x�N�g��
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
        /// <para>RepulsionForceByCollider</para>
        /// <para>�Ώۂ̗͂𕨎������l�������l�ɕϊ����܂�</para>
        /// </summary>
        /// <param name="physic">�Ώۂ̕���</param>
        /// <returns>���ۂ̗�</returns>
        public static Vector3 ChangeForceByPhysicMaterials(PhysicData physic)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //�ԋp�p
            Vector3 returnForce = physic.force;
            //���g�̕������������擾
            PhysicMaterials myMaterial = physic.colliderInfo.material;
            //�Փː�̕������������擾
            PhysicMaterials collisionMaterial = PhysicDataManager.SearchPhysicByCollider(physic);

            //Debug.Log(myMaterial.bounciness == collisionMaterial.bounciness);

            //�����������擾
            Vector3 vertical = VerticalForceBySurface(physic.colliderInfo);
            //�����������擾
            Vector3 horizontal = HorizontalForceBySurface(physic.colliderInfo.Collision.collider, vertical);
            //�����R�͂��Z�o
            Vector3 verticalResistance = GetTo.V3Projection(physic.force, vertical);
            //�ʂɑ΂��Đ����ɉ�����Ă���͂��Z�o
            Vector3 horizontalForce = GetTo.V3Projection(physic.force, horizontal);
            //Debug.Log("f:" + returnForce + "s:" + (verticalResistance + horizontalForce));

            //�����C�W�����Z�o
            float combineDynamicDrug = GetTo.ValueCombine(myMaterial.dynamicDrug, collisionMaterial.dynamicDrug, myMaterial.drugCombine);
            //�Î~���C�W�����Z�o
            float combineStaticDrug = GetTo.ValueCombine(myMaterial.staticDrug, collisionMaterial.staticDrug, myMaterial.drugCombine);
            //���C�͂��l�����������ɂ�����͂��Z�o
            returnForce += AddDrug(horizontalForce, verticalResistance, combineDynamicDrug, combineStaticDrug);

            //Debug.Log(returnForce + ":" + VerticalForceBySurface(collision));
            //�����W�����Z�o
            float combineRep = GetTo.ValueCombine(myMaterial.bounciness, collisionMaterial.bounciness, myMaterial.bounceCombine);
            //�����͂��Z�o
            Vector3 repulsionForce = -(combineRep * GetTo.V3Projection(physic.force,vertical));
            //Debug.Log("Re" + repulsionForce);
            //�����͂��l�����������ɂ�����͂��Z�o
            returnForce = AddRepulsionForce(returnForce, repulsionForce);

            return returnForce;
        }

        private static Vector3 AddDrug(Vector3 horizontalForce, Vector3 verticalResistance, float dynamicDrug, float staticDrug)
        {
            //Debug.Log(horizontalForce + ":" + verticalResistance);
            //Vector3��float�ϊ�
            float horizontalValue = horizontalForce.sqrMagnitude;

            //�ő�Î~���C�͂��Z�o
            float maxStaticDrug = (verticalResistance * staticDrug).sqrMagnitude;

            //�����ɉ����͂��ő�Î~���C�͈ȉ��ł���
            if (horizontalValue <= maxStaticDrug)
            {
                return -horizontalForce;
            }
            //�����C�͂�ԋp
            return -horizontalForce * dynamicDrug;
        }



        /// <summary>
        /// <para>AddRepulsion</para>
        /// <para>�����͂������������̗̂͂��Z�o���܂�</para>
        /// </summary>
        /// <param name="force">���݂̕��̗̂�</param>
        /// <param name="repulsion">������</param>
        /// <returns>�����͂������������̗̂�</returns>
        private static Vector3 AddRepulsionForce(Vector3 force, Vector3 repulsion)
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
            }

            //�ԋp
            return returnVector;
        }

        /// <summary>
        /// <para>VerticalForceBySurface</para>
        /// <para>�ʂɑ΂��鐂���������擾���܂�</para>
        /// </summary>
        /// <param name="collision">�Ώۂ�Collider</param>
        /// <returns>�ʂɑ΂��鐂������</returns>
        private static Vector3 VerticalForceBySurface(IColliderInfoAccessible collision)
        {
            //�ȈՏՓ˒n�_���擾
            Vector3 collsionPoint = collision.Collision.collider.InverseTransformPoint(collision.Point);

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
                    return -collision.Collision.collider.right;
                }
                //���̒l�ł���
                else
                {
                    return collision.Collision.collider.right;
                }
            }
            //Y������ԍ���
            else if(maxNor == norY)
            {
                //���̒l�ł���
                if (0 < collsionPoint.y)
                {
                    return -collision.Collision.collider.up;
                }
                //���̒l�ł���
                else
                {
                    return collision.Collision.collider.up;
                }
            }
            //Z������ԍ���
            else
            {
                //���̒l�ł���
                if (0 < collsionPoint.z)
                {
                    return -collision.Collision.collider.forward;
                }
                //���̒l�ł���
                else
                {
                    return collision.Collision.collider.forward;
                }
            }
        }

        /// <summary>
        /// <para>HorizontalForceBySurface</para>
        /// <para>�ʂɑ΂��鐅���������擾���܂�</para>
        /// </summary>
        /// <param name="surface">�ʕ������擾�ł���Transform</param>
        /// <param name="vertical">��������</param>
        /// <returns>���������ȊO�̕����̘a</returns>
        private static Vector3 HorizontalForceBySurface(Transform surface, Vector3 vertical)
        {
            //�ԋp�p
            Vector3 sumHorizontal = _vectorZero;

            //�����������㉺�ł���
            if (vertical == surface.up || vertical == -surface.up)
            {
                sumHorizontal += surface.right;
                sumHorizontal += surface.forward;
                return sumHorizontal;
            }
            //�������������E�ł���
            else if (vertical == surface.right || vertical == -surface.right)
            {
                sumHorizontal += surface.up;
                sumHorizontal += surface.forward;
                return sumHorizontal;
            }
            //�����������O��ł���
            else
            {
                sumHorizontal += surface.right;
                sumHorizontal += surface.up;
                return sumHorizontal;
            }
        }

        #endregion 
    }
}
