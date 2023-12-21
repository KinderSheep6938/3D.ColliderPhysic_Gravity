/// -----------------------------------------------------------------
/// Goal.cs
/// 
/// 作成日：2023/12/19
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Goal : MonoBehaviour
{
    #region 変数
    //一度処理制御
    private bool _isOnce = default;

    //メインシステム
    private MainSystem _mSystem = default;
    //プレイヤー
    private Player _player = default;
    //自身のCollider
    private OriginalCollider _collider = default;
    //クリアテキスト
    private ViewObj _clearTextsObj = default;

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
        _mSystem = FindObjectOfType<MainSystem>();
        _player = FindObjectOfType<Player>();
        _collider = GetComponent<OriginalCollider>();
        _clearTextsObj = GameObject.FindGameObjectWithTag("ClearText").GetComponent<ViewObj>();
        _clearTextsObj.SetView(false);
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
        //ゴール検査
        CheckGoal();
    }

    /// <summary>
    /// <para>CheckGoal</para>
    /// <para>ゴールしたかどうか検査します</para>
    /// </summary>
    private void CheckGoal()
    {
        //既に処理を行っている
        if (_isOnce)
        {
            return;
        }

        //衝突判定があるか かつ 一度も処理を行っていない
        if (_collider.Collision && !_isOnce)
        {
            //クリアテキスト表示
            _clearTextsObj.SetView(true);
            //プレイヤーの処理を停止
            _player.SetStopInput = false;
            _isOnce = true;
        }
    }

    /// <summary>
    /// <para>OnNextStage</para>
    /// <para>次のステージへ進行します</para>
    /// </summary>
    public void OnNextStage(InputAction.CallbackContext context)
    {
        //ボタンが押された かつ 既にゴール判定処理を行っている
        if (context.performed && _isOnce)
        {
            //次のシーンへ
            _mSystem.NextStage();
        }
    }
    #endregion
}
