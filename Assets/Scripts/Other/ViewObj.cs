/// -----------------------------------------------------------------
/// ViewObj.cs
/// 
/// �쐬���F2023/12/21
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObj : MonoBehaviour
{
    #region �ϐ�
    //���g��Transform
    private Transform _transform = default;
    //���g�̎q���̐�
    private int _childCnt = 0;
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //������
        _transform = transform;
        _childCnt = _transform.childCount;
    }

    /// <summary>
    /// <para>SetView</para>
    /// <para>�I�u�W�F�N�g�̕\����ݒ肵�܂�</para>
    /// </summary>
    /// <param name="active"></param>
    public void SetView(bool active)
    {
        //�q���̐����A�؂�ւ���
        for(int i = 0; i < _childCnt; i++)
        {
            //�q���̏�Ԃ�؂�ւ�
            _transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    #endregion
}
