/// -----------------------------------------------------------------
/// ColliderController.cs　Collider制御
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;

public class ColliderController : MonoBehaviour
{
    #region 変数
    [SerializeField, Header("StaticCollider")]
    private bool _isStatic = false;
    //自身のTransform
    private Transform _transform = default;
    //自身のColliderData
    [SerializeField,Header("ColliderData")]
    private ColliderData _myCol = new();

    #endregion

    #region プロパティ
    //ColliderData取得
    public ColliderData Data { get => _myCol; }
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

    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        //静的である場合は処理しない
        if (!_isStatic)
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

        //変更フラグを消去
        _transform.hasChanged = false;
    }

    /// <summary>
    /// <para>CheckCollision</para>
    /// <para>Colliderが衝突したかどうか検査します</para>
    /// </summary>
    private void CheckCollision()
    {
        
    }

    /// <summary>
    /// 描画処理
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmo描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_myCol.position, _myCol.localScale);
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
