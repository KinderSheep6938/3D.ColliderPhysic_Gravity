/// -----------------------------------------------------------------
/// ColliderManager.cs　Collider判定制御
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColliderLibrary.Manager
{
    using VectorMath;

    public class ColliderManager
    {
        #region 変数
        //基礎Vector情報保存用
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector2 _vector2Up = Vector2.up;
        private static readonly Vector2 _vector2Right = Vector2.right;

        //衝突判定範囲の最大値
        private static readonly float _collisionRange = GetTo.MaxRange;

        //Collider情報共有用
        private static List<ColliderData> _worldInColliders = new();

        #endregion

        #region プロパティ

        #endregion

        #region メソッド
        /// <summary>
        /// <para>SetColliderToWorld</para>
        /// <para>対象のCollider情報を共有リストに設定します</para>
        /// </summary>
        /// <param name="target">Collider情報</param>
        public static void SetColliderToWorld(ColliderData target)
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
        public static void RemoveColliderToWorld(ColliderData target)
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
        public static CollisionData CheckCollision(ColliderData collider)
        {
            //返却用
            CollisionData returnData = new();
            //返却用の衝突を設定
            returnData.collision = true;

            //検査対象に一番近い頂点座標
            Vector3 nearEdge;
            //その頂点座標のインデックス保存用
            int nearEdgeIndex;

            //共有リストから全Collider情報を取得し、衝突検査を行います
            foreach (ColliderData target in _worldInColliders)
            {
                //検査対象が 自身 である
                if (target.transform == collider.transform)
                {
                    //検査しない
                    continue;
                }
                //衝突相手を設定
                returnData.collider = target;

                //自身の中心座標 が 検査対象のCollider の内部にある
                if (CheckPointInCollider(collider.position, target))
                {
                    //Debug.Log("MyCenterCollision");
                    returnData.point = collider.position;
                    //衝突判定がある
                    return returnData;
                }

                //検査対処の中心座標 が 自身のCollider の内部にある
                if (CheckPointInCollider(target.position, collider))
                {
                    //Debug.Log("CenterCollision");
                    returnData.point = target.position;
                    //衝突判定がある
                    return returnData;
                }

                //自身の頂点座標 から 最も検査対象に近い頂点座標 を格納
                nearEdge = GetNearEdgeByCollider(target, collider.edgePos);
                //その頂点座標のインデックス取得
                nearEdgeIndex = Array.IndexOf(collider.edgePos, nearEdge);
                //Debug.Log(nearEdgeIndex + "myNearIndex");
                //自身の最も近い頂点から面上に結ぶことのできる線 が 検査対象のCollider に重なる
                if (CheckPlaneLineOverlap(nearEdgeIndex, collider.edgePos, target.transform))
                {
                    //Debug.Log("LineCollision ; " + target.name);
                    returnData.point = nearEdge;
                    //衝突判定がある
                    return returnData;
                }

                //検査対象の頂点座標 から 最も自身に近い頂点座標 を格納
                nearEdge = GetNearEdgeByCollider(collider, target.edgePos);
                //その頂点座標のインデックス取得
                nearEdgeIndex = Array.IndexOf(target.edgePos, nearEdge);
                //Debug.Log(nearEdgeIndex + "NearIndex");
                //自身の最も近い頂点から面上に結ぶことのできる線 が 検査対象のCollider に重なる
                if (CheckPlaneLineOverlap(nearEdgeIndex, target.edgePos, collider.transform))
                {
                    //Debug.Log("ColliderLineCollision : " + target.name);
                    returnData.point = nearEdge;
                    //衝突判定がある
                    return returnData;
                }

            }

            //返却用初期化
            returnData.collision = false;
            returnData.collider = default;
            returnData.point = _vectorZero;
            //Debug.Log("NoCollision");
            //衝突判定がない
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
                //Debug.Log(edge + ":" + localEdge + ":" + distance);
                //Debug.Log(distance + "dis " + localEdge + "local " + edge + "edge");
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
        private static bool CheckPointInCollider(Vector3 check, ColliderData collider)
        {
            //検査対象目線のローカル座標
            Vector3 localPos = collider.transform.InverseTransformPoint(check);
            //Debug.Log(localPos + "local");

            //Debug.Log(localPos + "localedge");

            //Colliderの各次元毎に外側にいるかを判定する
            //Colliderの X軸 において外側である
            if (_collisionRange < localPos.x || localPos.x < -_collisionRange)
            {
                //内部にいない
                return false;
            }
            //Colliderの Y軸 において外側である
            if (_collisionRange < localPos.y || localPos.y < -_collisionRange)
            {
                //内部にいない
                return false;
            }
            //Colliderの Z軸 において外側である
            if (_collisionRange < localPos.z || localPos.z < -_collisionRange)
            {
                //内部にいない
                return false;
            }

            //内部である
            return true;
        }

        /// <summary>
        /// <para>CheckPlaneLineOverlap</para>
        /// <para>頂点に対して面上で結ぶことのできる線がColliderに重なっているか検査します</para>
        /// </summary>
        /// <param name="edge">指定する頂点</param>
        /// <param name="edgePos">頂点座標</param>
        /// <param name="collider">対象Collider</param>
        /// <returns></returns>
        private static bool CheckPlaneLineOverlap(int edge, Vector3[] edgePos, Transform collider)
        {
            //頂点に対して、面上に結ぶことのできる頂点分検査します
            foreach (int lineEdge in EdgeLineManager.GetEdgeFromPlaneLine(edge))
            {
                //Debug.Log(lineEdge + "lineEdge" + edgePos[lineEdge]);
                //頂点と各対象の頂点を結ぶ線 が Collider に重なるか検査
                if (CheckLineOverlapByCollider(edgePos[edge], edgePos[lineEdge], collider))
                {
                    //重なっていると判定
                    return true;
                }
                //Debug.Log("--------------------------");
            }

            //重なっていないと判定
            return false;
        }

        /// <summary>
        /// <para>CheckLineOverlapByCollider</para>
        /// <para>線がColliderに重なるか検査します</para>
        /// </summary>
        /// <param name="startPoint">線の始点</param>
        /// <param name="endPoint">線の終点</param>
        /// <param name="collider">対象Collider</param>
        /// <returns>重なり判定</returns>
        private static bool CheckLineOverlapByCollider(Vector3 startPoint, Vector3 endPoint, Transform collider)
        {
            //Debug.Log(startPoint + " SP" + endPoint + " EP");
            //線の始点と終点をローカル変換
            Vector3 localStart = collider.InverseTransformPoint(startPoint);
            Vector3 localEnd = collider.InverseTransformPoint(endPoint);

            //各次元ごとの面で判定を行う
            //Z面（X軸 と Y軸）
            Vector2 vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.y;
            Vector2 vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.y;
            //Debug.Log("Z:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
            //面に重ならない場合は、Colliderに重ならないと判定する
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            //X面（Y軸 と Z軸）
            vec2Start = _vector2Right * localStart.z + _vector2Up * localStart.y;
            vec2End = _vector2Right * localEnd.z + _vector2Up * localEnd.y;
            //Debug.Log("X:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
            //面が重ならない場合は、Colliderに重ならないと判定する
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            //Y面（X軸 と Z軸）
            vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.z;
            vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.z;
            //Debug.Log("Y:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
            //面が重ならない場合は、Colliderに重ならないと判定する
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// <para>CheckLineOverlapByPlane</para>
        /// <para>線が面に重なるか検査します</para>
        /// </summary>
        /// <param name="startPoint">線の始点</param>
        /// <param name="endPoint">線の終点</param>
        /// <returns>重なり判定</returns>
        private static bool CheckLineOverlapByPlane(Vector2 startPoint, Vector2 endPoint)
        {
            //線の始点と終点で、片方でも面上にある場合は、面に重なると判定する
            //始点が面上にある
            if (CheckPointInPlane(startPoint))
            {
                return true;
            }
            //終点が面上にある
            if (CheckPointInPlane(endPoint))
            {
                return true;
            }

            //線が面に重なるか検査を始める
            //初めに検査に必要な頂点座標を取得する
            Vector2[] edges = GetTo.PlaneEdgeByPoint(startPoint);
            //線分の傾きが始点から各頂点座標を結ぶ線の傾き以内である場合は、重なると判定する
            if (CheckLineSlopeByPlane(startPoint, endPoint, edges))
            {
                return true;
            }

            //重ならない
            return false;

        }

        /// <summary>
        /// <para>CheckPointInPlane</para>
        /// <para>検査座標が面上にあるか検査します</para>
        /// </summary>
        /// <param name="point">検査座標</param>
        /// <returns>面上判定</returns>
        private static bool CheckPointInPlane(Vector2 point)
        {
            //座標が面の横幅外である
            if (point.x < -_collisionRange || _collisionRange < point.x)
            {
                return false;
            }

            //座標が面の縦幅外である
            if (point.y < -_collisionRange || _collisionRange < point.y)
            {
                return false;
            }

            //範囲内である
            return true;
        }

        /// <summary>
        /// <para>CheckLineSlopeByEdge</para>
        /// <para>始点と終点を結ぶ線の傾き が 面に当たる傾き であるか検査します</para>
        /// </summary>
        /// <param name="start">線の始点</param>
        /// <param name="end">線の終点</param>
        /// <param name="edges">頂点座標リスト</param>
        /// <returns>範囲内判定</returns>
        private static bool CheckLineSlopeByPlane(Vector2 start, Vector2 end, Vector2[] edges)
        {
            //傾き検査を通る かつ 線の大きさが面に重なるか
            if (CheckSlopeByEdgeSlope(start, end, edges) && CheckSlopeOverlapPlane(start, end))
            {
                //範囲内である
                return true;
            }

            //範囲外である
            return false;
        }

        /// <summary>
        /// <para>CheckSlopeByEdgeSlope</para>
        /// <para>対象ベクトルの傾きが頂点ベクトルの傾きの範囲内・間に存在するか検査します</para>
        /// </summary>
        /// <param name="lineSlope">対象ベクトル</param>
        /// <param name="edges">頂点座標</param>
        /// <param name="edgeMaxSlope">頂点ベクトルの最大値の傾き</param>
        /// <param name="edgeMinSlope">頂点ベクトルの最小値の傾き</param>
        /// <returns>範囲内判定</returns>
        private static bool CheckSlopeByEdgeSlope(Vector2 start, Vector2 end, Vector2[] edges)
        {
            //傾きを算出
            Vector2 lineSlope = end - start;
            Vector2 edgeSlope1 = edges[0] - start;
            Vector2 edgeSlope2 = edges[1] - start;
            //Debug.Log(lineSlope + " lS" + edgeSlope1 + " eS1" + edgeSlope2 + " eS2");

            //各ベクトル成分を正規化
            lineSlope = lineSlope.normalized;
            edgeSlope1 = edgeSlope1.normalized;
            edgeSlope2 = edgeSlope2.normalized;

            //始点と頂点座標を結ぶ線の傾きにおいて 各成分の最大値・最小値 を取り出し
            Vector2 edgeMaxSlope = _vectorZero;
            Vector2 edgeMinSlope = _vectorZero;
            //頂点座標ベクトルの傾きのX軸成分
            if (edgeSlope1.x < edgeSlope2.x)
            {
                edgeMaxSlope += _vector2Right * edgeSlope2.x;
                edgeMinSlope += _vector2Right * edgeSlope1.x;
            }
            else
            {
                edgeMaxSlope += _vector2Right * edgeSlope1.x;
                edgeMinSlope += _vector2Right * edgeSlope2.x;
            }
            //頂点座標ベクトルの傾きのY軸成分
            if (edgeSlope1.y < edgeSlope2.y)
            {
                edgeMaxSlope += _vector2Up * edgeSlope2.y;
                edgeMinSlope += _vector2Up * edgeSlope1.y;
            }
            else
            {
                edgeMaxSlope += _vector2Up * edgeSlope1.y;
                edgeMinSlope += _vector2Up * edgeSlope2.y;
            }

            //Debug.Log("Nomal: " + lineSlope + " lS" + edgeMaxSlope + " eMaS" + edgeMinSlope + " eMiS");

            //範囲検査 ---------------------------------------------------------------------------------
            ////方向の正負判定用（true:正 false:負）
            //bool direSign;
            //Debug.Log("edge:" + edges[0] + "|" + edges[1]);
            //頂点座標がお互いに同じ軸線上に存在しないか
            if (edges[0].x != edges[1].x && edges[0].y != edges[1].y)
            {
                //Debug.Log("NoXY");
                //対象ベクトルの傾きが最大値・最小値の範囲内である
                if ((edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x)
                    && (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y))
                {
                    //範囲内である
                    return true;
                }
                //範囲外である
                return false;
            }
            //頂点座標が同じX軸上に存在する
            else if (edges[0].x == edges[1].x)
            {
                //Debug.Log("X");
                //対象ベクトルの傾きのY軸が最大値・最小値の範囲内であるか
                if (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y)
                {
                    //範囲内である
                    return true;
                }
                //範囲外である
                return false;
            }
            //頂点座標が同じY軸上に存在する
            else
            {
                //Debug.Log("Y");
                //対象ベクトルの傾きのY軸が最大値・最小値の範囲外であるか
                if (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x)
                {
                    //範囲内である
                    return true;
                }
                //範囲外である
                return false;
            }
        }

        /// <summary>
        /// <para>CheckSlopeOverlapPlane</para>
        /// <para>始点と終点を結ぶ線の傾きが面に重なるか検査します</para>
        /// </summary>
        /// <param name="start">線の始点</param>
        /// <param name="end">線の終点</param>
        /// <returns>重なり判定</returns>
        private static bool CheckSlopeOverlapPlane(Vector2 start, Vector2 end)
        {
            //Debug.Log(start + "|" + end);
            //始点から見た面への基礎ベクトルを取得
            Vector2 slopeDire = GetTo.SlopeByPointToOrigin(start);
            //始点から面に当たる最小傾き分を算出
            //Debug.Log(start + " - " + GetProjection(slopeDire, start));
            Vector2 centorMinSlope = start - GetTo.Projection(slopeDire, start);
            //始点から終点への傾きを中心方向へ射影
            Vector2 centorSlope = end - start;

            //始点から見た中心への方向正負判定
            bool xDire = 0 < -start.x;
            bool yDire = 0 < -start.y;
            //それぞれの傾きが最小値を超えるか（同値でも超えると判定する）
            bool xSlope = (xDire && centorMinSlope.x <= centorSlope.x) || (!xDire && centorSlope.x <= centorMinSlope.x);
            bool ySlope = (yDire && centorMinSlope.y <= centorSlope.y) || (!yDire && centorSlope.y <= centorMinSlope.y);

            //Debug.Log(centorMinSlope + "|" + centorSlope + "|" + GetProjection(_vector2Right, _vector2Right + _vector2Up));
            //Debug.Log(xDire + ":" + yDire);
            //始点が面から見て斜めの位置である
            if (slopeDire.x != 0 && slopeDire.y != 0)
            {
                //Debug.Log("toXY");
                //始点から終点への線が最小傾き以上であるか
                if (xSlope && ySlope)
                {
                    return true;
                }
            }
            //始点が面から見てX軸方向にある
            else if (slopeDire.x != 0)
            {
                //Debug.Log("toX");
                //X軸において、始点から終点への線が最小傾き以上であるか
                if (xSlope)
                {
                    return true;
                }
            }
            //始点が面から見てY軸方向にある
            else
            {
                //Debug.Log("toY");
                //Y軸において、始点から終点への線が最小傾き以上であるか
                if (ySlope)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
