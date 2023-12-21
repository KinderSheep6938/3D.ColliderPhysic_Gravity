/// -----------------------------------------------------------------
/// ViewObj.cs
/// 
/// 作成日：2023/12/21
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObj : MonoBehaviour
{
    #region 変数
    //自身のTransform
    private Transform _transform = default;
    //自身の子供の数
    private int _childCnt = 0;
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _transform = transform;
        _childCnt = _transform.childCount;
    }

    /// <summary>
    /// <para>SetView</para>
    /// <para>オブジェクトの表示を設定します</para>
    /// </summary>
    /// <param name="active"></param>
    public void SetView(bool active)
    {
        //子供の数分、切り替える
        for(int i = 0; i < _childCnt; i++)
        {
            //子供の状態を切り替え
            _transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    #endregion
}
