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
using PhysicLibrary.Manager;


public class OriginalRigidBody : MonoBehaviour
{
    #region 変数
    //基準Vector
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorRight = Vector3.right;
    private readonly Vector3 _vectorUp = Vector3.up;
    private readonly Vector3 _vectorForward = Vector3.forward;

    //座標固定基準
    private enum Freeze
    {
        None,   //特になし
        XOnly,  //X軸固定
        YOnly,  //Y軸固定
        ZOnly,  //Z軸固定
        XtoY,   //X軸とY軸固定
        YtoZ,   //Y軸とZ軸固定
        XtoZ,   //X軸とZ軸固定
        All     //全て固定
    }
    [SerializeField, Header("座標固定")]
    private Freeze _freezeStatus = Freeze.None;

    //自身の物理データ
    [SerializeField]
    private PhysicData _myPhysic = new();
    //自身のCollider
    private OriginalCollider _collider = default;
    //自身のTransform
    private Transform _transform = default;
    #endregion

    #region プロパティ
    //現在の速度
    public Vector3 Velocity { get => _myPhysic.velocity; }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _myPhysic.velocity = new Vector3(0, 1f, 0);
        }
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        //座標が全て固定されている場合は何もしない
        if(_freezeStatus == Freeze.All)
        {
            return;
        }

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
        //重力加速度加算
        _myPhysic.force = PhysicManager.Gravity(_myPhysic);

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Colliderによる反発力を取得します</para>
    /// </summary>
    private void Repulsion(OriginalCollider collider)
    {
        //Colliderが設定されていない または 衝突判定がないか
        if (collider == default || !collider.CollisionData.collision)
        {
            //物体に加えられている力をそのまま速度として付加
            ForceToVelocity();
            return;
        }

        //衝突した情報を加味して、速度を算出
        _myPhysic.force = PhysicManager.RepulsionForceByCollider(_myPhysic, collider.CollisionData);
        ForceToVelocity();

        //衝突している物体の状況を加味して、速度を算出
        //_myPhysic.velocity = -(_myPhysic.reboundRatio * _myPhysic.velocity);
    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>物体に与えられた力を速度に変換します</para>
    /// </summary>
    private void ForceToVelocity()
    {
        //力を速度に代入
        _myPhysic.velocity = _myPhysic.force;

        //抵抗力を加味
        _myPhysic.velocity -= _myPhysic.resistance * _myPhysic.velocity;

        return;
    }

    /// <summary>
    /// <para>ApplyVelocity</para>
    /// <para>自身の速度を座標に反映させます</para>
    /// </summary>
    private void ApplyVelocity()
    {
        //設定されている座標固定を反映

        //特になしが設定されている
        if(_freezeStatus == Freeze.None)
        {
            //速度反映
            _transform.position += _myPhysic.velocity;
            return;
        }

        //反映用
        Vector3 noFreeze = _vectorZero;
        //X軸が固定されていない
        if(_freezeStatus != Freeze.XOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.XtoZ)
        {
            //代入
            noFreeze += _vectorRight * _myPhysic.velocity.x;
        }

        //Y軸が固定されていない
        if (_freezeStatus != Freeze.YOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.YtoZ)
        {
            //代入
            noFreeze += _vectorUp * _myPhysic.velocity.y;
        }

        //Z軸が固定されていない
        if (_freezeStatus != Freeze.ZOnly && _freezeStatus != Freeze.XtoZ && _freezeStatus != Freeze.YtoZ)
        {
            //代入
            noFreeze += _vectorForward * _myPhysic.velocity.z;
        }

        //固定されていない方向のみ速度を反映
        _transform.position += noFreeze;
    }
    #endregion
}
