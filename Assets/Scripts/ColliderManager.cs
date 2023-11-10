/// -----------------------------------------------------------------
/// ColliderEngine.cs　Collider判定制御
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderManager
{
    #region 変数
    //二分割用定数
    private const int HALF = 2;
    //Collider情報共有用
    private static List<ColliderController> _worldInColliders = new();
    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    /// <summary>
    /// <para>SetColliderToWorld</para>
    /// <para>対象のCollider情報を共有リストに設定します</para>
    /// </summary>
    /// <param name="target">Collider情報</param>
    public static void SetColliderToWorld(ColliderController target)
    {
        //既に格納されているか
        if (_worldInColliders.Contains(target))
        {
            //格納せず終了
            return;
        }

        //格納
        _worldInColliders.Add(target);
    }

    /// <summary>
    /// <para>RemoveColliderToWorld</para>
    /// <para>対象のCollider情報を共有リストから削除します</para>
    /// </summary>
    /// <param name="target">Collider情報</param>
    public static void RemoveColliderToWorld(ColliderController target)
    {
        //共有リストから削除
        _worldInColliders.Remove(target);
    }

    public static bool CheckCollisionByCollider(ColliderController collider)
    {
        Vector3 checkColliderSize;
        Vector3 localPos;
        //共有リストから全Collider情報を取得し、衝突検査を行います
        foreach(ColliderController target in _worldInColliders)
        {
            //検査対象が自身である
            if(target == collider)
            {
                //検査しない
                continue;
            }

            //検査開始
            //検査対象の大きさを取得
            checkColliderSize = target.Data.localScale / HALF;
            //検査対象の頂点座標を
            localPos = target.transform.InverseTransformPoint(collider.Data.position);
        }

        return false;
    }
    #endregion
}
