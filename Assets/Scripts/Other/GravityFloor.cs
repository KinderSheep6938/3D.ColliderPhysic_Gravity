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
    //引力距離
    private const float GRAVITY_DISTANCE = 2f;
    //オブジェクト範囲
    private const float OBJECT_RANGE = 0.5f;

    //一度処理制御
    private bool _isOnce = false;

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
        bool rangeZ = (-OBJECT_RANGE <= localPos.z && localPos.z <= OBJECT_RANGE);

        //横幅の範囲外である
        if ((!rangeX || !rangeZ) && _isOnce)
        {
            _player.OnFloor -= 1;
            _isOnce = false;
            CheckGravityCanChange();
            return;
        }
        //Debug.Log("in:" + localPos);

        //引き寄せ距離をローカル化する
        float localGravityDistance = GRAVITY_DISTANCE / _transform.localScale.y;
        //上に乗っている かつ 引き寄せ可能距離である かつ 重力方向がぶつかる方向である
        if (OBJECT_RANGE <= Mathf.Abs(localPos.y) && Mathf.Abs(localPos.y) <= OBJECT_RANGE + localGravityDistance)
        {
            CheckGravityCanChange();
            return;
        }


        _isOnce = false;
        Debug.DrawLine(_transform.position, _transform.position + _transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.white);
        Debug.DrawLine(_transform.position, _transform.position + -_transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.black);
    }

    /// <summary>
    /// <para>GetGravityOnVertical</para>
    /// <para>重力が水平方向に対してどのように働いているか調べます</para>
    /// </summary>
    private void CheckGravityCanChange()
    {
        //衝突判定がある かつ まだ一度も処理を行っていない
        if (_collider.Collision && !_isOnce)
        {
            //切り替え可能に
            _player.OnFloor += 1;
            _isOnce = true;
            Debug.Log("on");
        }
    }
    #endregion
}
