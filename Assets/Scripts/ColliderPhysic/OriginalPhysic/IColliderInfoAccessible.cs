/// -----------------------------------------------------------------
/// IColliderInfoAccessible.Interface Physic側からCollier情報へのアクセス制限
/// 
/// 作成日：2023/11/27
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;
using PhysicLibrary.Material;

public interface IColliderInfoAccessible
{
    #region プロパティ
    public Vector3[] Edge { get; }

    //物理挙動情報
    public PhysicMaterials Material { get; }
    #endregion

    #region メソッド
    //補完ありの衝突判定
    bool CheckCollisionToInterpolate(Vector3 velocity, bool saveCollision = false);
    #endregion
}
