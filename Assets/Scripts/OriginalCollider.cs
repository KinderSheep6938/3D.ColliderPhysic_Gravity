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

public class OriginalCollider : MonoBehaviour
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

    //衝突判定
    [SerializeField, Header("衝突判定"),ReadOnly]
    private bool _isCollision = false;

    //自身のTransform
    private Transform _transform = default;
    //自身のColliderData
    [SerializeField,Header("物理判定情報")]
    private ColliderData _myCol = new();


    #endregion

    #region プロパティ
    //ColliderData取得
    public ColliderData Data { get => _myCol; }
    //Transform取得
    public Transform MyTransform { get => _transform; }
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

        //Collider生成
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);
        //Collider情報を管理マネージャーに設定
        ColliderManager.SetColliderToWorld(this);
        
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
        CheckColliderUpdata();
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
        CheckColliderUpdata();
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
        CheckColliderUpdata();
    }

    /// <summary>
    /// <para>CheckColliderUpdata</para>
    /// <para>Collider情報を更新する必要があるか検査します</para>
    /// <para>また必要があった場合は、更新処理を行います</para>
    /// </summary>
    private void CheckColliderUpdata()
    {
        //Transform情報が変わってない
        if (!_transform.hasChanged)
        {
            return;
        }

        //Transformに基づいてColliderを作成する
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);

        //衝突判定を取得する
        _isCollision = ColliderManager.CheckCollision(this);

        //変更フラグを消去
        _transform.hasChanged = false;
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
        ColliderManager.SetColliderToWorld(this);
    }

    /// <summary>
    /// 無効化処理
    /// </summary>
    private void OnDisable()
    {
        //Collider情報を管理マネージャーから削除
        ColliderManager.RemoveColliderToWorld(this);
    }

    #endregion
}
