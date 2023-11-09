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
        //初期化
        _transform = transform;
        _transform.hasChanged = false;

        //コライダー生成
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);
    }
    
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //Collider情報の更新確認
        CheckColliderUpdata();

        //描画
        //Drow();
    }

    /// <summary>
    /// <para>CheckColliderUpdata</para>
    /// <para>Collider情報を更新する必要があるか検査します</para>
    /// <para>また必要があった場合は、更新処理を行います</para>
    /// </summary>
    private void CheckColliderUpdata()
    {
        //Transform情報が変わった
        if (_transform.hasChanged)
        {
            //そのTransformに基づいてColliderを作成する
            _myCol = ColliderEditor.SetColliderDataByCube(_transform);
            //変更フラグを初期化
            _transform.hasChanged = false;
        }
    }

    /// <summary>
    /// <para>Drow</para>
    /// <para>Collider情報を画面に描画します</para>
    /// </summary>
    private void Drow()
    {
        //描画
        ColliderEditor.DrowCollider(_myCol);
    }





    #endregion
}
