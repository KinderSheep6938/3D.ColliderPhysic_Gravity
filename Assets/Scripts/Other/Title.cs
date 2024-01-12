/// -----------------------------------------------------------------
/// Title.cs
/// 
/// �쐬���F2023/01/12
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Title : MonoBehaviour
{
    #region �ϐ�
    //���C���V�X�e��
    private MainSystem _mSystem = default;
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //������
        _mSystem = FindObjectOfType<MainSystem>();
    }

    /// <summary>
    /// <para>OnStart</para>
    /// <para>�X�^�[�g���͂��s�����ۂɏ������܂�</para>
    /// </summary>
    /// <param name="context">���͏��</param>
    public void OnStart(InputAction.CallbackContext context)
    {
        //�{�^���������ꂽ��
        if (context.performed)
        {
            //�X�e�[�W�ֈڍs
            _mSystem.NextStage();
        }
    }
    #endregion
}
