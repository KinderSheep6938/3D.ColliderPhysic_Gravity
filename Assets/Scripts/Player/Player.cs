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
   
    //カメラのTransform
    private Transform _cameraObj = default;
    //自身のTransform
    private Transform _transform = default;
    //自身のRigidBody
    private OriginalRigidBody _rigid = default;

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
        if(input.sqrMagnitude == 0)
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
        //移動させる
        _rigid.MyGravity = -_rigid.MyGravity;
        Debug.Log("Change");
    }
    #endregion
}
