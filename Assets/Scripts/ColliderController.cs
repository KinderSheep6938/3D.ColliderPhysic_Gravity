/// -----------------------------------------------------------------
/// ColliderEditor.cs
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OriginalColliderEditor;

public class ColliderController : MonoBehaviour
{
    #region 変数

    private Transform _transform = default;
    [SerializeField]
    private ColliderData _myCol = new ColliderData();


    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _transform = transform;
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        
    }




    #endregion
}
