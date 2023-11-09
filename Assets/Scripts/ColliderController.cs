/// -----------------------------------------------------------------
/// ColliderEditor.cs
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OriginalColliderEditor;

public class ColliderController : MonoBehaviour
{
    #region �ϐ�

    private Transform _transform = default;
    [SerializeField]
    private ColliderData _myCol = new ColliderData();


    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //������
        _transform = transform;
        _transform.hasChanged = false;

        //�R���C�_�[����
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);
    }
    
    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //Collider���̍X�V�m�F
        CheckColliderUpdata();

        //�`��
        //Drow();
    }

    /// <summary>
    /// <para>CheckColliderUpdata</para>
    /// <para>Collider�����X�V����K�v�����邩�������܂�</para>
    /// <para>�܂��K�v���������ꍇ�́A�X�V�������s���܂�</para>
    /// </summary>
    private void CheckColliderUpdata()
    {
        //Transform��񂪕ς����
        if (_transform.hasChanged)
        {
            //����Transform�Ɋ�Â���Collider���쐬����
            _myCol = ColliderEditor.SetColliderDataByCube(_transform);
            //�ύX�t���O��������
            _transform.hasChanged = false;
        }
    }

    /// <summary>
    /// <para>Drow</para>
    /// <para>Collider������ʂɕ`�悵�܂�</para>
    /// </summary>
    private void Drow()
    {
        //�`��
        ColliderEditor.DrowCollider(_myCol);
    }





    #endregion
}
