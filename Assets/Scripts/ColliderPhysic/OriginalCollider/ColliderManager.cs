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
    using ColliderLibrary.DataManager;
    using ColliderLibrary.Collision;
    using PhysicLibrary.Material;
    using PhysicLibrary.CollisionPhysic;

    /// <summary>
    /// <para>ColliderManager</para>
    /// <para>当たり判定を制御します</para>
    /// </summary>
    public class ColliderManager
    {
        #region 変数
        //基礎Vector情報保存用
        private static readonly Vector3 _vectorZero = Vector3.zero;
        #endregion

        #region メソッド
        /// <summary>
        /// <para>CheckCollision</para>
        /// <para>Collider情報から衝突しているか検査します</para>
        /// </summary>
        /// <param name="collider">Collider情報</param>
        /// <returns>衝突判定</returns>
        public static bool CheckCollision(ColliderData collider, Vector3 interpolate = default, bool saveCollision = true)
        {
            //検査対象に一番近い頂点座標
            Vector3 nearEdge;
            //その頂点座標のインデックス保存用
            int nearEdgeIndex;

            //データ一時保存用
            List<PhysicMaterials> saveTarget = new();
            saveTarget.Clear();
            List<int> saveEdge = new();
            saveEdge.Clear();
            List<Vector3> savePoint = new();
            savePoint.Clear();

            //共有リストから全Collider情報を取得し、衝突検査を行います
            foreach (ColliderData target in ColliderDataManager.GetColliderToWorld())
            {
                //検査対象が削除されている
                if(target.physic.transform == default)
                {
                    continue;
                }

                //検査対象が 自身 である
                if (target.physic.transform == collider.physic.transform)
                {
                    //検査しない
                    continue;
                }

                //自身の頂点座標 から 最も検査対象に近い頂点座標 を格納
                nearEdge = GetNearEdgeByCollider(target, collider.edgePos);
                //その頂点座標のインデックス取得
                nearEdgeIndex = Array.IndexOf(collider.edgePos, nearEdge);
                //自身の中心座標 が 検査対象のCollider の内部にある
                if (CollisionCheck.CheckPointInCollider(collider.position, target.physic.transform))
                {
                    //衝突情報を設定する
                    Save(target);
                    continue;
                }

                //検査対処の中心座標 が 自身のCollider の内部にある
                if (CollisionCheck.CheckPointInCollider(target.position, collider.physic.transform))
                {
                    //衝突情報を設定する
                    Save(target);
                    continue;
                }

                //その頂点から面上に別頂点へ結ぶことのできる線 が 検査対象のCollider に重なる
                if (CollisionCheck.CheckPlaneLineOverlap(nearEdgeIndex, collider.edgePos, target.physic.transform))
                {
                    //衝突情報を設定する
                    Save(target);
                    continue;
                }

                //検査対象の頂点座標 から 最も自身に近い頂点座標 を格納
                Vector3 nearTargetEdge = GetNearEdgeByCollider(collider, target.edgePos);
                //その頂点座標のインデックス取得
                int nearTargetEdgeIndex = Array.IndexOf(target.edgePos, nearTargetEdge);
                //その頂点から面上に別頂点へ結ぶことのできる線 が 自身のCollider に重なる
                if (CollisionCheck.CheckPlaneLineOverlap(nearTargetEdgeIndex, target.edgePos, collider.physic.transform))
                {
                    //衝突情報を設定する
                    Save(target);
                    continue;
                }

            }

            //設定された情報がない
            if (saveTarget.Count == 0)
            {
                //衝突判定がない
                return false;
            }
            //設定された情報がある
            else
            {
                //座標補完がある
                if (interpolate != default)
                {
                    RemoveInterpolate(interpolate, ref savePoint);
                }
                //衝突データを保存するか
                if (saveCollision)
                {
                    //衝突データを格納
                    SetCollisionData(collider.physic, saveTarget.ToArray(), saveEdge.ToArray(), savePoint.ToArray(), interpolate);
                }
                //衝突判定がある
                return true;
            }

            //リスト保存用ローカルメソッド
            void Save(ColliderData target)
            {
                //衝突情報を設定する
                saveTarget.Add(target.physic);
                saveEdge.Add(nearEdgeIndex);
                savePoint.Add(nearEdge);
            }
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
            Transform localObj = target.physic.transform;
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

        /// <summary>
        /// <para>RemoveInterpolate</para>
        /// <para>衝突地点の補完を除去します</para>
        /// </summary>
        /// <param name="interpolate">補完距離</param>
        /// <param name="point">登録された衝突地点</param>
        private static void RemoveInterpolate(Vector3 interpolate, ref List<Vector3> point)
        {
            //衝突が１つしかない場合
            if (point.Count == 1)
            {
                //補完削除
                point[0] -= interpolate;
                return;
            }
            //複数の補完を除去
            for (int i = 0; i < point.Count; i++)
            {
                //補完削除
                point[i] -= interpolate;
            }
            return;
        }

        /// <summary>
        /// <para>SetCollisionData</para>
        /// <para>衝突データを登録します</para>
        /// </summary>
        /// <param name="myPhysic">自身のPhysic</param>
        /// <param name="collisionPhysic">衝突のあった各Physic</param>
        /// <param name="point">各衝突地点</param>
        private static void SetCollisionData(PhysicMaterials myPhysic, PhysicMaterials[] collisionPhysic,int[] edgeId, Vector3[] point, Vector3 interpolate)
        {
            //補完速度はあるか
            bool interpolateFlag = (interpolate != _vectorZero);

            //衝突が１つしかない場合
            if (collisionPhysic.Length == 1)
            {
                //登録
                CollisionPhysicManager.SetCollision(myPhysic, collisionPhysic[0],edgeId[0] , point[0], interpolateFlag);
                return;
            }
            //複数の衝突を登録
            for (int i = 0; i < collisionPhysic.Length; i++)
            {
                //Debug.Log(collisionPhysic[i] + "[" + point[i] + "[" + interpolate);
                //登録
                CollisionPhysicManager.SetCollision(myPhysic, collisionPhysic[i],edgeId[i], point[i], interpolateFlag);
                //Debug.Log("1<<");
            }
            return;
        }
        #endregion
    }
}
