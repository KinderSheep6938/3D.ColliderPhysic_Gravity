/// -----------------------------------------------------------------
/// Title.cs
/// 
/// 作成日：2023/01/12
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Title : MonoBehaviour
{
    #region 変数
    //メインシステム
    private MainSystem _mSystem = default;
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _mSystem = FindObjectOfType<MainSystem>();
    }

    /// <summary>
    /// <para>OnStart</para>
    /// <para>スタート入力を行った際に処理します</para>
    /// </summary>
    /// <param name="context">入力情報</param>
    public void OnStart(InputAction.CallbackContext context)
    {
        //ボタンが押されたか
        if (context.performed)
        {
            //ステージへ移行
            _mSystem.NextStage();
        }
    }
    #endregion
}
