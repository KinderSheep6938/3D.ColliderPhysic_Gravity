/// -----------------------------------------------------------------
/// MoveObj.cs
/// 
/// �쐬���F2023/12/20
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObj : MonoBehaviour
{
    #region �ϐ�
    //�ړ��x�N�g��
    private Vector3 _moveValue = default;

    //���g��Transform
    private Transform _transform = default;
    #endregion

    #region �v���p�e�B
    //�ړ��x�N�g���̐ݒ�
    public Vector3 Speed { set => _moveValue = value; }
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //������
        _transform = transform;
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
        //�ړ�����
        Move();
    }

    /// <summary>
    /// <para>Move</para>
    /// <para>���̂��ړ������܂�</para>
    /// </summary>
    private void Move()
    {
        _transform.Translate(_moveValue * Time.deltaTime);
    }
    #endregion
}
