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
    //自身のColliderのTransform
    public Transform Collider { get; }

    //衝突情報
    public CollisionData Collision { get; }
    //衝突位置
    public Vector3 Point { get; }

    //物理挙動情報
    public PhysicMaterials material { get; }
    #endregion

    #region メソッド
    //補完ありの衝突判定
    public bool CheckCollisionToInterpolate(Vector3 velocity);
    #endregion
}
