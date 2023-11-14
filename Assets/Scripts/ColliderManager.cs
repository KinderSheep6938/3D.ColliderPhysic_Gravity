/// -----------------------------------------------------------------
/// ColliderEngine.cs　Collider判定制御
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;

public static class ColliderManager
{
    #region 変数
    //二分割用定数
    private const int HALF = 2;

    //基礎Vector情報保存用
    private static readonly Vector3 _vectorZero = Vector3.zero;
    private static readonly Vector3 _vector3Up = Vector3.up;
    private static readonly Vector3 _vector3Right = Vector3.right;
    private static readonly Vector3 _vector3Flont = Vector3.forward;
    private static readonly Vector2 _vector2Up = Vector2.up;
    private static readonly Vector2 _vector2Right = Vector2.right;

    //衝突判定範囲の最大値
    private static readonly Vector3 _collisionRange = Vector3.one / HALF;

    //リスト初期化用
    private static readonly Vector2[] _resetReturnList = new Vector2[2];

    //面における頂点座標
    private static readonly Vector2[] _planeEdge =
    {
        new Vector2(0.5f,0.5f),         //右上
        new Vector2(0.5f,-0.5f),        //右下
        new Vector2(-0.5f,0.5f),        //左上
        new Vector2(-0.5f,-0.5f)        //左下
    };
    //面における頂点座標のID（上記の頂点座標に対応）
    private struct EdgeByPlane
    {
        public const int rU = 0;        //右上
        public const int rD = 1;        //右下
        public const int lU = 2;        //左上
        public const int lD = 3;        //左下
    }

    //Collider情報共有用
    private static List<OriginalCollider> _worldInColliders = new();

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
        //その頂点座標のインデックス保存用
        int nearEdgeIndex;

        //共有リストから全Collider情報を取得し、衝突検査を行います
        foreach(OriginalCollider target in _worldInColliders)
        {
            //検査対象が 自身 である
            if(target == collider)
            {
                //検査しない
                continue;
            }

            //自身の頂点座標 から 最も検査対象に近い頂点座標 を格納
            nearEdge = GetNearEdgeByCollider(target, collider.Data.edgePos);
            //その頂点座標のインデックス取得
            nearEdgeIndex = Array.IndexOf(collider.Data.edgePos, nearEdge);
            //Debug.Log(nearEdgeIndex + "nearindex");
            //自身の最も近い頂点から面上に結ぶことのできる線 が 検査対象のCollider に重なる
            if (CheckPlaneLineOverlap(nearEdgeIndex, collider.Data.edgePos, target))
            {
                Debug.Log("LineCollision ; " + target.name);
                //衝突判定がある
                return true;
            }

            //検査対象の頂点座標 から 最も自身に近い頂点座標 を格納
            nearEdge = GetNearEdgeByCollider(collider, target.Data.edgePos);
            //その頂点座標のインデックス取得
            nearEdgeIndex = Array.IndexOf(target.Data.edgePos, nearEdge);
            //Debug.Log(nearEdgeIndex + "nearindex");
            //自身の最も近い頂点から面上に結ぶことのできる線 が 検査対象のCollider に重なる
            if (CheckPlaneLineOverlap(nearEdgeIndex, target.Data.edgePos, collider))
            {
                Debug.Log("ColliderLineCollision : " + target.name);
                //衝突判定がある
                return true;
            }


            ////最も近い頂点座標 が 検査対象のCollider の内部にある
            //if (CheckPointInCollider(nearEdge, target))
            //{
            //    Debug.Log("MyCollision");
            //    //衝突判定がある
            //    return true;
            //}

            ////検査対象の頂点座標 から 最も近い頂点座標 を格納
            //nearEdge = GetNearEdgeByCollider(collider, target.Data.edgePos);
            ////最も近い頂点座標 が 自身のCollider の内部にある
            //if(CheckPointInCollider(nearEdge, collider))
            //{
            //    Debug.Log("Collision");
            //    return true;
            //}

            //自身の中心座標 が 検査対象のCollider の内部にある
            if (CheckPointInCollider(collider.Data.position, target))
            {
                Debug.Log("MyCenterCollision");
                //衝突判定がある
                return true;
            }

            //検査対処の中心座標 が 自身のCollider の内部にある
            if (CheckPointInCollider(target.Data.position, collider))
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
    /// <para>GetNearEdgeByCollider</para>
    /// <para>対象のColliderから相対的に最も近い頂点座標を取得します</para>
    /// </summary>
    /// <param name="target">オブジェクト情報</param>
    /// <param name="edges">頂点座標リスト</param>
    /// <returns>最も近い頂点座標</returns>
    private static Vector3 GetNearEdgeByCollider(OriginalCollider target, Vector3[] edges)
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

    /// <summary>
    /// <para>CheckPlaneLineOverlap</para>
    /// <para>頂点に対して面上で結ぶことのできる線がColliderに重なっているか検査します</para>
    /// </summary>
    /// <param name="edge">指定する頂点</param>
    /// <param name="edgePos">頂点座標</param>
    /// <param name="collider">対象Collider</param>
    /// <returns></returns>
    private static bool CheckPlaneLineOverlap(int edge, Vector3[] edgePos, OriginalCollider collider)
    {
        //頂点に対して、面上に結ぶことのできる頂点分検査します
        foreach(int lineEdge in EdgeLineManager.GetEdgeFromPlaneLine(edge))
        {
            //Debug.Log(lineEdge + "lineEdge");
            //頂点と各対象の頂点を結ぶ線 が Collider に重なるか検査
            if(CheckLineOverlapByCollider(edgePos[edge], edgePos[lineEdge], collider))
            {
                //重なっていると判定
                return true;
            }
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
    private static bool CheckLineOverlapByCollider(Vector3 startPoint, Vector3 endPoint, OriginalCollider collider)
    {
        //Debug.Log(startPoint + " SP" + endPoint + " EP");
        //線の始点と終点をローカル変換
        Vector3 localStart = collider.MyTransform.InverseTransformPoint(startPoint);
        Vector3 localEnd = collider.MyTransform.InverseTransformPoint(endPoint);

        //各次元ごとの面で判定を行う
        //Z面（X軸 と Y軸）
        Vector2 vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.y;
        Vector2 vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.y;
        Debug.Log("Z:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
        //面に重ならない場合は、Colliderに重ならないと判定する
        if (!CheckLineOverlapByPlane(vec2Start, vec2End))
        {
            return false;
        }
        //X面（Y軸 と Z軸）
        vec2Start = _vector2Right * localStart.z + _vector2Up * localStart.y;
        vec2End = _vector2Right * localEnd.z + _vector2Up * localEnd.y;
        Debug.Log("X:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
        //面が重ならない場合は、Colliderに重ならないと判定する
        if (!CheckLineOverlapByPlane(vec2Start, vec2End))
        {
            return false;
        }
        //Y面（X軸 と Z軸）
        vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.z;
        vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.z;
        Debug.Log("Y:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
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
        Vector2[] edges = GetPlaneEdgeByPoint(startPoint);
        //線分の傾きが始点から各頂点座標を結ぶ線の傾き以内である場合は、重なると判定する
        if (CheckLineSlopeByEdge(startPoint, endPoint, edges))
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
        if(point.x < -_collisionRange.x || _collisionRange.x < point.x)
        {
            return false;
        }

        //座標が面の縦幅外である
        if (point.y < -_collisionRange.y || _collisionRange.y < point.y)
        {
            return false;
        }

        //範囲内である
        return true;
    }

    /// <summary>
    /// <para>GetPlaneEdgeByPoint</para>
    /// <para>面の頂点座標を取得します</para>
    /// </summary>
    /// <param name="point">検査座標</param>
    /// <returns>検査座標に対する必要な頂点座標リスト</returns>
    private static Vector2[] GetPlaneEdgeByPoint(Vector2 point)
    {
        //返却用
        Vector2[] returnEdges = _resetReturnList;

        //面から見た検査座標がどの位置に存在するかによって頂点座標を変更する
        
        //検査座標が 右上 または 左下 にある
        if((_collisionRange.x < point.x && _collisionRange.y < point.y)
            || (point.x < -_collisionRange.x && point.y < -_collisionRange.y ))
        {
            //左上と右下を返却
            returnEdges[0] = _planeEdge[EdgeByPlane.lU];
            returnEdges[1] = _planeEdge[EdgeByPlane.rD];
        }
        //検査座標が 右下 または 左上 にある
        else if((_collisionRange.x < point.x && point.y < -_collisionRange.y)
            || (point.x < -_collisionRange.x && _collisionRange.y < point.y))
        {
            //右上と左下を返却
            returnEdges[0] = _planeEdge[EdgeByPlane.rU];
            returnEdges[1] = _planeEdge[EdgeByPlane.lD];
        }
        //検査対象が 横幅の範囲内 である
        else if(-_collisionRange.x <= point.x && point.x <= _collisionRange.x)
        {
            //検査対象が 上側 にある
            if(_collisionRange.y < point.y)
            {
                //右上と左上を返却
                returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                returnEdges[1] = _planeEdge[EdgeByPlane.lU];
            }
            //下側 にある
            else
            {
                //右下と左下を返却
                returnEdges[0] = _planeEdge[EdgeByPlane.rD];
                returnEdges[1] = _planeEdge[EdgeByPlane.lD];
            }
        }
        //検査対象が 縦幅の範囲内 である
        else
        {
            //検査対象が 右側 にある
            if (_collisionRange.x < point.x)
            {
                //右上と右下を返却
                returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                returnEdges[1] = _planeEdge[EdgeByPlane.rD];
            }
            //左側 にある
            else
            {
                //左上と左下を返却
                returnEdges[0] = _planeEdge[EdgeByPlane.lU];
                returnEdges[1] = _planeEdge[EdgeByPlane.lD];
            }
        }

        //返却
        return returnEdges;
    }

    /// <summary>
    /// <para>CheckLineSlopeByEdge</para>
    /// <para>始点と終点を結ぶ線の傾き が 始点と各頂点座標を結ぶ線の傾き内 であるか検査します</para>
    /// </summary>
    /// <param name="start">線の始点</param>
    /// <param name="end">線の終点</param>
    /// <param name="edges">頂点座標リスト</param>
    /// <returns>範囲内判定</returns>
    private static bool CheckLineSlopeByEdge(Vector2 start, Vector2 end, Vector2[] edges)
    {
        //傾きを算出
        Vector2 lineSlope = end - start;
        Vector2 edgeSlope1 = edges[0] - start;
        Vector2 edgeSlope2 = edges[1] - start;
        Debug.Log(lineSlope + " lS" + edgeSlope1 + " eS1" + edgeSlope2 + " eS2");

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

        Debug.Log("Nomal: " + lineSlope + " lS" + edgeMaxSlope + " eMaS" + edgeMinSlope + " eMiS");

        //頂点ベクトルが同じ軸に存在 かつ その軸とは別の線分ベクトルの傾き成分が頂点ベクトルの傾き成分の範囲内であるか
        bool vecJustX = (edges[0].x == edges[1].x) && (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y);
        bool vecJustY = (edges[0].y == edges[1].y) && (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x);

        //頂点ベクトルの傾きの強さの正負を取得
        bool vecXSign = edgeMinSlope.x < edgeMaxSlope.x;
        bool vecYSign = edgeMinSlope.y < edgeMaxSlope.y;

        //その状況下にて、その軸の線分ベクトルの傾き成分が頂点ベクトルの傾き成分以上であるか
        bool vecOverX = (vecXSign && edgeMinSlope.x <= lineSlope.x) || (!vecXSign && lineSlope.x <= edgeMinSlope.x);
        bool vecOverY = (vecYSign && edgeMinSlope.y <= lineSlope.y) || (!vecYSign && lineSlope.y <= edgeMinSlope.y);
        Debug.Log(edges[0] +":" + edges[1] + ":" + vecJustX + " " + vecXSign +" " + vecOverX + "|" + vecJustY + " " + vecXSign + " " + vecOverY);
        if((vecJustX && vecOverX) || (vecJustY && vecOverY))
        {
            //範囲内である
            return true;
        }
        
        //線分ベクトルの傾きが頂点座標ベクトルの傾きの範囲内であるか
        if (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x
            && edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y)
        {
            //範囲内である
            return true;
        }

        //範囲外である
        return false;
    }
    #endregion
}
