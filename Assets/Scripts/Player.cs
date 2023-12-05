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
    [SerializeField, Header("跳躍力")]
    private float _jumpPower = 1f;


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
        _transform = FindObjectOfType<CameraCtrl>().transform;
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
        Vector3 set = _transform.right * input.x + _transform.forward * input.y;

        //移動させる
        _rigid.AddForce(set * _speed * Time.deltaTime);
        //Debug.Log(input);
    }

    /// <summary>
    /// <para>Jump</para>
    /// <para>跳躍を行います</para>
    /// </summary>
    public void Jump()
    {
        //移動させる
        _rigid.AddForce(_transform.up * _jumpPower);
        Debug.Log("jump");
    }
    #endregion
}
