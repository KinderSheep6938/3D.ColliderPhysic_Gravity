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
        _transform = transform;
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        
    }




    #endregion
}
