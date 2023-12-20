/// -----------------------------------------------------------------
/// MoveObj.cs
/// 
/// 作成日：2023/12/20
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObj : MonoBehaviour
{
    #region 変数
    //移動ベクトル
    private Vector3 _moveValue = default;

    //自身のTransform
    private Transform _transform = default;
    #endregion

    #region プロパティ
    //移動ベクトルの設定
    public Vector3 Speed { set => _moveValue = value; }
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _transform = transform;
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //移動処理
        Move();
    }

    /// <summary>
    /// <para>Move</para>
    /// <para>物体を移動させます</para>
    /// </summary>
    private void Move()
    {
        _transform.Translate(_moveValue * Time.deltaTime);
    }
    #endregion
}
