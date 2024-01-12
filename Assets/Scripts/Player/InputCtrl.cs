/// -----------------------------------------------------------------
/// PlayerInput.cs プレイヤー入力管理
/// 
/// 作成日：2023/11/28
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCtrl : MonoBehaviour
{
    #region 変数
    //移動入力フラグ
    private bool _isMove = false;

    //プレイヤー操作
    private Player _player = default;
    //InputSystem
    private InputAction _move = default;
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
        _player = GetComponent<Player>();
        _move = GetComponent<PlayerInput>().actions["Move"];
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //プレイヤー処理を取得していない
        if (!_player)
        {
            return;
        }

        //出力処理
        Output();
    }

    /// <summary>
    /// <para>Output</para>
    /// <para>入力に対して、各々出力します</para>
    /// </summary>
    private void Output()
    {
        if (!_isMove)
        {
            return;
        }

        //入力取得
        Vector2 input = _move.ReadValue<Vector2>();
        //入力反映
        _player.Move(input);
    }

    /// <summary>
    /// <para>OnMove</para>
    /// <para>移動入力を行った際に処理します</para>
    /// </summary>
    /// <param name="context">入力情報</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //入力を開始した
        if (context.performed)
        {
            _isMove = true;
        }

        //入力を終了した
        if (context.canceled)
        {
            _isMove = false;
        }
    }

    /// <summary>
    /// <para>OnJump</para>
    /// <para>跳躍入力を行った際に処理します</para>
    /// </summary>
    /// <param name="context">入力情報</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        //ボタンが押されたか
        if (context.performed)
        {
            //プレイヤー跳躍
            _player.ChangeGravity();
        }
    }
    #endregion
}
