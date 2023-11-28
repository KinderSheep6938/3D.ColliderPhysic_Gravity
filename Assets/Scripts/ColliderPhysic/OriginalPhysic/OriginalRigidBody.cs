/// -----------------------------------------------------------------
/// OriginalRigidBody.cs 物理挙動
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
    //最小速度
    private const float PERMISSION_MINMAGUNITYDE= 0.00001f;

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

    //面の衝突判定
    private bool _onSurface = false;
    //最小重力加速度
    private Vector3 _minGravity = default;

    //自身の物理データ
    [SerializeField]
    private PhysicData _physicData = new(PhysicManager.CommonMass, PhysicManager.CommonGravity);
    //自身のCollider
    private IColliderInfoAccessible _colliderAccess = default;
    //自身のTransform
    private Transform _transform = default;
    #endregion

    #region プロパティ
    //現在の速度
    public Vector3 Velocity { get => _physicData.velocity; }
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _transform = transform;
        _colliderAccess = GetComponent<IColliderInfoAccessible>();

        //当たり判定側の接続インターフェイスを設定
        _physicData.colliderInfo = _colliderAccess;


        //ゲームオブジェクトが無効化されている場合は処理をやめる
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _physicData.force += new Vector3(0, 10f, 0);
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

        //重力制御
        Gravity();

        //衝突・反発制御
        ColliderRepulsion();

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
        Vector3 gravity = PhysicManager.Gravity(_physicData);

        //重力によって衝突する
        if (_colliderAccess.CheckCollisionToInterpolate(gravity))
        {
            //面衝突判定を設定
            _onSurface = true;
            return;
        }
        //面衝突判定を消去
        _onSurface = false;
        //重力を加算
        _physicData.force += gravity;

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Colliderが衝突した際の反発制御を行います</para>
    /// </summary>
    private void ColliderRepulsion()
    {
        //現在かかっている力がない
        if(_physicData.force == _vectorZero)
        {
            //速度を初期化
            _physicData.velocity = _vectorZero;
            return;
        }

        //現在かかっている力を速度に変換
        Vector3 interpolateVelocity = ForceToVelocity();

        //Colliderが設定されていない
        if (_colliderAccess == default)
        {
            //そのまま速度として付加
            _physicData.velocity = interpolateVelocity;
            return;
        }

        //面接触判定がない かつ 現在速度で衝突判定が起きない
        if (!_onSurface && !_colliderAccess.CheckCollisionToInterpolate(interpolateVelocity))
        {
            //そのまま速度として付加
            _physicData.velocity = interpolateVelocity;
            return;
        }

        //Debug.Log("col");
        //最小重力加速度設定
        //_minGravity = PhysicManager.Gravity(_physicData);
        //_physicData.force += _minGravity;
        //衝突した情報を加味して、速度を算出
        _physicData.force = PhysicManager.ChangeForceByPhysicMaterials(_physicData);
        //_physicData.force += _minGravity;

        //反発後の力が最低値以下であれば物質にかかる力を消去する
        if (_physicData.force.sqrMagnitude <= PERMISSION_MINMAGUNITYDE)
        {
            _physicData.force = _vectorZero;
        }
        //反発後の力を速度に反映
        _physicData.velocity = ForceToVelocity();

        //衝突している物体の状況を加味して、速度を算出
        //_myPhysic.velocity = -(_myPhysic.reboundRatio * _myPhysic.velocity);
    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>物体に与えられた力を速度に変換します</para>
    /// </summary>
    private Vector3 ForceToVelocity()
    {
        //物質にかかっている力がない
        if (_physicData.force == _vectorZero)
        {
            return _vectorZero;
        }

        //返却用
        Vector3 returnVelocity = _physicData.force;
        
        //空気抵抗を考慮
        returnVelocity += _physicData.airResistance * -returnVelocity;

        return returnVelocity;
    }

    /// <summary>
    /// <para>ApplyVelocity</para>
    /// <para>自身の速度を座標に反映させます</para>
    /// </summary>
    private void ApplyVelocity()
    {
        //そもそも速度がないときは処理しない
        if(_physicData.velocity == _vectorZero)
        {
            return;
        }

        //設定されている座標固定を反映

        //特になしが設定されている
        if(_freezeStatus == Freeze.None)
        {
            //速度反映
            _transform.position += _physicData.velocity;
            return;
        }

        //反映用
        Vector3 noFreeze = _vectorZero;
        //X軸が固定されていない
        if(_freezeStatus != Freeze.XOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.XtoZ)
        {
            //代入
            noFreeze += _vectorRight * _physicData.velocity.x;
        }

        //Y軸が固定されていない
        if (_freezeStatus != Freeze.YOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.YtoZ)
        {
            //代入
            noFreeze += _vectorUp * _physicData.velocity.y;
        }

        //Z軸が固定されていない
        if (_freezeStatus != Freeze.ZOnly && _freezeStatus != Freeze.XtoZ && _freezeStatus != Freeze.YtoZ)
        {
            //代入
            noFreeze += _vectorForward * _physicData.velocity.z;
        }

        //固定されていない方向のみ速度を反映
        _transform.position += noFreeze;
    }

    /// <summary>
    /// <para>AddForce</para>
    /// <para>物質に力を加えます</para>
    /// </summary>
    /// <param name="add">加える力</param>
    public void AddForce(Vector3 add)
    {
        //力を加算
        _physicData.force += add;
    }
    #endregion
}
