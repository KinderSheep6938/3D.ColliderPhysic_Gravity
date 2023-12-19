/// -----------------------------------------------------------------
/// Goal.cs
/// 
/// 作成日：2023/12/19
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    #region 変数
    //自身のCollider
    private OriginalCollider _collider = default;
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
        _collider = GetComponent<OriginalCollider>();
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

    }

    /// <summary>
    /// <para>CheckGoal</para>
    /// <para>ゴールしたかどうか検査します</para>
    /// </summary>
    private void CheckGoal()
    {
        //衝突判定があるか
        if (_collider.Collision)
        {

        }
    }
    #endregion
}
