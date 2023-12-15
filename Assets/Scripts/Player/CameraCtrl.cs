/// -----------------------------------------------------------------
/// CameraCtrl.cs
/// 
/// 作成日：2023/12/05
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCtrl : MonoBehaviour
{
    #region 変数
    //カメラ移動入力の最小データ値
    private const float DEAD_MINVALUE = 0.9f;
    //カメラ角度
    private const float CAMERA_MOVE_ANGLE = 90f;
    //カメラ距離
    private const float CAMERA_DISTANCE = -40f;
    //最大カメラ位置
    private const int MAX_POS = 3;

    //最小移動量
    private const float TRACK_PERMISSION = 0.0001f;
    //カメラの移動速度
    private const float TRACK_SPEED = 10f;
    //基礎ベクトル
    private readonly Vector3 _cameraX = Vector3.up;     //カメラ横


    //移動可能フラグ
    private bool _canMove = true;
    //カメラの角度位置
    private int _cameraPos = 0;

    //InputSystem
    private InputAction _mouseMove = default;
    //自身のTransform
    private Transform _horizontalObj = default;
    //カメラオブジェ
    [SerializeField]
    private Transform _cameraObj = default;
    #endregion

    #region プロパティ
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _horizontalObj = transform;
        _mouseMove = FindObjectOfType<PlayerInput>().actions["CameraMove"];
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
    private void LateUpdate()
    {
        //カメラ移動
        RotateCamera();

        //カメラトラック
        CameraTracking();

    }

    /// <summary>
    /// <para>CameraTracking</para>
    /// <para>カメラ位置を適した角度へ移動させます</para>
    /// </summary>
    private void CameraTracking()
    {
        //カメラ位置
        Vector3 trackPos = _horizontalObj.position + _horizontalObj.forward * CAMERA_DISTANCE;
        //カメラ位置とプレイヤーとの差分ベクトルを取得
        Vector3 trackVelocity = trackPos - _cameraObj.position;

        //移動量が規定値未満だったら処理しない
        if(trackVelocity.sqrMagnitude < TRACK_PERMISSION)
        {
            _cameraObj.position = trackPos;
            return;
        }
        
        //移動
        _cameraObj.position += (trackVelocity * TRACK_SPEED) * Time.deltaTime;
        _cameraObj.LookAt(_horizontalObj);

    }

    /// <summary>
    /// <para>RotateCamera</para>
    /// <para>プレイヤーの入力値に対し、カメラを反映させます</para>
    /// </summary>
    private void RotateCamera()
    {
        //カメラ移動入力値
        Vector2 input = _mouseMove.ReadValue<Vector2>();

        //移動可能ではない
        if (!_canMove)
        {
            //入力値がない場合、処理はしない
            if (Mathf.Abs(input.x) < DEAD_MINVALUE)
            {
                //移動可能に
                _canMove = true;
            }
            return;
        }

        //入力待機
        if(Mathf.Abs(input.x) < DEAD_MINVALUE)
        {
            return;
        }

        //入力値によって位置を移動
        _cameraPos -= (int)Mathf.Sign(input.x);

        //正規化
        if(MAX_POS < _cameraPos)
        {
            _cameraPos = 0;
        }
        if(_cameraPos < 0)
        {
            _cameraPos = MAX_POS;
        }

        //反映
        _horizontalObj.eulerAngles = _cameraX * _cameraPos * CAMERA_MOVE_ANGLE;

        _canMove = false;
    }



    #endregion
}
