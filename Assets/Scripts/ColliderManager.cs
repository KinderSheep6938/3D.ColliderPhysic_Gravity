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
    //直線検査の最小距離
    private const float MIN_CHECK_DISTANCE = 0.01f;

    //Collider情報共有用
    private static List<OriginalCollider> _worldInColliders = new();

    //基礎Vector情報保存用
    private static readonly Vector3 _vector3Up    = Vector3.up;
    private static readonly Vector3 _vector3Right = Vector3.right;
    private static readonly Vector3 _vector3Flont = Vector3.forward;

    //Colliderの衝突判定範囲
    private static readonly Vector3 _collisionRange = Vector3.one / HALF;

    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    /// <summary>
    /// <para>SetColliderToWorld</para>
    /// <para>対象のCollider情報を共有リストに設定します</para>
    /// </summary>
    /// <param name="target">Collider情報</param>
    public static void SetColliderToWorld(OriginalCollider target)
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
    public static void RemoveColliderToWorld(OriginalCollider target)
    {
        //共有リストから削除
        _worldInColliders.Remove(target);
    }

    /// <summary>
    /// <para>CheckCollision</para>
    /// <para>Collider情報から衝突しているか検査します</para>
    /// </summary>
    /// <param name="collider">Collider情報</param>
    /// <returns>衝突判定</returns>
    public static bool CheckCollision(OriginalCollider collider)
    {
        //検査対象に一番近い頂点座標
        Vector3 nearEdge;

        //共有リストから全Collider情報を取得し、衝突検査を行います
        foreach(OriginalCollider target in _worldInColliders)
        {
            //検査対象が 自身 である
            if(target == collider)
            {
                //検査しない
                continue;
            }

            //自身の頂点座標 から 最も近い頂点座標 を格納
            nearEdge = GetNearEdgeByOrigin(target, collider.Data.edgePos);
            //最も近い頂点座標 が 検査対象のCollider の内部にある
            if (CheckPointInCollider(nearEdge, target))
            {
                Debug.Log("MyCollision");
                //衝突判定がある
                return true;
            }

            //検査対象の頂点座標 から 最も近い頂点座標 を格納
            nearEdge = GetNearEdgeByOrigin(collider, target.Data.edgePos);
            //最も近い頂点座標 が 自身のCollider の内部にある
            if(CheckPointInCollider(nearEdge, collider))
            {
                Debug.Log("Collision");
                return true;
            }

            //自身の中心座標 が 検査対象のCollider の内部にある
            if(CheckPointInCollider(collider.Data.position, target))
            {
                Debug.Log("MyCenterCollision");
                //衝突判定がある
                return true;
            }

            //検査対処の中心座標 が 自身のCollider の内部にある
            if(CheckPointInCollider(target.Data.position, collider))
            {
                Debug.Log("CenterCollision");
                //衝突判定がある
                return true;
            }

        }

        Debug.Log("NoCollision");
        //衝突判定がない
        return false;
    }

    /// <summary>
    /// <para>GetNearEdgeByOrigin</para>
    /// <para>対象のオブジェクトから相対的に最も近い頂点座標を取得します</para>
    /// </summary>
    /// <param name="target">原点</param>
    /// <param name="edges">頂点座標リスト</param>
    /// <returns>最も近い頂点座標</returns>
    private static Vector3 GetNearEdgeByOrigin(OriginalCollider target, Vector3[] edges)
    {
        //算出結果保存用
        float distance;
        //最も近い距離保存用　初期値として-1を格納する
        float minDistance = -1;

        //オブジェクト保存用
        Transform localObj = target.MyTransform;
        //オブジェクトの中心座標
        Vector3 origin = target.Data.position;
        //ローカル変換用
        Vector3 localEdge;

        //返却用
        Vector3 returnPos = default;

        //原点から最も近い頂点座標を検査
        foreach(Vector3 edge in edges)
        {
            //ローカル変換
            localEdge = localObj.InverseTransformPoint(edge);
            //距離算出
            distance = Vector3.Distance(origin, localEdge);
            //Debug.Log(distance + "dis " + localEdge + "local " + edge + "edge");
            //算出結果が保存されている距離より大きい または 初回でない 場合は何もしない
            if(minDistance < distance && minDistance != -1)
            {
                continue;
            }

            //距離更新
            minDistance = distance;
            //座標設定
            returnPos = edge;
        }
        //Debug.Log(returnPos + "edge ");
        //検査終了
        return returnPos;
    }

    /// <summary>
    /// <para>CheckPointInCollider</para>
    /// <para>検査対象座標がCollider内部にあるか検査します</para>
    /// </summary>
    /// <param name="check">検査対象座標</param>
    /// <param name="collider">検査対象Collider</param>
    /// <returns>内部判定</returns>
    private static bool CheckPointInCollider(Vector3 check, OriginalCollider collider)
    {
        //検査対象目線のローカル座標
        Vector3 localPos = collider.MyTransform.InverseTransformPoint(check);
        //Debug.Log(localPos + "local");

        //Debug.Log(localPos + "localedge");

        //Colliderの各次元毎に外側にいるかを判定する
        //Colliderの X軸 において外側である
        if (_collisionRange.x < localPos.x || localPos.x < -_collisionRange.x)
        {
            //内部にいない
            return false;
        }
        //Colliderの Y軸 において外側である
        if (_collisionRange.y < localPos.y || localPos.y < -_collisionRange.y)
        {
            //内部にいない
            return false;
        }
        //Colliderの Z軸 において外側である
        if (_collisionRange.z < localPos.z || localPos.z < -_collisionRange.z)
        {
            //内部にいない
            return false;
        }

        //内部である
        return true;
    }
    #endregion
}
