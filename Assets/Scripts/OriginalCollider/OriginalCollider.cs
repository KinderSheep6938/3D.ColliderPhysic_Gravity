/// -----------------------------------------------------------------
/// OriginalCollider.cs　Collider制御
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;
using ColliderLibrary.Editor;
using ColliderLibrary.Manager;
using PhysicLibrary.Material;
using PhysicLibrary.DataManager;

public class OriginalCollider : MonoBehaviour, IColliderInfoAccessible
{
    #region 変数
    //Colliderの更新頻度
    private enum UpdateStatus
    {
        Once,       //一度きり
        Update,     //Update毎
        LateUpdate, //LateUpdate毎
        FixedUpdate //FixedUpdate毎
    }
    [SerializeField, Header("更新頻度")]
    private UpdateStatus _updateStatus = UpdateStatus.FixedUpdate;

    //情報の共有判定
    private bool _isSetWorld = false;

    //自身のTransform
    private Transform _transform = default;
    //自身のRenderer
    private MeshRenderer _renderer = default;
    //自身のRigidbody
    private OriginalRigidBody _rigid = default;

    //自身の物理挙動情報
    [SerializeField, Header("物理挙動情報")]
    private PhysicMaterials _physicMaterial = new();
    //自身の当たり判定情報
    [SerializeField, Header("当たり判定情報")]
    private ColliderData _colliderData = new();
    //衝突情報保存用
    private CollisionData _collisionData = new();
    //座標補完用の当たり判定
    private ColliderData _interpolateCol = new();

    //衝突マテリアル
    [SerializeField]
    private Material _normal = default;
    [SerializeField]
    private Material _collision = default;


    #endregion

    #region プロパティ
    //自身のTransform情報取得
    Transform IColliderInfoAccessible.Collider { get => _colliderData.transform; }
    //衝突情報取得
    CollisionData IColliderInfoAccessible.Collision { get => _collisionData; }
    //衝突位置取得
    Vector3 IColliderInfoAccessible.Point { get => _collisionData.point - _collisionData.interpolate; }
    //自身の物理情報
    PhysicMaterials IColliderInfoAccessible.material { get => _physicMaterial; }
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _transform = transform;
        _transform.hasChanged = false;
        _renderer = _transform.GetComponent<MeshRenderer>();
        _rigid = _transform.GetComponent<OriginalRigidBody>();

        //補完分の移動量を初期化
        _collisionData.interpolate = Vector3.zero;

        //Collider生成
        _colliderData = ColliderEditor.SetColliderDataByCube(_transform);
        //Physic情報に自身を設定
        _physicMaterial.transform = _transform;

        //ゲームオブジェクトが無効化されている場合は処理をやめる
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        //Collider情報を管理マネージャーに設定
        ColliderManager.SetColliderToWorld(_colliderData);
        //Physic情報を管理マネージャーに設定
        PhysicDataManager.SetPhysicToWorld(_physicMaterial);
        
    }
    
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //更新頻度が Update でなければ処理しない
        if (_updateStatus != UpdateStatus.Update)
        {
            return;
        }

        //Collider情報の更新
        CheckColliderUpdate();
    }

    /// <summary>
    /// 更新後処理
    /// </summary>
    private void LateUpdate()
    {
        //更新頻度が LateUpdate でなければ処理しない
        if (_updateStatus != UpdateStatus.LateUpdate)
        {
            return;
        }

        //Collider情報の更新
        CheckColliderUpdate();
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        //更新頻度が FixedUpdate でなければ処理しない
        if (_updateStatus != UpdateStatus.FixedUpdate)
        {
            return;
        }

        //Collider情報の更新
        CheckColliderUpdate();
    }

    /// <summary>
    /// <para>CheckColliderUpdate</para>
    /// <para>Collider情報を更新する必要があるか検査します</para>
    /// <para>また必要があった場合は、更新処理を行います</para>
    /// </summary>
    private void CheckColliderUpdate()
    {
        //Transform情報が変わってない
        if (!_transform.hasChanged)
        {
            return;
        }

        //Transformに基づいてColliderを作成する
        _colliderData = ColliderEditor.SetColliderDataByCube(_transform);

        //座標補完あり衝突確認
        CheckCollisionToInterpolate();

        //変更フラグを消去
        _transform.hasChanged = false;
    }

    /// <summary>
    /// <para>CheckCollisionToInterpolate</para>
    /// <para>Colliderの衝突判定を検査します</para>
    /// <para>また移動補完をする場合は、移動分を補完した状態で検査します</para>
    /// </summary>
    private void CheckCollisionToInterpolate()
    {
        //補完をしない または RigidBodyが付属していない
        if(_rigid == default || _rigid.Interpolate == OriginalRigidBody.InterpolateStatus.None)
        {
            //衝突判定を取得する
            _collisionData = ColliderManager.CheckCollision(_colliderData);

        }
        else
        {
            //補完変数に自身の情報を設定
            _interpolateCol = _colliderData;
            //速度分を補完する
            _interpolateCol.position += _rigid.Velocity;
            for(int i = 0;i < EdgeLineManager.MaxEdge; i++)
            {
                _interpolateCol.edgePos[i] += _rigid.Velocity;
            }

            //衝突判定を取得
            _collisionData = ColliderManager.CheckCollision(_interpolateCol);
            //補完分の移動量を設定
            _collisionData.interpolate = _rigid.Velocity;
        }

        //デバッグ用見た目変更
        if (_collisionData.flag)
        {
            _renderer.material = _collision;
        }
        else
        {
            _renderer.material = _normal;
        }
    }

    /// <summary>
    /// 描画処理
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmo描画
        //初期化用
        Matrix4x4 cache = Gizmos.matrix;
        //色設定
        Gizmos.color = Color.green;
        //ローカル設定
        Gizmos.matrix = Matrix4x4.TRS(transform.position,transform.rotation,transform.lossyScale);
        //描画
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        //グローバル設定
        Gizmos.matrix = cache;
    }

    /// <summary>
    /// 有効化処理
    /// </summary>
    private void OnEnable()
    {
        //Collider情報を管理マネージャーに設定
        ColliderManager.SetColliderToWorld(_colliderData);
        //Physic情報を管理マネージャーに設定
        PhysicDataManager.SetPhysicToWorld(_physicMaterial);
    }

    /// <summary>
    /// 無効化処理
    /// </summary>
    private void OnDisable()
    {
        //Collider情報を管理マネージャーから削除
        ColliderManager.RemoveColliderToWorld(_colliderData);
        //Physic情報を管理マネージャーから削除
        PhysicDataManager.RemovePhysicToWorld(_physicMaterial);
    }
    #endregion
}
