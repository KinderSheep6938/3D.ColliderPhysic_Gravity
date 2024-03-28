/// -----------------------------------------------------------------
/// DeathCollider.cs
/// 
/// 作成日：2023/12/15
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBorder : MonoBehaviour
{
    #region 変数
    //基礎ベクトル
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorUp = Vector3.up;

    //一度処理制御
    private bool _isPlay = false;

    //最高高度、最小高度
    [SerializeField]
    private float _maxHeight = 50f;
    [SerializeField]
    private float _minHeight = -50f;

    //プレイヤー
    private Transform _player = default;
    //MainSystemのリトライ機構
    private IRetryble _retrySct = default;
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
        _player = FindObjectOfType<Player>().transform;
        _se = GetComponent<SoundPlayer>();

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
        CheckDeath();
    }

    /// <summary>
    /// <para>CheckDeath</para>
    /// <para>死亡判定に衝突したか検査します</para>
    /// </summary>
    private void CheckDeath()
    {
        //既に処理を行っている
        if (_isPlay)
        {
            return;
        }

        //死亡のラインを超えている
        if (_maxHeight < _player.position.y || _player.position.y < _minHeight)
        {
            //シーン読み込み
            _retrySct.StageRetry();
            _isPlay = true;
            _se.Play();
        }
    }

    /// <summary>
    /// 描画処理
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmo描画
        //高度は０の現在位置を取得
        Vector3 nowPos = transform.position - _vectorUp * transform.position.y;

        //色設定
        Gizmos.color = Color.red;
        //描画
        Gizmos.DrawLine(nowPos + _vectorUp * _maxHeight,nowPos + _vectorUp * _minHeight);
        Gizmos.DrawSphere(nowPos + _vectorUp * _maxHeight, 0.5f);
        Gizmos.DrawSphere(nowPos + _vectorUp * _minHeight, 0.5f);
        //現在高度色
        Gizmos.color = Color.red;
        //プレイヤーの現在高度
        if(_player != default)
        {
            Gizmos.DrawSphere(_vectorUp * _player.position.y + nowPos - _vectorUp * nowPos.y, 1);
        }
    }
    #endregion
}
