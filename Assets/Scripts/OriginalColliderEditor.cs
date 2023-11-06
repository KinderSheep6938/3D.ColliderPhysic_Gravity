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
        [SerializeField] public ColliderEdge edgePos;
    }

    //オブジェクトの頂点座標保存用
    [System.Serializable]
    public struct ColliderEdge
    {
        [SerializeField] public Vector3 flontRightUp;
        [SerializeField] public Vector3 flontRightDown;
        [SerializeField] public Vector3 flontLeftUp;
        [SerializeField] public Vector3 flontLeftDown;
        [SerializeField] public Vector3 backRightUp;
        [SerializeField] public Vector3 backRightDown;
        [SerializeField] public Vector3 backLeftUp;
        [SerializeField] public Vector3 backLeftDown;
    }
    #endregion

    #region クラス
    public static class ColliderEditor
    {
        #region 変数
        //二分割用定数
        private const int HALF = 2;

        //頂点座標識別用
        private enum Edge
        {
            flontRightUp,
            flontRightDown,
            flontLeftUp,
            flontLeftDown,
            backRightUp,
            backRightDown,
            backLeftUp,
            backLeftDown
        }
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
        /// <returns></returns>
        private static ColliderEdge GetObjectEdgePos(Vector3 Origin, Vector3 scale)
        {
            //返却用
            ColliderEdge returnEdge = new();

            //頂点座標取得
            returnEdge.flontRightUp = Origin + GetEdgeDistanceByScale(scale, Edge.flontRightUp);
            returnEdge.flontRightDown = Origin + GetEdgeDistanceByScale(scale, Edge.flontRightDown);
            returnEdge.flontLeftUp = Origin + GetEdgeDistanceByScale(scale, Edge.flontLeftUp);
            returnEdge.flontLeftDown = Origin + GetEdgeDistanceByScale(scale, Edge.flontLeftDown);
            returnEdge.backRightUp = Origin + GetEdgeDistanceByScale(scale, Edge.backRightUp);
            returnEdge.backRightDown = Origin + GetEdgeDistanceByScale(scale, Edge.backRightDown);
            returnEdge.backLeftUp = Origin + GetEdgeDistanceByScale(scale, Edge.backLeftUp);
            returnEdge.backLeftDown = Origin + GetEdgeDistanceByScale(scale, Edge.backLeftDown);

            return returnEdge;
        }

        /// <summary>
        /// <para>GetScaleEdgeDis</para>
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        private static Vector3 GetEdgeDistanceByScale(Vector3 scale, Edge edge)
        {
            //返却用
            Vector3 returnPos;

            //Scaleの半分を増加分として設定
            scale /= HALF;

            //指定された頂点の座標を取得
            switch (edge)
            {
                //前方右上 ----------------------------------------------------------------
                case Edge.flontRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //前方右下 ----------------------------------------------------------------
                case Edge.flontRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //前方左上 ----------------------------------------------------------------
                case Edge.flontLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //前方左下 ----------------------------------------------------------------
                case Edge.flontLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //後方右上 ----------------------------------------------------------------
                case Edge.backRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //後方右下 ----------------------------------------------------------------
                case Edge.backRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //後方左上 ----------------------------------------------------------------
                case Edge.backLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //後方左下 ----------------------------------------------------------------
                case Edge.backLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //例外 --------------------------------------------------------------------
                default:
                    returnPos = Vector3.zero;
                    break;
            }

            return returnPos;
        }
        #endregion
    }
    #endregion
}