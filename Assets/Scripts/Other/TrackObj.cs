/// -----------------------------------------------------------------
/// TrackObj.cs
/// 
/// 作成日：2023/12/15
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObj : MonoBehaviour
{
    #region 変数
    //追跡速度
    private const float TRACK_SPEED = 10f;
    //最小追跡距離
    private const float PERMISSION_TRACK_DISTANCE = 0.001f;

    //自身のTransform
    private Transform _transform = default;
    //追跡先
    [SerializeField]
    private Transform _track = default;

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
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //追跡処理
        TrackToTransform();
    }

    /// <summary>
    /// <para>TrackToTransform</para>
    /// <para>対象オブジェクトを追跡します</para>
    /// </summary>
    private void TrackToTransform()
    {
        //追跡距離ベクトルを取得
        Vector3 trackDistance = _track.position - _transform.position;

        //最小追跡距離以下である
        if(trackDistance.sqrMagnitude < PERMISSION_TRACK_DISTANCE)
        {
            _transform.position = _track.position;
            return;
        }

        //追跡
        _transform.position += trackDistance * TRACK_SPEED * Time.deltaTime;
    }
    #endregion
}
