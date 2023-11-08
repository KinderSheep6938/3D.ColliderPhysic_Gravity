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

    //頂点座標識別用
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
            {EdgeData.flontRightUp,EdgeData.flontRightDown },
            {EdgeData.flontLeftUp,EdgeData.flontLeftDown },
            {EdgeData.flontRightUp,EdgeData.flontLeftUp },
            {EdgeData.flontRightDown,EdgeData.flontLeftDown },
            {EdgeData.flontRightUp,EdgeData.backRightUp },
            {EdgeData.flontRightDown,EdgeData.backRightDown },
            {EdgeData.flontLeftUp,EdgeData.backLeftUp },
            {EdgeData.flontLeftDown,EdgeData.backLeftDown },
            {EdgeData.backRightUp,EdgeData.backRightDown },
            {EdgeData.backLeftUp,EdgeData.backLeftDown },
            {EdgeData.backRightUp,EdgeData.backLeftUp },
            {EdgeData.backRightDown,EdgeData.backLeftDown }
        };
        //Collider描画線残留時間
        private const float LINE_VIEWTIME = 0.01f;

        //基礎Vector情報保存用
        private static readonly Vector3 _vector3Up = Vector3.up;
        private static readonly Vector3 _vector3Right = Vector3.right;
        private static readonly Vector3 _vector3Left = Vector3.left;
        private static readonly Vector3 _vector3Down = Vector3.down;
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

            //指定された頂点の座標を取得
            switch (edge)
            {
                //前方右上 ----------------------------------------------------------------
                case EdgeData.flontRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //前方右下 ----------------------------------------------------------------
                case EdgeData.flontRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //前方左上 ----------------------------------------------------------------
                case EdgeData.flontLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //前方左下 ----------------------------------------------------------------
                case EdgeData.flontLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //後方右上 ----------------------------------------------------------------
                case EdgeData.backRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //後方右下 ----------------------------------------------------------------
                case EdgeData.backRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //後方左上 ----------------------------------------------------------------
                case EdgeData.backLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //後方左下 ----------------------------------------------------------------
                case EdgeData.backLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //例外 --------------------------------------------------------------------
                default:
                    returnPos = Vector3.zero;
                    break;
            }

            return returnPos;
        }

        /// <summary>
        /// <para>CheckColliderFromTransform</para>
        /// <para>Colliderを更新する必要があるか検査します</para>
        /// </summary>
        /// <returns>更新可否判定</returns>
        public static bool CheckColliderFromTransform(Transform transform, ColliderData collider)
        {
            //Transform情報とCollider情報を比較し、変更があった場合は更新が必要であると判定します

            //座標比較
            if(transform.position != collider.position)
            {
                return true;
            }

            //角度比較
            if(transform.rotation.eulerAngles != collider.rotation)
            {
                return true;
            }

            //大きさ比較
            if(transform.localScale != collider.localScale)
            {
                return true;
            }

            //すべて同じである
            return false;
        }


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