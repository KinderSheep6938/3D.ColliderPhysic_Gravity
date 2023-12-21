/// -----------------------------------------------------------------
/// ColliderDataManager.cs Collider���Ǘ�
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.DataManager
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>ColliderDataManager</para>
    /// <para>�����蔻��f�[�^���Ǘ����܂�</para>
    /// </summary>
    public class ColliderDataManager
    {
        #region �ϐ�
        //Collider��񋤗L�p
        private static List<ColliderData> _collidersInWorld = new();
        #endregion

        #region �v���p�e�B

        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>SetColliderToWorld</para>
        /// <para>�Ώۂ�Collider�������L���X�g�ɐݒ肵�܂�</para>
        /// </summary>
        /// <param name="target">Collider���</param>
        public static void SetColliderToWorld(ref ColliderData target)
        {
            //���Ɋi�[����Ă��邩
            if (_collidersInWorld.Contains(target))
            {
                //���ɂ���f�[�^���폜
                RemoveColliderToWorld(target);
            }

            //�i�[
            _collidersInWorld.Add(target);

        }

        /// <summary>
        /// <para>RemoveColliderToWorld</para>
        /// <para>�Ώۂ�Collider�������L���X�g����폜���܂�</para>
        /// </summary>
        /// <param name="target">Collider���</param>
        public static void RemoveColliderToWorld(ColliderData target)
        {
            //���L���X�g����폜
            _collidersInWorld.Remove(target);
        }

        /// <summary>
        /// <para>GetColliderToWorld</para>
        /// <para>�g�p�\�ȕۑ�����Ă���SCollider��ԋp���܂�</para>
        /// </summary>
        /// <returns>�SCollider</returns>
        public static ref List<ColliderData> GetColliderToWorld()
        {
            //�]���ȃf�[�^���폜����
            for(int i = 0;i < _collidersInWorld.Count;i++)
            { 
                //�����Ώۂ��폜����Ă���
                if(_collidersInWorld[i].physic.transform == default)
                {
                    //�폜����
                    RemoveColliderToWorld(_collidersInWorld[i]);
                    i--;
                }
            }

            //�ԋp
            return ref _collidersInWorld;
        }
        #endregion
    }
}

