/// -----------------------------------------------------------------
/// PhysicManager.cs
/// 
/// 作成日：2023/11/17
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicLibrary;

public class PhysicManager : MonoBehaviour
{
    #region 変数
    private static readonly Vector3 _gravityScale = new Vector3(0f, -0.981f, 0f);
    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    public static Vector3 Gravity(PhysicData physic)
    {
        //重力加速度を算出 : 質量 x 重力
        Vector3 acceleration = physic.mass * _gravityScale;
        //重力加速度を時間積分 : 重力加速度 x 経過時間
        Vector3 velocity = physic.velocity + (acceleration * Time.fixedDeltaTime);
        //返却
        return velocity;
    }
    #endregion
}
