/// -----------------------------------------------------------------
/// PhysicDataManager.cs Physic���Ǘ�
/// 
/// �쐬���F2023/11/27
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary.DataManager
{
    using System.Collections.Generic;
    using PhysicLibrary.Material;

    /// <summary>
    /// <para>PhysicDataManager</para>
    /// <para>�����f�[�^���Ǘ����܂�</para>
    /// </summary>
    public class PhysicDataManager
    {
        #region �ϐ�
        //Physic��񋤗L�p
        private static List<PhysicMaterials> _physicsInWorld = new();
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>SetPhysicToWorld</para>
        /// <para>�Ώۂ�Physic�������L���X�g�ɐݒ肵�܂�</para>
        /// </summary>
        /// <param name="target">�ǉ��\���Physic���</param>
        public static void SetPhysicToWorld(PhysicMaterials target)
        {
            //���Ɋi�[����Ă��邩
            if (_physicsInWorld.Contains(target))
            {
                //�i�[�����I��
                return;
            }

            //�i�[
            _physicsInWorld.Add(target);
        }

        /// <summary>
        /// <para>RemovePhysicToWorld</para>
        /// <para>�Ώۂ�Physic�������L���X�g����폜���܂�</para>
        /// </summary>
        /// <param name="target">�폜�Ώۂ�Physic���</param>
        public static void RemovePhysicToWorld(PhysicMaterials target)
        {
            //���L���X�g����폜
            _physicsInWorld.Remove(target);
        }

        /// <summary>
        /// <para>SearchPhysicByCollider</para>
        /// <para>���L���X�g����Փˏ�����ɍ��v����Physic�����������܂�</para>
        /// <para>������Ȃ��ꍇ�́A���g��Physic����Ԃ��܂�</para>
        /// </summary>
        /// <param name="myData">���g��Physic���</param>
        /// <returns>���v����Physic���</returns>
        public static PhysicMaterials SearchPhysicByCollider(PhysicData myData)
        {
            //���L���X�g���猟��
            foreach (PhysicMaterials search in _physicsInWorld)
            {
                //Debug.Log(search.transform + ":" + myData.colliderInfo.Collision.collider + "a:" + (search.transform == myData.colliderInfo.Collision.collider));
                

            }

            //������Ȃ��ꍇ�͌v�Z�����ȗ����̂��߁A���g��Physic����ԋp����
            return myData.colliderInfo.Material;
        }
        #endregion

    }
}
