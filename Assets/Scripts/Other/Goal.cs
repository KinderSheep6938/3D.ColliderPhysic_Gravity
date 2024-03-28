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
    //各テキストのオブジェクトタグ
    private const string CLEARTEXT_OBJECTTAG = "ClearText";
    private const string MANUALTEXT_OBJECTTAG = "ManualText";

    //一度処理制御
    private bool _isPlay = default;

    //メインシステム
    private MainSystem _mSystem = default;
    //プレイヤー
    private Player _player = default;
    //自身のCollider
    private OriginalCollider _collider = default;
    //クリアテキスト
    private ViewObj _clearTextsObj = default;
    //操作方法テキスト
    private ViewObj _manualTextsObj = default;
    //効果音再生クラス
    private SoundPlayer _se = default;
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
        _se = GetComponent<SoundPlayer>();
        _clearTextsObj = GameObject.FindGameObjectWithTag(CLEARTEXT_OBJECTTAG).GetComponent<ViewObj>();
        _manualTextsObj = GameObject.FindGameObjectWithTag(MANUALTEXT_OBJECTTAG).GetComponent<ViewObj>();
        _clearTextsObj.SetView(false);
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
        if (_isPlay)
        {
            return;
        }

        //衝突判定があるか かつ 一度も処理を行っていない
        if (_collider.Collision && !_isPlay)
        {
            //テキスト表示処理
            _clearTextsObj.SetView(true);
            _manualTextsObj.SetView(false);
            //プレイヤーの処理を停止
            _player.SetStopInput = false;
            _isPlay = true;
            _se.Play();
        }
    }

    /// <summary>
    /// <para>OnNextStage</para>
    /// <para>次のステージへ進行します</para>
    /// </summary>
    public void OnNextStage(InputAction.CallbackContext context)
    {
        //ボタンが押された かつ 既にゴール判定処理を行っている
        if (context.performed && _isPlay)
        {
            //次のシーンへ
            _mSystem.NextStage();
            //テキスト表示処理
            _clearTextsObj.SetView(false);
            _manualTextsObj.SetView(true);
        }
    }
    #endregion
}
