/// -----------------------------------------------------------------
/// PlayerInput.cs �v���C���[���͊Ǘ�
/// 
/// �쐬���F2023/11/28
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCtrl : MonoBehaviour
{
    #region �ϐ�
    //�ړ����̓t���O
    private bool _isMove = false;

    //�v���C���[����
    private Player _player = default;
    //InputSystem
    private InputAction _move = default;
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
        _player = GetComponent<Player>();
        _move = GetComponent<PlayerInput>().actions["Move"];
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //�v���C���[�������擾���Ă��Ȃ�
        if (!_player)
        {
            return;
        }

        //�o�͏���
        Output();
    }

    /// <summary>
    /// <para>Output</para>
    /// <para>���͂ɑ΂��āA�e�X�o�͂��܂�</para>
    /// </summary>
    private void Output()
    {
        if (!_isMove)
        {
            return;
        }

        //���͎擾
        Vector2 input = _move.ReadValue<Vector2>();
        //���͔��f
        _player.Move(input);
    }

    /// <summary>
    /// <para>OnMove</para>
    /// <para>�ړ����͂��s�����ۂɏ������܂�</para>
    /// </summary>
    /// <param name="context">���͏��</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //���͂��J�n����
        if (context.performed)
        {
            _isMove = true;
        }

        //���͂��I������
        if (context.canceled)
        {
            _isMove = false;
        }
    }

    /// <summary>
    /// <para>OnJump</para>
    /// <para>�������͂��s�����ۂɏ������܂�</para>
    /// </summary>
    /// <param name="context">���͏��</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        //�{�^���������ꂽ��
        if (context.performed)
        {
            //�v���C���[����
            _player.ChangeGravity();
        }
    }
    #endregion
}
