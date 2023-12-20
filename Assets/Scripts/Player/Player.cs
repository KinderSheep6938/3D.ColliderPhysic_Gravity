/// -----------------------------------------------------------------
/// Player.cs プレイヤー挙動
/// 
/// 作成日：2023/11/28
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region 変数
    [SerializeField, Header("移動速度")]
    private float _speed = 1f;

    //重力反転可能
    [SerializeField]
    private bool _canChange = true;
    //プレイヤーの操作可能フラグ
    private bool _canInput = true;
   
    //カメラのTransform
    private Transform _cameraObj = default;
    //自身のTransform
    private Transform _transform = default;
    //自身のRigidBody
    private OriginalRigidBody _rigid = default;

    #endregion

    #region プロパティ
    //重力切り替え設定
    public bool CanChange { set => _canChange = value; }
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
        _cameraObj = FindObjectOfType<CameraCtrl>().transform;
        _transform = transform;
        _rigid = GetComponent<OriginalRigidBody>();
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
        if (!_canInput || !_canChange)
        {
            return;
        }

        //入力方向
        Vector3 set = _cameraObj.right * input.x + _cameraObj.forward * -input.y;

        //移動実行
        _rigid.AddForce(set * _speed * Time.deltaTime);

        _transform.LookAt(_transform.position + set);
        //Debug.Log(input);
    }

    /// <summary>
    /// <para>ChangeGravity</para>
    /// <para>重力を切り替えます</para>
    /// </summary>
    public void ChangeGravity()
    {
        //操作不可能である
        if (!_canInput)
        {
            return;
        }

        //切り替え可能である
        if (_canChange)
        {
            //重力反転する
            _rigid.MyGravity = -_rigid.MyGravity;
            _rigid.ResetForce();
            _canChange = false;
            Debug.Log("Change");
        }

    }
    #endregion
}
