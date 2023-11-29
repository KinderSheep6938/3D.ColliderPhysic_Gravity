/// -----------------------------------------------------------------
/// PhysicManager.cs ������������
/// 
/// �쐬���F2023/11/17
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary.Manager
{
    using UnityEngine;
    using OriginalMath;
    using PhysicLibrary.Material;
    using PhysicLibrary.CollisionPhysic;

    /// <summary>
    /// <para>PhysicManager</para>
    /// <para>�����������Ǘ����܂�</para>
    /// </summary>
    public class PhysicManager
    {
        #region �ϐ�
        //�߂荞�ݔ����{��
        private const float REPULESSION_RATIO = 1.02f;
        //�d��
        private static readonly Vector3 _gravityScale = new(0f, -9.81f, 0f);
        //��b�x�N�g��
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector3 _vectorRight = Vector3.right;
        private static readonly Vector3 _vectorUp = Vector3.up;
        private static readonly Vector3 _vectorForward = Vector3.forward;
        #endregion

        #region �v���p�e�B
        //�����d��
        public static float CommonMass { get => 1f; }
        //�����d��
        public static Vector3 CommonGravity { get => _gravityScale; }
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
            //�d�͂��O�̏ꍇ�͏������Ȃ�
            if (physic.gravity == _vectorZero)
            {
                return _vectorZero;
            }

            //�d�͉����x���Z�o : ���� x �d��
            Vector3 acceleration = physic.mass * physic.gravity;
            //�d�͉����x�����Ԑϕ� : �d�͉����x x �o�ߎ���
            Vector3 gravityForce = acceleration * Time.fixedDeltaTime;
            //�ԋp
            return gravityForce;
        }

        /// <summary>
        /// <para>NoForceToCollision</para>
        /// <para>���̂ɂ�����͂��Ȃ��߂荞�ݐ�����s���܂�</para>
        /// </summary>
        /// <param name="physic">�Ώۂ̕���</param>
        /// <param name="otherPhysic">�߂荞�ݏ��</param>
        /// <param name="AirResistance">��C��R</param>
        /// <returns>�߂荞�ݐ���p�̗�</returns>
        public static Vector3 NoForceToCollision(PhysicData physic, OtherPhysicData otherPhysic)
        {
            Debug.Log(physic.colliderInfo.material.transform + ":" + otherPhysic.collision.transform);
            //����������Scale�ɐ��K�����Ď擾�i���������ł���ʂ܂ł̋����x�N�g���j
            Vector3 surfaceVerticalDis = VerticalDirectionBySurface(otherPhysic,otherPhysic.collision.transform.lossyScale);
            //�Փ˒n�_�܂ł̐����ȋ����x�N�g��
            Vector3 collisionVerticalDis = GetTo.V3Projection(otherPhysic.point - otherPhysic.collision.transform.position, surfaceVerticalDis);
            //�Փ˒n�_����ʂ܂ł̋����x�N�g�����擾
            Vector3 collisionToSurface = surfaceVerticalDis - collisionVerticalDis;
            //Debug.Log(surfaceVerticalDis + ":" + collisionVerticalDis + ":" + collisionToSurface);
        

            //��C��R�����l�����ĉ��Z
            Vector3 returnForce = collisionToSurface + (collisionToSurface * physic.airResistance);

            return returnForce;
        }

        /// <summary>
        /// <para>RepulsionForceByCollider</para>
        /// <para>�Ώۂ̗͂𕨎������l�������l�ɕϊ����܂�</para>
        /// </summary>
        /// <param name="physic">�Ώۂ̕���</param>
        /// <param name="otherPhysic">�Փˏ��</param>
        /// <returns>���ۂ̗�</returns>
        public static Vector3 ChangeForceByPhysicMaterials(PhysicData physic, OtherPhysicData otherPhysic)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //�ԋp�p
            Vector3 returnForce = physic.force;
            //���g�̕������������擾
            PhysicMaterials myMaterial = physic.colliderInfo.material;
            //�Փː�̕������������擾
            PhysicMaterials collisionMaterial = otherPhysic.collision;

            //Debug.Log(myMaterial.bounciness == collisionMaterial.bounciness);

            //�����������擾
            Vector3 vertical = VerticalDirectionBySurface(otherPhysic);
            //�����R�͂��Z�o
            Vector3 verticalResistance = GetTo.V3Projection(Gravity(physic), vertical);
            //�����ɓ����͂��擾
            Vector3 horizontalForce = HorizontalForceBySurface(otherPhysic.collision.transform, vertical, physic.force);
            //Debug.Log("f:" + returnForce + "s:" + (verticalResistance + horizontalForce));

            //�����C�W�����Z�o
            float combineDynamicDrug = GetTo.ValueCombine(myMaterial.dynamicDrug, collisionMaterial.dynamicDrug, myMaterial.drugCombine);
            //�Î~���C�W�����Z�o
            float combineStaticDrug = GetTo.ValueCombine(myMaterial.staticDrug, collisionMaterial.staticDrug, myMaterial.drugCombine);
            //Debug.Log("v:" + vertical + verticalResistance + " :h" + horizontalForce + " | " + physic.force);
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

        /// <summary>
        /// <para>AddDrug</para>
        /// <para>���C�͂��Z�o���܂�</para>
        /// </summary>
        /// <param name="horizontalForce">���������̗�</param>
        /// <param name="verticalResistance">�����R��</param>
        /// <param name="dynamicDrug">�����C�W��</param>
        /// <param name="staticDrug">�Î~���C�W��</param>
        /// <returns>���C��</returns>
        private static Vector3 AddDrug(Vector3 horizontalForce, Vector3 verticalResistance, float dynamicDrug, float staticDrug)
        {
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

            //�����͂ƕ����ɂ�����͂��Ȃ�
            if(repulsion == _vectorZero && force == _vectorZero)
            {
                return _vectorZero;
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
        /// <para>�傫�����ݒ肳�ꂽ�ꍇ�́A�����������X�P�[�����O</para>
        /// </summary>
        /// <param name="otherPhysic">�ՓˑΏۂ�Collider</param>
        /// <param name="scale">Collider�̑傫��</param>
        /// <returns>�ʂɑ΂��鐂������</returns>
        private static Vector3 VerticalDirectionBySurface(OtherPhysicData otherPhysic, Vector3 scale = default)
        {
            //�ȈՏՓ˒n�_���擾
            Vector3 collsionPoint = otherPhysic.collision.transform.InverseTransformPoint(otherPhysic.point);
            //Debug.Log(otherPhysic.collision.transform + ":" + otherPhysic.point);

            //�����֌W�Ȃ��Ɉ�ԍ��������𔻒肷��
            //�e�����̐�Βl���擾
            float norX = Mathf.Abs(collsionPoint.x);
            float norY = Mathf.Abs(collsionPoint.y);
            float norZ = Mathf.Abs(collsionPoint.z);
            //�e��Βl�̍ő�l���擾
            float maxNor = Mathf.Max(norX, norY, norZ);
            //Debug.Log(collsionPoint + "xyz" + norX + ":" + norY + ":" + norZ + " colname:" + collision.Collision.collider.name);


            Vector3 transformDire;
            float scaleRatio = 0;
            //X������ԍ���
            if(maxNor == norX)
            {
                //Scale���ݒ肳��Ă���
                if (scale != default)
                {
                    scaleRatio = scale.x;
                }

                //���̒l�ł���
                if (0 < collsionPoint.x)
                {
                    transformDire = otherPhysic.collision.transform.right;
                }
                //���̒l�ł���
                else
                {
                    transformDire = -otherPhysic.collision.transform.right;
                }
            }
            //Y������ԍ���
            else if(maxNor == norY)
            {
                //Scale���ݒ肳��Ă���
                if (scale != default)
                {
                    scaleRatio = scale.y;
                }

                //���̒l�ł���
                if (0 < collsionPoint.y)
                {
                    transformDire = otherPhysic.collision.transform.up;
                }
                //���̒l�ł���
                else
                {
                    transformDire = -otherPhysic.collision.transform.up;
                }
            }
            //Z������ԍ���
            else
            {
                //Scale���ݒ肳��Ă���
                if (scale != default)
                {
                    scaleRatio = scale.z;
                }

                //���̒l�ł���
                if (0 < collsionPoint.z)
                {
                    transformDire = otherPhysic.collision.transform.forward;
                }
                //���̒l�ł���
                else
                {
                    transformDire = -otherPhysic.collision.transform.forward;
                }
            }

            //Scale�{�����ݒ肳��Ă���
            if(scaleRatio != 0)
            {
                //Scale�{���̐�Βl�̔���������x�N�g���ɂ������l �� ���S����ʂ܂ł̐����ȋ����x�N�g�� �ƂȂ�
                transformDire *= (Mathf.Abs(scaleRatio) / GetTo.Half);
            }
            //�ԋp
            return transformDire;
        }

        /// <summary>
        /// <para>HorizontalForceBySurface</para>
        /// <para>�ʂɑ΂��鐅���������擾���܂�</para>
        /// </summary>
        /// <param name="surface">�ʕ������擾�ł���Transform</param>
        /// <param name="vertical">��������</param>
        /// <returns>���������ȊO�̕����̘a</returns>
        private static Vector3 HorizontalForceBySurface(Transform surface, Vector3 vertical, Vector3 force)
        {
            //�ԋp�p
            Vector3 sumHorizontal = _vectorZero;

            //�����������㉺�ł���
            if (vertical == surface.up || vertical == -surface.up)
            {
                sumHorizontal += GetTo.V3Projection(force,surface.right);
                sumHorizontal += GetTo.V3Projection(force, surface.forward);
                return sumHorizontal;
            }
            //�������������E�ł���
            else if (vertical == surface.right || vertical == -surface.right)
            {
                sumHorizontal += GetTo.V3Projection(force, surface.up);
                sumHorizontal += GetTo.V3Projection(force, surface.forward);
                return sumHorizontal;
            }
            //�����������O��ł���
            else
            {
                sumHorizontal += GetTo.V3Projection(force, surface.right);
                sumHorizontal += GetTo.V3Projection(force, surface.up);
                return sumHorizontal;
            }
        }

        #endregion 
    }
}
