/// -----------------------------------------------------------------
/// UpFloor.cs
/// 
/// 作成日：2023/12/14
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OriginalMath;

public class GravityFloor : MonoBehaviour
{
    #region 変数
    //オブジェクト範囲
    private const float OBJECT_RANGE = 0.5f;

    //一度処理制御
    private bool _isOnePlay = false;
    //切り替え判定可能距離
    private float _canChangeDistance = 0.55f;

    //自身のTransform
    private Transform _transform = default;
    //自身のCollider
    private OriginalCollider _collider = default;
    //プレイヤーのTransform
    private Player _player = default;

    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化処理
        _transform = transform;
        _player = FindObjectOfType<Player>().GetComponent<Player>();
        _collider = GetComponent<OriginalCollider>();

        //引き寄せ可能距離にプレイヤーの縦幅半分の距離を足す
        _canChangeDistance += _player.transform.localScale.y / 2;
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
        SetGravity();
    }

    /// <summary>
    /// <para>HeightOverDestroy</para>
    /// <para>ある一定高度を超えた場合、削除します</para>
    /// </summary>
    private void SetGravity()
    {
        //プレイヤー座標をローカル化
        Vector3 localPos = _transform.InverseTransformPoint(_player.transform.position);
        //X軸の範囲内
        bool rangeX = (-OBJECT_RANGE <= localPos.x && localPos.x <= OBJECT_RANGE);
        //Z軸の範囲内
        bool rangeZ = (-OBJECT_RANGE <= localPos.z && localPos.z <= OBJECT_RANGE);

        //横幅の範囲外である かつ 一度目の処理である
        if ((!rangeX || !rangeZ) && _isOnePlay)
        {
            DeleteConect();
            CheckGravityCanChange();
            return;
        }
        //Debug.Log("in:" + localPos);

        //引き寄せ距離をローカル化する
        float localGravityDistance = _canChangeDistance / _transform.localScale.y;
        //上に乗っている かつ 引き寄せ可能距離である かつ 重力方向がぶつかる方向である
        if (OBJECT_RANGE <= Mathf.Abs(localPos.y) && Mathf.Abs(localPos.y) <= OBJECT_RANGE + localGravityDistance)
        {
            CheckGravityCanChange();
            return;
        }

        DeleteConect();
        Debug.DrawLine(_transform.position, _transform.position + _transform.up * (_transform.localScale.y / 2 + _canChangeDistance),Color.white);
        Debug.DrawLine(_transform.position, _transform.position + -_transform.up * (_transform.localScale.y / 2 + _canChangeDistance),Color.black);
    }

    /// <summary>
    /// <para>GetGravityOnVertical</para>
    /// <para>重力が水平方向に対してどのように働いているか調べます</para>
    /// </summary>
    private void CheckGravityCanChange()
    {
        //衝突判定がある かつ まだ一度も処理を行っていない
        if (_collider.Collision && !_isOnePlay)
        {
            //接地カウントを増加
            _player.OnFloor += 1;
            _isOnePlay = true;
            //Debug.Log("on");
        }
    }

    /// <summary>
    /// <para>DeleteConect</para>
    /// <para>接地判定を削除する</para>
    /// </summary>
    private void DeleteConect()
    {
        //処理を行った
        if (_isOnePlay)
        {
            //接地カウントを減少
            _player.OnFloor -= 1;
            _isOnePlay = false;
        }

    }
    #endregion
}
