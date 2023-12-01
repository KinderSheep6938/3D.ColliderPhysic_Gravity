/// -----------------------------------------------------------------
/// OriginalMath.cs 特殊計算管理用
/// 
/// 作成日：2023/11/17
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;


namespace OriginalMath
{
    using UnityEngine;

    //計算方法
    public enum Combine
    {
        Maximum,
        Minimum,
        Average,
        Multiplty
    }

    /// <summary>
    /// <para>GetTo</para>
    /// <para>特殊な計算等を行います</para>
    /// </summary>
    public class GetTo
    {
        #region 変数
        //二分割用定数
        private const int HALF = 2;
        //面の最大範囲
        private const float MAXRANGE = 0.5f;
        
        //基礎ベクトル
        private static readonly Vector2 _vectorZero = Vector2.zero;
        private static readonly Vector2 _vectorRight = Vector2.right;
        private static readonly Vector2 _vectorUp = Vector2.up;

        //返却用初期化リスト
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
        #endregion

        #region プロパティ
        public static float Half { get => HALF; }
        public static float MaxRange { get => MAXRANGE; }
        #endregion

        #region メソッド
        /// <summary>
        /// <para>ValueCombine</para>
        /// <para>与えられた値を設定された計算での結果を返します</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="combine">計算方法</param>
        /// <returns>計算結果</returns>
        public static float ValueCombine(float a,float b, Combine combine)
        {
            //設定された計算方法に基づいて値を返却
            switch (combine)
            {
                case Combine.Maximum:   //最大値
                    return Mathf.Max(a, b);

                case Combine.Minimum:   //最小値
                    return Mathf.Min(a, b);

                case Combine.Average:   //平均値
                    return (a + b) / HALF;

                case Combine.Multiplty: //掛け算
                    return (a * b);

                default:                //例外は0で返す
                    return 0;
                    
            }
        }

        /// <summary>
        /// <para>V2Projection</para>
        /// <para>対象ベクトルを地面ベクトルに対し、射影を行ったベクトルを出力します</para>
        /// <para>Vector2専用</para>
        /// </summary>
        /// <param name="target">対象ベクトル</param>
        /// <param name="ground">地面ベクトル</param>
        /// <returns>射影ベクトル</returns>
        public static Vector2 V2Projection(Vector2 target, Vector2 ground)
        {
            //射影分算出
            Vector2 multiTG = target * ground;
            Vector2 powG = ground * ground;
            Vector2 projection = multiTG / powG;
            //Debug.Log("Pro:" + multiTG + "|" + powG + "|" + projection);
            //計算
            Vector2 returnPro = ground * projection;

            return returnPro;
        }

        /// <summary>
        /// <para>V3Projection</para>
        /// <para>対象ベクトルを地面ベクトルに対し、射影を行ったベクトルを出力します</para>
        /// <para>Vector3専用</para>
        /// </summary>
        /// <param name="target">対象ベクトル</param>
        /// <param name="ground">地面ベクトル</param>
        /// <returns>射影ベクトル</returns>
        public static Vector3 V3Projection(Vector3 target, Vector3 ground)
        {
            //射影分算出
            float multiTG = Vector3.Dot(target,ground);
            float powG = Vector3.Dot(ground,ground);
            float projection = multiTG / powG;
            //Debug.Log("Pro:" + multiTG + "|" + powG + "|" + projection);
            //計算
            Vector3 returnPro = ground * projection;

            return returnPro;
        }

        /// <summary>
        /// <para>GetSlopeByStartToOrigin</para>
        /// <para>ある点から中心に向かう基礎傾きを取得します</para>
        /// </summary>
        /// <param name="point">対象となる点</param>
        /// <returns>中心への基礎傾き</returns>
        public static Vector2 SlopeByPointToOrigin(Vector2 point)
        {
            //返却用
            Vector2 returnSlope = _vectorZero;

            //横軸
            //範囲より上か
            if (MAXRANGE < point.x)
            {
                returnSlope += -_vectorRight;
            }
            //範囲より下か
            else if (point.x < -MAXRANGE)
            {
                returnSlope += _vectorRight;
            }

            //縦軸
            //範囲より上か
            if (MAXRANGE < point.y)
            {
                returnSlope += -_vectorUp;
            }
            //範囲より下か
            else if (point.y < -MAXRANGE)
            {
                returnSlope += _vectorUp;
            }

            //返却
            return returnSlope;
        }

        /// <summary>
        /// <para>GetPlaneEdgeByPoint</para>
        /// <para>面の頂点座標を取得します</para>
        /// </summary>
        /// <param name="point">検査座標</param>
        /// <returns>検査座標に対する必要な頂点座標リスト</returns>
        public static Vector2[] PlaneEdgeByPoint(Vector2 point)
        {
            //返却用
            Vector2[] returnEdges = _resetReturnList;

            //面から見た検査座標がどの位置に存在するかによって頂点座標を変更する

            //検査座標が 右上 または 左下 にある
            if ((MAXRANGE < point.x && MAXRANGE < point.y)
                || (point.x < -MAXRANGE && point.y < -MAXRANGE))
            {
                //左上と右下を返却
                returnEdges[0] = _planeEdge[EdgeByPlane.lU];
                returnEdges[1] = _planeEdge[EdgeByPlane.rD];
            }
            //検査座標が 右下 または 左上 にある
            else if ((MAXRANGE < point.x && point.y < -MAXRANGE)
                || (point.x < -MAXRANGE && MAXRANGE < point.y))
            {
                //右上と左下を返却
                returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                returnEdges[1] = _planeEdge[EdgeByPlane.lD];
            }
            //検査対象が 横幅の範囲内 である
            else if (-MAXRANGE <= point.x && point.x <= MAXRANGE)
            {
                //検査対象が 上側 にある
                if (MAXRANGE < point.y)
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
                if (MAXRANGE < point.x)
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
        #endregion
    }
}
