/// -----------------------------------------------------------------
/// ColliderEngine.cs�@Collider���萧��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderManager
{
    #region �ϐ�
    //�񕪊��p�萔
    private const int HALF = 2;
    //Collider��񋤗L�p
    private static List<ColliderController> _worldInColliders = new();
    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    /// <summary>
    /// <para>SetColliderToWorld</para>
    /// <para>�Ώۂ�Collider�������L���X�g�ɐݒ肵�܂�</para>
    /// </summary>
    /// <param name="target">Collider���</param>
    public static void SetColliderToWorld(ColliderController target)
    {
        //���Ɋi�[����Ă��邩
        if (_worldInColliders.Contains(target))
        {
            //�i�[�����I��
            return;
        }

        //�i�[
        _worldInColliders.Add(target);
    }

    /// <summary>
    /// <para>RemoveColliderToWorld</para>
    /// <para>�Ώۂ�Collider�������L���X�g����폜���܂�</para>
    /// </summary>
    /// <param name="target">Collider���</param>
    public static void RemoveColliderToWorld(ColliderController target)
    {
        //���L���X�g����폜
        _worldInColliders.Remove(target);
    }

    public static bool CheckCollisionByCollider(ColliderController collider)
    {
        Vector3 checkColliderSize;
        Vector3 localPos;
        //���L���X�g����SCollider�����擾���A�Փˌ������s���܂�
        foreach(ColliderController target in _worldInColliders)
        {
            //�����Ώۂ����g�ł���
            if(target == collider)
            {
                //�������Ȃ�
                continue;
            }

            //�����J�n
            //�����Ώۂ̑傫�����擾
            checkColliderSize = target.Data.localScale / HALF;
            //�����Ώۂ̒��_���W��
            localPos = target.transform.InverseTransformPoint(collider.Data.position);
        }

        return false;
    }
    #endregion
}
