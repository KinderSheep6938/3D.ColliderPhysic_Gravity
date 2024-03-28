/// -----------------------------------------------------------------
/// DeathCollider.cs
/// 
/// �쐬���F2023/12/15
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBorder : MonoBehaviour
{
    #region �ϐ�
    //��b�x�N�g��
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorUp = Vector3.up;

    //��x��������
    private bool _isPlay = false;

    //�ō����x�A�ŏ����x
    [SerializeField]
    private float _maxHeight = 50f;
    [SerializeField]
    private float _minHeight = -50f;

    //�v���C���[
    private Transform _player = default;
    //MainSystem�̃��g���C�@�\
    private IRetryble _retrySct = default;
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
        _player = FindObjectOfType<Player>().transform;
        _se = GetComponent<SoundPlayer>();

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
        CheckDeath();
    }

    /// <summary>
    /// <para>CheckDeath</para>
    /// <para>���S����ɏՓ˂������������܂�</para>
    /// </summary>
    private void CheckDeath()
    {
        //���ɏ������s���Ă���
        if (_isPlay)
        {
            return;
        }

        //���S�̃��C���𒴂��Ă���
        if (_maxHeight < _player.position.y || _player.position.y < _minHeight)
        {
            //�V�[���ǂݍ���
            _retrySct.StageRetry();
            _isPlay = true;
            _se.Play();
        }
    }

    /// <summary>
    /// �`�揈��
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmo�`��
        //���x�͂O�̌��݈ʒu���擾
        Vector3 nowPos = transform.position - _vectorUp * transform.position.y;

        //�F�ݒ�
        Gizmos.color = Color.red;
        //�`��
        Gizmos.DrawLine(nowPos + _vectorUp * _maxHeight,nowPos + _vectorUp * _minHeight);
        Gizmos.DrawSphere(nowPos + _vectorUp * _maxHeight, 0.5f);
        Gizmos.DrawSphere(nowPos + _vectorUp * _minHeight, 0.5f);
        //���ݍ��x�F
        Gizmos.color = Color.red;
        //�v���C���[�̌��ݍ��x
        if(_player != default)
        {
            Gizmos.DrawSphere(_vectorUp * _player.position.y + nowPos - _vectorUp * nowPos.y, 1);
        }
    }
    #endregion
}
