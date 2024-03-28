/// -----------------------------------------------------------------
/// DeathObj.cs
/// 
/// �쐬���F2023/12/18
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathObj : MonoBehaviour
{
    #region �ϐ�
    //��x��������
    private bool _isPlay = false;
    
    //���g��Collider
    private OriginalCollider _collider = default;
    //MainSystem�̃��g���C�@�\
    private IRetryble _retrySct = default;
    //���ʉ��Đ��N���X
    private SoundPlayer _se = default;
    
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
        _collider = GetComponent<OriginalCollider>();
        _se = GetComponent<SoundPlayer>();
        
        //���C���V�X�e�������݂��邩
        if (!FindObjectOfType<MainSystem>())
        {
            return;
        }
        _retrySct = FindObjectOfType<MainSystem>().GetComponent<IRetryble>();
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //���S����
        CheckDeath();
    }

    /// <summary>
    /// <para>CheckDeath</para>
    /// <para>���S���肩�ǂ����m���߂܂�</para>
    /// </summary>
    private void CheckDeath()
    {
        //���ɏ������s���Ă���
        if (_isPlay)
        {
            return;
        }

        //�Փ˔��肪���� ���� ��x���������s���Ă��Ȃ�
        if (_collider.Collision && !_isPlay)
        {
            //�V�[���ǂݍ���
            _retrySct.StageRetry();
            _isPlay = true;
            _se.Play();
        }

    }
    #endregion
}