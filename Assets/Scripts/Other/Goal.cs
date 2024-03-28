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
    //�e�e�L�X�g�̃I�u�W�F�N�g�^�O
    private const string CLEARTEXT_OBJECTTAG = "ClearText";
    private const string MANUALTEXT_OBJECTTAG = "ManualText";

    //��x��������
    private bool _isPlay = default;

    //���C���V�X�e��
    private MainSystem _mSystem = default;
    //�v���C���[
    private Player _player = default;
    //���g��Collider
    private OriginalCollider _collider = default;
    //�N���A�e�L�X�g
    private ViewObj _clearTextsObj = default;
    //������@�e�L�X�g
    private ViewObj _manualTextsObj = default;
    //���ʉ��Đ��N���X
    private SoundPlayer _se = default;
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
        _se = GetComponent<SoundPlayer>();
        _clearTextsObj = GameObject.FindGameObjectWithTag(CLEARTEXT_OBJECTTAG).GetComponent<ViewObj>();
        _manualTextsObj = GameObject.FindGameObjectWithTag(MANUALTEXT_OBJECTTAG).GetComponent<ViewObj>();
        _clearTextsObj.SetView(false);
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
        if (_isPlay)
        {
            return;
        }

        //�Փ˔��肪���邩 ���� ��x���������s���Ă��Ȃ�
        if (_collider.Collision && !_isPlay)
        {
            //�e�L�X�g�\������
            _clearTextsObj.SetView(true);
            _manualTextsObj.SetView(false);
            //�v���C���[�̏������~
            _player.SetStopInput = false;
            _isPlay = true;
            _se.Play();
        }
    }

    /// <summary>
    /// <para>OnNextStage</para>
    /// <para>���̃X�e�[�W�֐i�s���܂�</para>
    /// </summary>
    public void OnNextStage(InputAction.CallbackContext context)
    {
        //�{�^���������ꂽ ���� ���ɃS�[�����菈�����s���Ă���
        if (context.performed && _isPlay)
        {
            //���̃V�[����
            _mSystem.NextStage();
            //�e�L�X�g�\������
            _clearTextsObj.SetView(false);
            _manualTextsObj.SetView(true);
        }
    }
    #endregion
}
