/// -----------------------------------------------------------------
/// ColliderDataLibrary　Collider情報
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using UnityEngine;


namespace ColliderLibrary
{
    #region データ構造
    //オブジェクト情報保存用
    [System.Serializable]
    public struct ColliderData
    {
        //Transform情報
        [SerializeField,ReadOnly] public Vector3 position;
        [SerializeField,ReadOnly] public Quaternion rotation;
        [SerializeField,ReadOnly] public Vector3 localScale;
        //頂点座標保管
        [SerializeField,ReadOnly] public Vector3[] edgePos;
    }

    //頂点座標共通識別用
    //前方 > 右側 > 上側 の関係でIDを振り分ける
    public struct EdgeData
    {
        //各頂点
        public const int flontRightUp   = 0;
        public const int flontRightDown = 1;
        public const int flontLeftUp    = 2;
        public const int flontLeftDown  = 3;
        public const int backRightUp    = 4;
        public const int backRightDown  = 5;
        public const int backLeftUp     = 6;
        public const int backLeftDown   = 7;
        //最大頂点数
        public const int maxEdgeCnt     = 8;
    }
    #endregion
}