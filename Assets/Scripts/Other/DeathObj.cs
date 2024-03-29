/// -----------------------------------------------------------------
/// DeathObj.cs
/// 
/// 作成日：2023/12/18
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathObj : MonoBehaviour
{
    #region 変数
    //一度処理判定
    private bool _isPlay = false;
    
    //自身のCollider
    private OriginalCollider _collider = default;
    //MainSystemのリトライ機構
    private IRetryble _retrySct = default;
    //効果音再生クラス
    private SoundPlayer _se = default;
    
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
        _se = GetComponent<SoundPlayer>();
        
        //メインシステムが存在するか
        if (!FindObjectOfType<MainSystem>())
        {
            return;
        }
        _retrySct = FindObjectOfType<MainSystem>().GetComponent<IRetryble>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //死亡検査
        CheckDeath();
    }

    /// <summary>
    /// <para>CheckDeath</para>
    /// <para>死亡判定かどうか確かめます</para>
    /// </summary>
    private void CheckDeath()
    {
        //既に処理を行っている
        if (_isPlay)
        {
            return;
        }

        //衝突判定がある かつ 一度も処理を行っていない
        if (_collider.Collision && !_isPlay)
        {
            //シーン読み込み
            _retrySct.StageRetry();
            _isPlay = true;
            _se.Play();
        }

    }
    #endregion
}
