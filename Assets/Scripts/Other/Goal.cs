/// -----------------------------------------------------------------
/// Goal.cs
/// 
/// �쐬���F2023/12/19
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    #region �ϐ�
    //���g��Collider
    private OriginalCollider _collider = default;
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

    }

    /// <summary>
    /// <para>CheckGoal</para>
    /// <para>�S�[���������ǂ����������܂�</para>
    /// </summary>
    private void CheckGoal()
    {
        //�Փ˔��肪���邩
        if (_collider.Collision)
        {

        }
    }
    #endregion
}
