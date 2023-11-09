/// -----------------------------------------------------------------
/// OriginalColliderEditor.cs
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using UnityEngine;
using UnityEditor;


namespace OriginalColliderEditor
{
    #region 変数 - 共通
    //オブジェクト情報保存用
    [System.Serializable]
    public struct ColliderData
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 rotation;
        [SerializeField] public Vector3 localScale;
        [SerializeField] public Vector3[] edgePos;
    }

    //頂点座標共通識別用
    //前方 > 右側 > 上側 の関係でIDを振り分ける
    public struct EdgeData
    {
        public const int flontRightUp = 0;
        public const int flontRightDown = 1;
        public const int flontLeftUp = 2;
        public const int flontLeftDown = 3;
        public const int backRightUp = 4;
        public const int backRightDown = 5;
        public const int backLeftUp = 6;
        public const int backLeftDown = 7;
        public const int maxEdgeCnt = 8;
    }
    #endregion

    #region クラス
    public static class ColliderEditor
    {
        #region 変数
        //二分割用定数
        private const int HALF = 2;
        //最大頂点座標数
        private const int MAX_EDGE = EdgeData.maxEdgeCnt;
        //Collider描画処理用（オブジェクトの辺、頂点と頂点を結ぶ）
        private static readonly int[,] _drowLineIndex =
        {
            //前方４辺
            {EdgeData.flontRightUp,EdgeData.flontRightDown },
            {EdgeData.flontLeftUp,EdgeData.flontLeftDown },
            {EdgeData.flontRightUp,EdgeData.flontLeftUp },
            {EdgeData.flontRightDown,EdgeData.flontLeftDown },
            //前方から後方への４辺
            {EdgeData.flontRightUp,EdgeData.backRightUp },
            {EdgeData.flontRightDown,EdgeData.backRightDown },
            {EdgeData.flontLeftUp,EdgeData.backLeftUp },
            {EdgeData.flontLeftDown,EdgeData.backLeftDown },
            //後方４辺
            {EdgeData.backRightUp,EdgeData.backRightDown },
            {EdgeData.backLeftUp,EdgeData.backLeftDown },
            {EdgeData.backRightUp,EdgeData.backLeftUp },
            {EdgeData.backRightDown,EdgeData.backLeftDown }
        };
        //Collider描画線残留時間
        private const float LINE_VIEWTIME = 0.01f;

        //頂点座標の各次元の正負判別用
        private const int EDGE_JUDGE_AXISX = 0;
        private const int EDGE_JUDGE_AXISY = 0;
        private const int EDGE_JUDGE_AXISZ = 4;

        //基礎Vector情報保存用
        private static readonly Vector3 _vector3Up = Vector3.up;
        private static readonly Vector3 _vector3Right = Vector3.right;
        private static readonly Vector3 _vector3Flont = Vector3.forward;
        #endregion

        #region プロパティ

        #endregion

        #region メソッド
        /// <summary>
        /// <para>SetData</para>
        /// <para>対象の情報からCollider情報を取得します</para>
        /// </summary>
        /// <param name="targetObj">対象のオブジェクト</param>
        /// <returns>Collider情報</returns>
        public static ColliderData SetColliderDataByCube(Transform targetObj)
        {
            //返却用
            ColliderData returnData = new();

            //アクセスを簡略にする
            returnData.position = targetObj.position;
            returnData.rotation = targetObj.rotation.eulerAngles;
            returnData.localScale = targetObj.localScale;

            //オブジェクトの頂点座標設定
            returnData.edgePos = GetObjectEdgePos(returnData.position, returnData.localScale);

            Debug.Log("Create");
            return returnData;
        }

        /// <summary>
        /// <para>GetObjectEdgePos</para>
        /// <para>対象のオブジェクト情報から頂点座標を取得します</para>
        /// </summary>
        /// <param name="Origin">オブジェクトの中心</param>
        /// <param name="scale">オブジェクトの大きさ</param>
        /// <returns>頂点座標格納リスト</returns>
        private static Vector3[] GetObjectEdgePos(Vector3 Origin, Vector3 scale)
        {
            //返却用
            Vector3[] returnEdge = new Vector3[MAX_EDGE];

            //頂点座標取得
            returnEdge[EdgeData.flontRightUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontRightUp);
            returnEdge[EdgeData.flontRightDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontRightDown);
            returnEdge[EdgeData.flontLeftUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontLeftUp);
            returnEdge[EdgeData.flontLeftDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontLeftDown);
            returnEdge[EdgeData.backRightUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backRightUp);
            returnEdge[EdgeData.backRightDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backRightDown);
            returnEdge[EdgeData.backLeftUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backLeftUp);
            returnEdge[EdgeData.backLeftDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backLeftDown);

            return returnEdge;
        }

        /// <summary>
        /// <para>GetEdgeDistanceByScale</para>
        /// <para>指定された頂点座標を取得します</para>
        /// </summary>
        /// <param name="scale">オブジェクトの大きさ</param>
        /// <param name="edge">指定された頂点</param>
        /// <returns>指定された頂点座標</returns>
        private static Vector3 GetEdgeDistanceByScale(Vector3 scale, int edge)
        {
            //返却用
            Vector3 returnPos;

            //Scaleの半分を増加分として設定
            scale /= HALF;

            //各方向の差異を算出
            Vector3 scaleDisX = _vector3Right * scale.x * JudgeEdgeAxisX(edge);
            Vector3 scaleDisY = _vector3Up * scale.y * JudgeEdgeAxisY(edge);
            Vector3 scaleDisZ = _vector3Flont * scale.z * JudgeEdgeAxisZ(edge);

            //算出結果を合計する
            returnPos = scaleDisX + scaleDisY + scaleDisZ;

            return returnPos;
        }

        #region 頂点座標IDから各軸の正負を取得
        /// <summary>
        /// <para>JudgeEdgeAxisX</para>
        /// <para>指定された頂点座標のX軸の正負を取得します</para>
        /// </summary>
        /// <param name="edge">頂点座標</param>
        /// <returns>頂点座標のX軸値の正負</returns>
        private static int JudgeEdgeAxisX(int edge)
        {
            //右側を判定 （前方座標群である）
            bool isRight = (edge / HALF == EDGE_JUDGE_AXISX);

            //前述の判定を引継ぎ
            //右側を判定 かつ 後方座標群である
            isRight = isRight || ((edge - EDGE_JUDGE_AXISZ) / HALF == EDGE_JUDGE_AXISX && EDGE_JUDGE_AXISZ <= edge);

            //右方向である
            if (isRight)
            {
                return 1;
            }
            //左方向である
            return -1;
        }


        /// <summary>
        /// <para>JudgeEdgeAxisY</para>
        /// <para>指定された頂点座標のY軸の正負を取得します</para>
        /// </summary>
        /// <param name="edge">頂点座標</param>
        /// <returns>頂点座標のY軸値の正負</returns>
        private static int JudgeEdgeAxisY(int edge)
        {
            //上側を判定
            bool isUp = (edge % HALF == EDGE_JUDGE_AXISY);

            //上方向である
            if (isUp)
            {
                return 1;
            }
            //下方向である
            return -1;
        }

        /// <summary>
        /// <para>EdgeJudgeAxisZ</para>
        /// <para>指定された頂点座標のZ軸の正負を取得します</para>
        /// </summary>
        /// <param name="edge">頂点座標</param>
        /// <returns>頂点座標のZ軸値の正負</returns>
        private static int JudgeEdgeAxisZ(int edge)
        {
            //前方側を判定
            bool isFlont = (edge < EDGE_JUDGE_AXISZ);

            //前方である
            if (isFlont)
            {
                return 1;
            }
            //後方である
            return -1;
        }
        #endregion 

        /// <summary>
        /// <para>DrowCollider</para>
        /// <para>コライダーを描画します</para>
        /// </summary>
        /// <param name="edge"></param>
        public static void DrowCollider(ColliderData col)
        {
            //頂点座標
            Vector3[] edgePoss = col.edgePos;
            //描画リスト参照用
            int startPosIndex = 0;
            int endPosIndex = 1;
            //描画対象の頂点座標
            Vector3 startPos;
            Vector3 endPos;

            Debug.Log(edgePoss.Length + "----");
            //Colliderの辺を描画
            for(int lineIndex = 0;lineIndex < _drowLineIndex.GetLength(0); lineIndex++)
            {
                Debug.Log(lineIndex);
                //頂点座標を設定
                startPos = edgePoss[_drowLineIndex[lineIndex, startPosIndex]];
                endPos = edgePoss[_drowLineIndex[lineIndex, endPosIndex]];

                //描画
                Debug.DrawLine(startPos, endPos, Color.green,LINE_VIEWTIME);
            }
        }
        #endregion
    }
    #endregion
}