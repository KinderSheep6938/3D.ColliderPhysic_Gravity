/// -----------------------------------------------------------------
/// ColliderLibrary　Collider共通情報
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary
{
    using UnityEngine;
    using PhysicLibrary.Material;

    #region Collider情報
    //オブジェクト情報保存用
    [System.Serializable]
    public struct ColliderData
    {
        //材質データ
        [SerializeField, ReadOnly] public PhysicMaterials physic;
        //Transform情報
        [SerializeField, ReadOnly] public Vector3 position;
        [SerializeField, ReadOnly] public Quaternion rotation;
        [SerializeField, ReadOnly] public Vector3 localScale;
        //頂点座標保管
        [SerializeField, ReadOnly] public Vector3[] edgePos;
    }

    //頂点座標共通識別用
    //前方 > 右側 > 上側 の関係でIDを振り分ける
    public struct EdgeData
    {
        //各頂点
        public const int fRU    = 0; //前方右上
        public const int fRD    = 1; //前方右下
        public const int fLU    = 2; //前方左上
        public const int fLD    = 3; //前方左下
        public const int bRU    = 4; //後方右上
        public const int bRD    = 5; //後方右下
        public const int bLU    = 6; //後方左上
        public const int bLD    = 7; //後方左下
    }
    #endregion

    /// <summary>
    /// <para>EdgeLineManager</para>
    /// <para>頂点から結ぶことのできる線を管理します</para>
    /// </summary>
    public class EdgeLineManager
    {
        #region 変数
        //最大頂点座標数
        private const int MAX_EDGEINDEX = 7;
        //返却用配列初期化用
        private static readonly int[] _resetReturnList = new int[6];
        #endregion

        #region プロパティ
        //頂点座標データの最大インデックス
        public static int MaxEdge { get => MAX_EDGEINDEX + 1; }
        #endregion

        #region メソッド
        /// <summary>
        /// <para>GetEdgeFromPlaneLine</para>
        /// <para>指定した頂点に対して、面上で結ぶことのできる頂点を配列で取得する</para>
        /// </summary>
        /// <param name="edge">指定した頂点</param>
        /// <returns>面上で結ぶことのできる頂点座標配列</returns>
        public static int[] GetEdgeFromPlaneLine(int edge)
        {
            //返却用配列
            int[] returnEdges = _resetReturnList;
            //指定した頂点から真反対の頂点以外を格納
            int listIndex = 0;
            int checkEdge = -1;
            //頂点数分検査する
            while(checkEdge < MAX_EDGEINDEX)
            {
                //検査インデックス加算
                checkEdge++;
                //Debug.Log(edge + ":" + checkEdge + ":" + listIndex);
                //指定した頂点自身 または 真反対の頂点 である時は処理を戻す
                if (edge == checkEdge || MAX_EDGEINDEX - edge == checkEdge)
                {
                    continue;
                }
                //対象の頂点を格納
                returnEdges[listIndex] = checkEdge;
                listIndex++;
            }
            //Debug.Log("return" + returnEdges.Length) ;
            //返却
            return returnEdges;
        }
        #endregion
    }
}