/// -----------------------------------------------------------------
/// ColliderEditor.cs　Collider情報生成
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using UnityEngine;
using ColliderLibrary;

public static class ColliderEditor
{
    #region 変数
    //二分割用定数
    private const int HALF = 2;
    //最大頂点座標数
    private const int MAX_EDGE = EdgeData.maxEdgeCnt;

    //頂点座標の各次元の正負判別用
    private const int EDGE_JUDGE_AXISX = 0;
    private const int EDGE_JUDGE_AXISY = 0;
    private const int EDGE_JUDGE_AXISZ = 4;
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
        returnData.rotation = targetObj.rotation;
        returnData.localScale = targetObj.localScale;

        //オブジェクトの頂点座標設定
        returnData.edgePos = GetObjectEdgePos(targetObj);

        //生成完了
        return returnData;
    }

    /// <summary>
    /// <para>GetObjectEdgePos</para>
    /// <para>対象のオブジェクト情報から頂点座標を取得します</para>
    /// </summary>
    /// <param name="target">対象オブジェクト</param>
    /// <returns>頂点座標格納リスト</returns>
    private static Vector3[] GetObjectEdgePos(Transform target)
    {
        //返却用
        Vector3[] returnEdge = new Vector3[MAX_EDGE];
        //オブジェクトの中心座標
        Vector3 origin = target.position;
        //オブジェクトの大きさ
        Vector3 scale = target.localScale;

        //全て頂点座標を取得
        for(int edge = 0;edge < EdgeData.maxEdgeCnt; edge++)
        {
            returnEdge[edge] = origin + GetEdgeDistanceByScale(scale, edge, target);
        }

        //取得完了
        return returnEdge;
    }

    /// <summary>
    /// <para>GetEdgeDistanceByScale</para>
    /// <para>指定された頂点座標を取得します</para>
    /// </summary>
    /// <param name="scale">オブジェクトの大きさ</param>
    /// <param name="edge">指定された頂点</param>
    /// <param name="localObj">ローカル変換用オブジェクト</param>
    /// <returns>指定された頂点座標</returns>
    private static Vector3 GetEdgeDistanceByScale(Vector3 scale, int edge, Transform localObj)
    {
        //返却用
        Vector3 returnPos;

        //Scaleの半分を増加分として設定
        scale /= HALF;

        //各方向の差異を算出
        Vector3 scaleDisX = (localObj.right   * scale.x) * JudgeEdgeAxisX(edge);
        Vector3 scaleDisY = (localObj.up      * scale.y) * JudgeEdgeAxisY(edge);
        Vector3 scaleDisZ = (localObj.forward * scale.z) * JudgeEdgeAxisZ(edge);

        //算出結果を合計する
        returnPos = scaleDisX + scaleDisY + scaleDisZ;
        
        //算出完了
        return returnPos;
    }

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
}
