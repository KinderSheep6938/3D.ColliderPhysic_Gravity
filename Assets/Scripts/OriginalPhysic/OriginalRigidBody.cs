/// -----------------------------------------------------------------
/// OriginalRigidBody.cs
/// 
/// 作成日：2023/11/17
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicLibrary;

public class OriginalRigidBody : MonoBehaviour
{
    #region 変数
    //自身の物理データ
    [SerializeField]
    private PhysicData _myPhysic = new();

    //自身のCollider
    private OriginalCollider _collider = default;
    //自身のTransform
    private Transform _transform = default;
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
        _collider = _transform.GetComponent<OriginalCollider>();
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        //重力
        Gravity();

        //反発
        Repulsion(_collider);

        //速度反映
        ApplyVelocity();
    }

    /// <summary>
    /// <para>Gravity</para>
    /// <para>自身に重力を反映させます</para>
    /// </summary>
    private void Gravity()
    {
        //重力加算
        _myPhysic.velocity = PhysicManager.Gravity(_myPhysic);
    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Colliderによる反発力を取得します</para>
    /// </summary>
    private void Repulsion(OriginalCollider collider)
    {
        //Colliderが設定されていない場合は処理しない
        if(collider == default)
        {
            return;
        }

        //衝突判定があるか
        if (collider.Collision)
        {
            //そのまま逆へ反発
            _myPhysic.velocity = -_myPhysic.velocity;
        }
    }

    /// <summary>
    /// <para>ApplyVelocity</para>
    /// <para>自身の速度を座標に反映させます</para>
    /// </summary>
    private void ApplyVelocity()
    {
        //速度反映
        _transform.position += _myPhysic.velocity;
    }
    #endregion
}
