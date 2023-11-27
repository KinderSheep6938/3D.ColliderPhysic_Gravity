using System.Collections;
using System.Collections.Generic;


namespace ColliderLibrary.Manager
{
    using UnityEngine;
    using OriginalMath;
    using ColliderLibrary;
    using ColliderLibrary.DataManager;
    using ColliderLibrary.Collision;

    #region Cast基本データ
    //Castの返却データ構造
    public struct CastHit
    {
        public bool collision;
        public ColliderData collider;
        public Vector3 point;
    }
    #endregion

    public class RaycastManager
    {
        #region 変数
        //基礎ベクトル
        private static readonly Vector3 _vectorZero = Vector3.zero;
        #endregion

        #region メソッド
        public static CastHit Raycast(Vector3 origin, Vector3 vector, float length)
        {
            //方向ベクトルを正規化
            Vector3 norVector = vector.normalized;
            //照射点に距離分の正規化した方向ベクトルを加算したものを終点とする
            Vector3 lineEnd = origin + (norVector * length);

            //衝突判定
            bool collision = false;
            //原点から頂点に対する距離ベクトル保存用
            Vector3 originToEdge = _vectorZero;
            //算出距離保存用
            float colliderLength = 0;
            //最短距離保存用
            float minLength = float.MaxValue;
            //Collider返却用変数
            ColliderData returnCollider = new();
            //衝突地点保存用
            Vector3 point = _vectorZero;

            //一番近い線が当たるColliderを取得
            foreach(ColliderData collider in ColliderDataManager.ColliderInWorld)
            {
                //Raycastの始点と終点を結ぶ線に重ならない
                if (!CollisionCheck.CheckLineOverlapByCollider(origin, lineEnd, collider.transform))
                {
                    //処理をスキップ
                    continue;
                }

                //原点と一番原点に近い頂点との距離ベクトルを算出
                originToEdge = GetNearEdgeByOrigin(origin, collider.edgePos) - origin;
                //距離ベクトルをRaycastベクトルに射影
                originToEdge = GetTo.V3Projection(originToEdge, lineEnd - origin);

                //射影ベクトルの距離を算出
                colliderLength = Vector3.Distance(origin, origin + originToEdge);
                //設定されている最小距離より短い
                if (colliderLength <= minLength)
                {
                    //最小距離を設定
                    minLength = colliderLength;
                    //Collider情報を設定
                    returnCollider = collider;
                    //衝突地点を設定
                    point = origin + originToEdge;
                    //衝突判定設定
                    collision = true;
                }
            }

            //返却用
            CastHit returnHit = new();
            //返却用値設定
            returnHit.collision = collision;
            returnHit.collider = returnCollider;
            returnHit.point = point;

            return returnHit;
        }

        /// <summary>
        /// <para>GetNearEdgeByOrigin</para>
        /// <para>原点から最も近い頂点座標を取得します</para>
        /// </summary>
        /// <param name="origin">原点</param>
        /// <param name="edges">頂点座標リスト</param>
        /// <returns>最も近い頂点座標</returns>
        private static Vector3 GetNearEdgeByOrigin(Vector3 origin, Vector3[] edges)
        {
            //算出結果保存用
            float distance;
            //最も近い距離保存用　初期値として-1を格納する
            float minDistance = float.MaxValue;

            //返却用
            Vector3 returnPos = default;

            //原点から最も近い頂点座標を検査
            foreach (Vector3 edge in edges)
            {
                //距離算出
                distance = Vector3.Distance(origin, edge);

                //算出結果が保存されている距離より大きい場合は何もしない
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

