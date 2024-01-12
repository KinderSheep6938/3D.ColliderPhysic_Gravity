/// -----------------------------------------------------------------
/// Player.cs プレイヤー挙動
/// 
/// 作成日：2023/11/28
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OriginalMath;
using ColliderLibrary.DataManager;
using ColliderLibrary;

public class Player : MonoBehaviour
{
    #region 変数
    //垂直な力の最小制限量
    private const float PERMISSION_VERTICAL_MINMAGNITUDE = 1f;

    //基礎ベクトル
    private readonly Vector3 _vectorUp = Vector3.up;

    [SerializeField, Header("移動速度")]
    private float _speed = 1f;
    //重力反転可能
    [SerializeField]
    private int _onFloorCnt = 0;

    //プレイヤーの操作可能フラグ
    private bool _canInput = true;
    //一度処理制御
    private bool _isPlay = false;
   
    //カメラのTransform
    private Transform _cameraObj = default;
    //自身のTransform
    private Transform _transform = default;
    //自身のRigidBody
    private OriginalRigidBody _rigid = default;

    #endregion

    #region プロパティ
    //重力切り替え設定
    public int OnFloor { get => _onFloorCnt; set => _onFloorCnt = value; }
    //操作不可能設定
    public bool SetStopInput { set => _canInput = value; }
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _transform = transform;
        _rigid = GetComponent<OriginalRigidBody>();

        //カメラ処理
        CameraCtrl camera = FindObjectOfType<CameraCtrl>();
        //カメラ処理が設定されている
        if (camera)
        {
            //カメラのTransformを取得
            _cameraObj = FindObjectOfType<CameraCtrl>().transform;
        }

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

    }

    /// <summary>
    /// <para>Move</para>
    /// <para>移動を行います</para>
    /// </summary>
    /// <param name="input">[Vector2]入力</param>
    public void Move(Vector2 input)
    {
        //操作不可能である または 切り替え不可能である
        if (!_canInput || _onFloorCnt == 0)
        {
            //一度も処理を行っていない
            if (!_isPlay)
            {
                _isPlay = true;
                _rigid.ResetForce();
                Debug.Log("reset");
            }
            return;
        }

        //カメラの向きから各方向の移動量を取得
        Vector3 set = _cameraObj.right * input.x + _cameraObj.forward * input.y;

        //移動実行
        _rigid.AddForce(set * _speed * Time.deltaTime);
        //キャラを移動方向に向ける
        _transform.LookAt(_transform.position + set);
        _isPlay = false;
        //Debug.Log(input);
    }

    /// <summary>
    /// <para>ChangeGravity</para>
    /// <para>重力を切り替えます</para>
    /// </summary>
    public void ChangeGravity()
    {
        //Debug.Log(GetTo.V3Projection(_rigid.Velocity, _vectorUp).sqrMagnitude);
        //操作不可能である
        if (!_canInput || PERMISSION_VERTICAL_MINMAGNITUDE < Mathf.Abs(GetTo.V3Projection(_rigid.Velocity, _vectorUp).sqrMagnitude))
        {
            return;
        }

        //切り替え可能である
        if (_onFloorCnt != 0)
        {
            //重力反転する
            _rigid.MyGravity = -_rigid.MyGravity;
            _rigid.ResetForce();
            Debug.Log("Change");
        }

    }

    private void OnDrawGizmos()
    {
        foreach(ColliderData a in ColliderDataManager.GetColliderToWorld())
        {
            Gizmos.DrawWireCube(a.position, a.localScale);
        }
    }
    #endregion
}
