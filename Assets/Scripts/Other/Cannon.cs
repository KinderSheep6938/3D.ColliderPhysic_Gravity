/// -----------------------------------------------------------------
/// Cannon.cs
/// 
/// 作成日：2023/12/20
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    #region 変数
    //基礎ベクトル
    private readonly Vector3 _vectorZero = Vector3.zero;

    [SerializeField, Header("弾速")]
    private Vector3 _bulletMoveValue = default;
    [SerializeField, Header("弾の存在時間")]
    private float _bulletLifeTime = default;
    [SerializeField, Header("弾オブジェ")]
    private GameObject _bullet = default;

    //弾の移動制御
    private MoveObj _bulletMove = default;
    //自身のTransform
    private Transform _transform = default;

    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //初期化
        _transform = transform;

        //弾生成
        _bullet = Instantiate(_bullet,_transform.position,Quaternion.identity);
        //弾の移動制御クラスを取得
        _bulletMove = _bullet.GetComponent<MoveObj>();

        //コルーチン開始
        StartCoroutine(LoopFire());
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// <para>LoopFire</para>
    /// <para>弾の発射処理をおこないます</para>
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoopFire()
    {
        //繰り返し
        while (true)
        {
            //弾の位置を初期化
            _bullet.transform.position = _transform.position;
            _bulletMove.Speed = SpeedLocalize(_bulletMoveValue);

            //生存時間まで待機
            yield return new WaitForSeconds(_bulletLifeTime);
        }
    }

    /// <summary>
    /// <para>SpeedLocalize</para>
    /// <para>弾の移動量を大砲の向きに合わせてローカル化します</para>
    /// </summary>
    /// <param name="speed">弾の移動量</param>
    /// <returns>ローカル化された移動量</returns>
    private Vector3 SpeedLocalize(Vector3 speed)
    {
        //返却用合計値
        Vector3 sum = _vectorZero;

        //各軸の移動量を計算
        sum += _transform.right * speed.x;
        sum += _transform.up * speed.y;
        sum += _transform.forward * speed.z;

        //返却
        return sum;
    }

    #endregion
}
