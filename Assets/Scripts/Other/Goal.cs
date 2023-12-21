/// -----------------------------------------------------------------
/// Goal.cs
/// 
/// �쐬���F2023/12/19
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Goal : MonoBehaviour
{
    #region �ϐ�
    //��x��������
    private bool _isOnce = default;

    //���C���V�X�e��
    private MainSystem _mSystem = default;
    //�v���C���[
    private Player _player = default;
    //���g��Collider
    private OriginalCollider _collider = default;
    //�N���A�e�L�X�g
    private ViewObj _clearTextsObj = default;

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
        _mSystem = FindObjectOfType<MainSystem>();
        _player = FindObjectOfType<Player>();
        _collider = GetComponent<OriginalCollider>();
        _clearTextsObj = GameObject.FindGameObjectWithTag("ClearText").GetComponent<ViewObj>();
        _clearTextsObj.SetView(false);
    }

    /// <summary>
    /// �X�V�O����
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //�S�[������
        CheckGoal();
    }

    /// <summary>
    /// <para>CheckGoal</para>
    /// <para>�S�[���������ǂ����������܂�</para>
    /// </summary>
    private void CheckGoal()
    {
        //���ɏ������s���Ă���
        if (_isOnce)
        {
            return;
        }

        //�Փ˔��肪���邩 ���� ��x���������s���Ă��Ȃ�
        if (_collider.Collision && !_isOnce)
        {
            //�N���A�e�L�X�g�\��
            _clearTextsObj.SetView(true);
            //�v���C���[�̏������~
            _player.SetStopInput = false;
            _isOnce = true;
        }
    }

    /// <summary>
    /// <para>OnNextStage</para>
    /// <para>���̃X�e�[�W�֐i�s���܂�</para>
    /// </summary>
    public void OnNextStage(InputAction.CallbackContext context)
    {
        //�{�^���������ꂽ ���� ���ɃS�[�����菈�����s���Ă���
        if (context.performed && _isOnce)
        {
            //���̃V�[����
            _mSystem.NextStage();
        }
    }
    #endregion
}
