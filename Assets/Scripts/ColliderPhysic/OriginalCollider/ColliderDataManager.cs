/// -----------------------------------------------------------------
/// ColliderDataManager.cs Collider���Ǘ�
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.DataManager
{
    using System.Collections.Generic;

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
        public static ref List<ColliderData> ColliderInWorld { get => ref _collidersInWorld; }
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>SetColliderToWorld</para>
        /// <para>�Ώۂ�Collider�������L���X�g�ɐݒ肵�܂�</para>
        /// </summary>
        /// <param name="target">Collider���</param>
        public static void SetColliderToWorld(ColliderData target)
        {
            //���Ɋi�[����Ă��邩
            if (_collidersInWorld.Contains(target))
            {
                //�i�[�����I��
                return;
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
        #endregion
    }
}

