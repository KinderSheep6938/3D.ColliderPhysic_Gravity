/// -----------------------------------------------------------------
/// CollisionPhysicManager.cs
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary.CollisionPhysic
{
    using System.Collections.Generic;
    using UnityEngine;
    using PhysicLibrary.Material;

    #region �f�[�^�Ǘ��p
    public enum PhysicThis
    {
        A,      //PhysicA
        B,      //PhysicB
        None    //�ǂ���ł��Ȃ�
    }
    #endregion

    #region �Փ˃f�[�^
    [System.Serializable]
    public struct PhysicCollision
    {
        public readonly PhysicMaterials physicA;  //A�̍ގ��f�[�^
        public readonly PhysicMaterials physicB;  //B�̍ގ��f�[�^
        public readonly Vector3 point;            //�Փ˒n�_
        public readonly Vector3 velocity;         //�⊮���x

        /// <summary>
        /// <para>PhysicCollision</para>
        /// <para>�ՓˊǗ��p�f�[�^</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="point">�Փ˒n�_</param>
        /// <param name="velocity">�⊮���x</param>
        public PhysicCollision(PhysicMaterials a, PhysicMaterials b, Vector3 point, Vector3 velocity)
        {
            this.physicA = a;
            this.physicB = b;
            this.point = point;
            this.velocity = velocity;
        }

        /// <summary>
        /// <para>Contains</para>
        /// <para>�����Ώۂ��܂܂�邩�A�Ώ��ς݂ł͂Ȃ����m�F���܂�</para>
        /// </summary>
        /// <param name="search">�����Ώ�</param>
        /// <returns>��������</returns>
        public bool Contains(PhysicMaterials search)
        {
            //�ǂ��炩�ɂ͑��݂���
            if(Which(search) != PhysicThis.None)
            {
                return true;
            }
            //�ǂ���ł��Ȃ�
            return false;
        }

        /// <summary>
        /// <para>Which</para>
        /// <para>�����Ώۂ��ǂ�����Physic�ł��邩�Ԃ��܂�</para>
        /// <para>�ǂ���ł��Ȃ����́ANone��Ԃ��܂�</para>
        /// </summary>
        /// <param name="search">�����Ώ�</param>
        /// <returns>��������</returns>
        public PhysicThis Which(PhysicMaterials search)
        {
            //PhysicA�ł���
            if (physicA.transform == search.transform)
            {
                return PhysicThis.A;
            }
            //PhysicB�ł���
            else if (physicB.transform == search.transform)
            {
                return PhysicThis.B;
            }
            //�ǂ���ł��Ȃ�
            return PhysicThis.None;
        }

        /// <summary>
        /// <para>OtherPhysic</para>
        /// <para>�����Ώۂƕʂ�PhysicMaterials��Ԃ��܂�</para>
        /// <para>���݂��Ȃ��ꍇ�́APhysicMaterials�̃f�t�H���g�l���ԋp����܂�</para>
        /// </summary>
        /// <param name="search">�����Ώ�</param>
        /// <returns>�����Е���PhysicMaterials</returns>
        public PhysicMaterials OtherPhysic(PhysicMaterials search)
        {
            //���݂��Ȃ�
            if (!Contains(search))
            {
                return default;
            }

            //PhysicA�ł���
            if(physicA.transform == search.transform)
            {
                return physicB;
            }
            //PhysicB�ł���
            return physicA;
        }
    }
    #endregion

    #region ���������g�p�f�[�^
    public struct OtherPhysicData
    {
        public readonly PhysicMaterials collision;  //����̍ގ��f�[�^
        public readonly Vector3 point;              //�Փ˒n�_
        public readonly Vector3 velocity;           //�⊮���x

        /// <summary>
        /// <para>UsePhysucData</para>
        /// <para>���������ɕK�v�ȃf�[�^</para>
        /// </summary>
        /// <param name="collision">����̍ގ�</param>
        /// <param name="point">�Փ˒n�_</param>
        /// <param name="velocity">�⊮���x</param>
        public OtherPhysicData(PhysicMaterials collision,Vector3 point,Vector3 velocity)
        {
            this.collision = collision;
            this.point = point;
            this.velocity = velocity;
        }
    }
    #endregion

    /// <summary>
    /// <para>CollisionPhysicManager</para>
    /// <para>�Փ˔��肪�������������Ǘ����܂�</para>
    /// </summary>
    public class CollisionPhysicManager
    {
        #region �ϐ�
        //�Փ˃f�[�^�Ǘ����X�g
        [SerializeField]private static List<PhysicCollision> _collisionData = new();
        private static List<bool> _physicAChecks = new();
        private static List<bool> _physicBChecks = new();
        //�Փ˃f�[�^�̊m�F�҂��������X�g
        private static List<PhysicMaterials> _physicLogs = new();
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>SetCollision</para>
        /// <para>�Փ˂��������������f�[�^�Ƃ��ēo�^���܂�</para>
        /// </summary>
        /// <param name="physicA"></param>
        /// <param name="physicB"></param>
        /// <param name="point">�Փ˒n�_</param>
        public static void SetCollision(PhysicMaterials physicA, PhysicMaterials physicB, Vector3 point, Vector3 velocity)
        {
            //�Փ˂��Ă��镨���������ł���
            if(physicA.transform == physicB.transform)
            {
                //��f�[�^�ł���\���̂��ߏ������Ȃ�
                return;
            }

            //�L�^����Ă���Փ˂��m�F
            foreach (PhysicCollision collision in _collisionData)
            {
                //���ɂ���Փ˂�
                if (collision.Contains(physicA) && collision.Contains(physicB))
                {
                    //�i�[�����I������
                    return;
                }
            }

            //PhysicA���o�^���O�ɋL�^����Ă��Ȃ�
            if (!_physicLogs.Contains(physicA))
            {
                //PhysicA���L�^
                _physicLogs.Add(physicA);
            }
            //PhysicB���o�^���O�ɋL�^����Ă��Ȃ�
            if (!_physicLogs.Contains(physicB))
            {
                //PhysicB���L�^
                _physicLogs.Add(physicB);
            }

            //�f�[�^�ǉ�
            _collisionData.Add(new PhysicCollision(physicA, physicB, point, velocity));
            //�Ώۂ̕�����Rigidbody�����Ă��Ȃ��ꍇ
            // -> ����ɍl������K�v�͂Ȃ��̂Ŋm�F�ς݂Ɣ��肷��
            _physicAChecks.Add(!physicA.rigid);
            _physicBChecks.Add(!physicB.rigid);
        }

        /// <summary>
        /// <para>CheckRemoveCollision</para>
        /// <para>�����Ώۂ̏Փ˃f�[�^���폜�Ώۂ��������܂�</para>
        /// <para>�܂��폜�Ώۂ̏ꍇ�́A�폜���܂�</para>
        /// </summary>
        /// <param name="remove">�����Ώ�</param>
        private static void CheckRemoveCollision(PhysicCollision remove)
        {
            //�v�f�ԍ��擾
            int listIndex = _collisionData.IndexOf(remove);

            //�܂������Ƃ��m�F�ς݂ł͂Ȃ�
            if (!_physicAChecks[listIndex] || !_physicBChecks[listIndex])
            {
                //�폜���Ȃ�
                return;
            }

            //�f�[�^�폜
            _collisionData.Remove(remove);
            _physicAChecks.RemoveAt(listIndex);
            _physicBChecks.RemoveAt(listIndex);
        }

        /// <summary>
        /// <para>CheckRemoveLog</para>
        /// <para>�����Ώۂ̕������폜�Ώۂ��������܂�</para>
        /// <para>�܂��폜�Ώۂ̏ꍇ�́A�폜���܂�</para>
        /// </summary>
        /// <param name="remove"></param>
        private static void CheckRemoveLog(PhysicMaterials remove)
        {
            //�Փ˃f�[�^���m�F
            foreach (PhysicCollision collision in _collisionData)
            {
                //�֗^���Ă���Փ˃f�[�^�ł��� ���� �܂��m�F�ς݂ł͂Ȃ�
                if (collision.Contains(remove) && !PhysicToCheckLog(collision, remove))
                {
                    //�폜����
                    return;
                }
            }
            //�L�^�폜
            _physicLogs.Remove(remove);
        }

        /// <summary>
        /// <para>PhysicToCheckLog</para>
        /// <para>�����Ώۂ��Ώۂ̏Փ˃f�[�^���m�F�ς݂ł��邩�Ԃ��܂�</para>
        /// </summary>
        /// <param name="check">�Ώۂ̏Փ˃f�[�^</param>
        /// <param name="physic">�����Ώ�</param>
        /// <returns>�m�F�ςݔ���</returns>
        private static bool PhysicToCheckLog(PhysicCollision check, PhysicMaterials physic)
        {
            //�v�f�ԍ��擾
            int listIndex = _collisionData.IndexOf(check);
            //�ǂ����Physic���擾
            PhysicThis ans = check.Which(physic);
            
            //PhysicA�ł��� ���� �܂��m�F�ς݂ł͂Ȃ�
            if(ans == PhysicThis.A && !_physicAChecks[listIndex])
            {
                return false;
            }
            //PhysicB�ł��� ���� �܂��m�F�ς݂ł͂Ȃ�
            else if (ans == PhysicThis.B && !_physicBChecks[listIndex])
            {
                return false;
            }
            //�m�F�ς݂ł͂���
            return true;
        }

        /// <summary>
        /// <para>CheckIn</para>
        /// <para>�Ώۂ̏Փ˃f�[�^�ɑΉ������m�F�ςݔ����ݒ肵�܂�</para>
        /// </summary>
        /// <param name="target">�Ώۂ̏Փ˃f�[�^</param>
        /// <param name="physic">�Ή�����Physic</param>
        private static void CheckIn(PhysicCollision target, PhysicThis physic)
        {
            //�v�f�ԍ��擾
            int listIndex = _collisionData.IndexOf(target);

            //PhysicA�ł���
            if (physic == PhysicThis.A)
            {
                _physicAChecks[listIndex] = true;
            }
            //PhysicB�ł���
            else
            {
                _physicBChecks[listIndex] = true;
            }
        }
        
        /// <summary>
        /// <para>GetCollision</para>
        /// <para>�����Ώۂɑ΂��đΉ����镨�������f�[�^��Ԃ��܂�</para>
        /// <para>�Ή�����f�[�^���Ȃ��ꍇ�́Adefault��Ԃ��܂�</para>
        /// </summary>
        /// <param name="search">�����Ώ�</param>
        /// <returns>���������f�[�^</returns>
        public static OtherPhysicData GetCollision(PhysicMaterials search)
        {
            Debug.Log(_collisionData.Count + ":" + _physicLogs.Count);

            //�L�^����Ă���Փ˂��m�F
            foreach(PhysicCollision collision in _collisionData)
            {
                //�֗^���Ă���Փ˃f�[�^�ł��� ���� �܂��m�F�ς݂ł͂Ȃ�
                if (collision.Contains(search) && !PhysicToCheckLog(collision,search))
                {
                    //�g�p����f�[�^���܂Ƃ߂�
                    OtherPhysicData returnData = new OtherPhysicData(collision.OtherPhysic(search), collision.point, collision.velocity);
                    //�m�F�ς݂Ɛݒ肷��
                    CheckIn(collision, collision.Which(search));
                    //�Փ˃f�[�^���폜����
                    CheckRemoveCollision(collision);
                    //�m�F�҂��L�^���폜����
                    CheckRemoveLog(search);
                    return returnData;
                }
                //�Փ˃f�[�^���폜����
                CheckRemoveCollision(collision);
            }
            //�ՓˋL�^���폜����
            CheckRemoveLog(search);
            //������Ȃ�
            return default;
        }

        /// <summary>
        /// <para>CheckWaitContains</para>
        /// <para>�Ώۂ̕������m�F�҂��ɓo�^����Ă��Ȃ����`�F�b�N���܂�</para>
        /// </summary>
        /// <param name="target">�Ώۂ̕���</param>
        /// <returns>�o�^����</returns>
        public static bool CheckWaitContains(PhysicMaterials target)
        {
            //�m�F�҂������Ȃ� �܂��� �o�^����Ă��Ȃ�
            if (_physicLogs.Count == 0 || !_physicLogs.Contains(target))
            {
                //�o�^����Ă��Ȃ�
                return false;
            }

            //�Ώۂ̕�����Rigidbody���t�^����Ă��Ȃ�
            if (!target.rigid)
            {
                //�m�F�҂������������
                _physicLogs.Remove(target);
            }
            //�o�^����Ă���
            return true;
        }
        
        #endregion
    }
}
