/// -----------------------------------------------------------------
/// UpFloor.cs
/// 
/// 作成日：2023/12/14
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicLibrary.Manager;

public class GravityFloor : MonoBehaviour
{
    #region 変数
    //引力距離
    private const float GRAVITY_DISTANCE = 10f;
    //重力反転角度
    private const float GRAVITY_ANGLE = 180f;
    //オブジェクト
    private const float OBJECT_RANGE = 0.5f;
    //基礎ベクトル
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorForward = Vector3.forward;

    //自身のTransform
    private Transform _transform = default;
    //プレイヤーのTransform
    private Transform _player = default;
    //プレイヤーのRigid
    private OriginalRigidBody _playerRigid = default;

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
        _player = FindObjectOfType<Player>().transform;
        _playerRigid = _player.GetComponent<OriginalRigidBody>();


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
        Vector3 localPos = _transform.InverseTransformPoint(_player.position);
        //X軸の範囲内
        bool rangeX = (-OBJECT_RANGE <= localPos.x && localPos.x <= OBJECT_RANGE);
        bool rangeZ = (-OBJECT_RANGE <= localPos.z && localPos.z <= OBJECT_RANGE);

        //横幅の範囲外である
        if (!rangeX || !rangeZ)
        {
            return;
        }
        Debug.Log("in:" + localPos);

        //引き寄せ距離をローカル化する
        float localGravityDistance = GRAVITY_DISTANCE / _transform.localScale.y;
        //上に乗っている かつ 引き寄せ可能距離である
        if (OBJECT_RANGE <= localPos.y && localPos.y <= OBJECT_RANGE + localGravityDistance)
        {
            Debug.Log("up");
            _playerRigid.MyGravity = _transform.up * PhysicManager.CommonGravity.y;
            _player.eulerAngles = _vectorZero;
        }

        //下に乗っている かつ 引き寄せ可能距離である
        if (localPos.y <= -OBJECT_RANGE && -OBJECT_RANGE - localGravityDistance <= localPos.y)
        {
            Debug.Log("down");
            _playerRigid.MyGravity = -_transform.up * PhysicManager.CommonGravity.y;
            _player.eulerAngles = _vectorForward * GRAVITY_ANGLE;
        }

        Debug.DrawLine(_transform.position, _transform.position + _transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.white);
        Debug.DrawLine(_transform.position, _transform.position + -_transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.black);
    }
    #endregion
}
