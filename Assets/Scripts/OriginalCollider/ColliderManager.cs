/// -----------------------------------------------------------------
/// ColliderManager.cs　Collider判定制御
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.Manager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using OriginalMath;
    using ColliderLibrary.DataManager;
    using ColliderLibrary.Collision;

    public class ColliderManager
    {
        #region 変数
        //基礎Vector情報保存用
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector2 _vector2Up = Vector2.up;
        private static readonly Vector2 _vector2Right = Vector2.right;

        //衝突判定範囲の最大値
        private static readonly float _collisionRange = GetTo.MaxRange;
        #endregion

        #region メソッド
        /// <summary>
        /// <para>CheckCollision</para>
        /// <para>Collider情報から衝突しているか検査します</para>
        /// </summary>
        /// <param name="collider">Collider情報</param>
        /// <returns>衝突判定</returns>
        public static CollisionData CheckCollision(ColliderData collider)
        {
            //検査対象に一番近い頂点座標
            Vector3 nearEdge;
            //その頂点座標のインデックス保存用
            int nearEdgeIndex;

            //共有リストから全Collider情報を取得し、衝突検査を行います
            foreach (ColliderData target in ColliderDataManager.ColliderInWorld)
            {
                //検査対象が 自身 である
                if (target.transform == collider.transform)
                {
                    //検査しない
                    continue;
                }

                //自身の中心座標 が 検査対象のCollider の内部にある
                if (CollisionCheck.CheckPointInCollider(collider.position, target))
                {
                    //衝突情報を返却する
                    return ReturnCollisionData(target, collider.position);
                }

                //検査対処の中心座標 が 自身のCollider の内部にある
                if (CollisionCheck.CheckPointInCollider(target.position, collider))
                {
                    //衝突情報を返却する
                    return ReturnCollisionData(target, target.position);
                }

                //自身の頂点座標 から 最も検査対象に近い頂点座標 を格納
                nearEdge = GetNearEdgeByCollider(target, collider.edgePos);
                //その頂点座標のインデックス取得
                nearEdgeIndex = Array.IndexOf(collider.edgePos, nearEdge);
                //その頂点から面上に別頂点へ結ぶことのできる線 が 検査対象のCollider に重なる
                if (CollisionCheck.CheckPlaneLineOverlap(nearEdgeIndex, collider.edgePos, target.transform))
                {
                    //衝突情報を返却する
                    return ReturnCollisionData(target, nearEdge);
                }

                //検査対象の頂点座標 から 最も自身に近い頂点座標 を格納
                nearEdge = GetNearEdgeByCollider(collider, target.edgePos);
                //その頂点座標のインデックス取得
                nearEdgeIndex = Array.IndexOf(target.edgePos, nearEdge);
                //その頂点から面上に別頂点へ結ぶことのできる線 が 自身のCollider に重なる
                if (CollisionCheck.CheckPlaneLineOverlap(nearEdgeIndex, target.edgePos, collider.transform))
                {
                    //衝突情報を返却する
                    return ReturnCollisionData(target, GetNearEdgeByCollider(target, collider.edgePos));
                }

            }

            //返却用
            CollisionData returnData = new();
            //返却用初期化
            returnData.flag = false;
            returnData.collider = default;
            returnData.point = _vectorZero;
            //空の衝突情報を返却する
            return returnData;
        }

        /// <summary>
        /// <para>ReturnCollisionData</para>
        /// <para>与えられた情報を衝突情報として変換します</para>
        /// </summary>
        /// <returns>衝突情報</returns>
        private static CollisionData ReturnCollisionData(ColliderData target, Vector3 point)
        {
            //返却用
            CollisionData returnData = new();

            //返却用設定
            returnData.flag = true;
            returnData.collider = target.transform;
            returnData.point = point;

            return returnData;
        }

        /// <summary>
        /// <para>GetNearEdgeByCollider</para>
        /// <para>対象のColliderから相対的に最も近い頂点座標を取得します</para>
        /// </summary>
        /// <param name="target">オブジェクト情報</param>
        /// <param name="edges">頂点座標リスト</param>
        /// <returns>最も近い頂点座標</returns>
        private static Vector3 GetNearEdgeByCollider(ColliderData target, Vector3[] edges)
        {
            //算出結果保存用
            float distance;
            //最も近い距離保存用　初期値として-1を格納する
            float minDistance = float.MaxValue;

            //オブジェクト保存用
            Transform localObj = target.transform;
            //ローカル変換用
            Vector3 localEdge;

            //返却用
            Vector3 returnPos = default;

            //原点から最も近い頂点座標を検査
            foreach (Vector3 edge in edges)
            {
                //ローカル変換
                localEdge = localObj.InverseTransformPoint(edge);
                //距離算出
                distance = Vector3.Distance(_vectorZero, localEdge);

                //算出結果が保存されている距離より大きい または 初回でない 場合は何もしない
                if (minDistance < distance)
                {
                    continue;
                }

                //距離更新
                minDistance = distance;
                //座標設定
                returnPos = edge;
            }
            //検査終了
            return returnPos;
        }
        #endregion
    }
}
