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
using PhysicLibrary.Material;
using PhysicLibrary.CollisionPhysic;

public class OriginalRigidBody : MonoBehaviour
{
    #region 変数
    //最小速度
    private const float PERMISSION_MINMAGUNITYDE = 0.0001f;

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

    //再帰処理初回処理判定
    private bool _isOnce = true;
    //面の衝突判定
    private bool _onSurface = false;
    //重力加速度
    private Vector3 _gravity = default;

    //自身の物理データ
    [SerializeField]
    private PhysicData _physicData = new(PhysicManager.CommonMass, PhysicManager.CommonGravity);
    //自身のCollider
    private IColliderInfoAccessible _colliderAccess = default;
    //自身のTransform
    private Transform _transform = default;
    //一番最後に使用した衝突データ
    private OtherPhysicData _usedCollisionData = default;

    #endregion

    #region プロパティ
    //重力
    public Vector3 MyGravity { get => _physicData.gravity; set => _physicData.gravity = value; }
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
        _gravity = PhysicManager.Gravity(_physicData);

        //重力によって衝突する
        if (_colliderAccess.CheckCollisionToInterpolate(_gravity, true))
        {
            //面衝突判定を設定
            _onSurface = true;
            return;
        }
        //面衝突判定を消去
        _onSurface = false;
        //重力を加算
        _physicData.force += _gravity;

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Colliderが衝突した際の反発制御を行います</para>
    /// </summary>
    private void ColliderRepulsion()
    {
        //現在かかっている力を速度に変換
        Vector3 interpolateVelocity = ForceToVelocity(_physicData.force);

        //Colliderが設定されていない
        if (_colliderAccess == default)
        {
            //そのまま速度として付加
            _physicData.velocity = interpolateVelocity;
            return;
        }

        //面接触判定がない かつ 現在速度で衝突判定が起きない
        if (!_onSurface && !_colliderAccess.CheckCollisionToInterpolate(interpolateVelocity, true))
        {
            //そのまま速度として付加
            _physicData.velocity = interpolateVelocity;
            return;
        }

        //Debug.Log("-----------------------------------------------------");
        //初回処理設定
        _isOnce = true;
        //衝突情報を検索
        CheckCollisionData(_physicData.colliderInfo.Material);
        //_physicData.force += _minGravity;

        //反発後の力が最低値以下であれば物質にかかる力を消去する
        if (_physicData.force.sqrMagnitude <= PERMISSION_MINMAGUNITYDE)
        {
            _physicData.force = _vectorZero;
        }
        //反発後の力を速度に反映
        _physicData.velocity = ForceToVelocity(_physicData.force);

        //衝突している物体の状況を加味して、速度を算出
        //_myPhysic.velocity = -(_myPhysic.reboundRatio * _myPhysic.velocity);
    }

    /// <summary>
    /// <para>CheckCollisionData</para>
    /// <para>検査対象が登録されている衝突データから反発後の力を算出します</para>
    /// <para>再起処理</para>
    /// </summary>
    /// <param name="data">検査対象</param>
    private void CheckCollisionData(PhysicMaterials data)
    {
        //登録されていない場合は処理しない
        if (!CollisionPhysicManager.CheckWaitContains(data))
        {
            //初回処理か
            if (_isOnce)
            {
                //Debug.Log("once");
                _isOnce = false;
                //重力であたる物体を検査
                _colliderAccess.CheckCollisionToInterpolate(ForceToVelocity(_gravity), true);
                //再起処理
                CheckCollisionData(data);
            }
            else 
            {
                //実際の力に反映
                _physicData.force = ChangeForceByPhysic(_physicData, _usedCollisionData);
            }
            return;
        }
        //初回処理判定消去
        _isOnce = false;

        //使用する衝突データを取得
        _usedCollisionData = CollisionPhysicManager.GetCollision(data);
        //Debug.Log("m" + _usedCollisionData.interpolate + "f" + _physicData.force.sqrMagnitude + "Us" + _usedCollisionData.point);
        //Debug.Log(_physicData.force);
        //実際の力に反映
        _physicData.force = ChangeForceByPhysic(_physicData, _usedCollisionData);
        //Debug.Log(_physicData.force);

        //まだ登録されている
        if (CollisionPhysicManager.CheckWaitContains(data))
        {
            //再起処理
            CheckCollisionData(data);
        }
        return;
    }

    /// <summary>
    /// <para>ChangeForceByPhysic</para>
    /// <para>物質にかかる力を</para>
    /// </summary>
    /// <param name="myPhysic">自身の物質</param>
    /// <param name="environment">衝突環境</param>
    /// <returns>環境影響後の力</returns>
    private Vector3 ChangeForceByPhysic(PhysicData myPhysic, OtherPhysicData environment)
    {
        //Debug.Log(myPhysic.force);
        //面に対し水平方向の力取得
        Vector3 horizontalForce = PhysicManager.HorizontalForceByPhysicMaterials(myPhysic, environment);
        //Debug.Log("Hor" + horizontalForce);
        //面に対し垂直方向の力取得
        Vector3 verticalForce = PhysicManager.VerticalForceByPhysicMaterials(myPhysic, environment);
        //Debug.Log("Repu" + verticalForce);
        //合成
        Vector3 returnForce = horizontalForce + verticalForce;

        //垂直方向に働く力がない
        if (verticalForce == _vectorZero)
        {
            //面に接するように力を加える
            Vector3 intrusion = PhysicManager.NoForceToCollision(myPhysic, environment);
            //Debug.Log("CollFor :" + intrusion);

            returnForce += intrusion;
        }

        return returnForce; 
    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>物体に与えられた力を速度に変換します</para>
    /// </summary>
    private Vector3 ForceToVelocity(Vector3 force)
    {
        //返却用
        Vector3 returnVelocity = force;
        
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
