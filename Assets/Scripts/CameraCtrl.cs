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
    //縦軸の最大・最小角度
    private const float CAMERA_MAXVERTICAL = 60f;
    private const float CAMERA_MINVERTICAL = 10f;
    //カメラ距離
    private const float CAMERA_POS = -25f;

    //最小移動量
    private const float TRACK_PERMISSION = 0.0001f;
    //カメラの移動速度
    private const float TRACK_SPEED = 10f;
    //カメラの回転速度
    private const float ROTATE_SPEED = 5f;
    //基礎ベクトル
    private readonly Vector3 _cameraX = Vector3.up;     //カメラ横
    private readonly Vector3 _cameraY = Vector3.right;  //カメラ縦
    private readonly Vector3 _cameraHorizontal = Vector3.up + Vector3.right;
    private readonly Vector3 _cameraGravity = Vector3.forward; //カメラ反転


    //InputSystem
    private InputAction _mouseMove = default;
    //自身のTransform
    private Transform _horizontalObj = default;
    //縦軸回転オブジェ
    private Transform _verticalObj = default;
    //プレイヤーオブジェ
    [SerializeField]
    private Transform _playerObj = default;
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
        _verticalObj = _horizontalObj.GetChild(0);
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
        OutputCamera();
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);

    }

    /// <summary>
    /// <para>PositionTracking</para>
    /// <para>カメラ位置をプレイヤーの位置へ移動させます</para>
    /// </summary>
    private void PositionTracking()
    {
        //カメラ位置とプレイヤーとの差分ベクトルを取得
        Vector3 track = _playerObj.position - _horizontalObj.position;

        //移動量が規定値未満だったら処理しない
        if(track.sqrMagnitude < TRACK_PERMISSION)
        {
            _horizontalObj.position = _playerObj.position;
            return;
        }
        
        //移動
        _horizontalObj.position += (track * TRACK_SPEED) * Time.deltaTime;


    }

    /// <summary>
    /// <para>OutputCamera</para>
    /// <para>プレイヤーの入力値に対し、カメラを反映させます</para>
    /// </summary>
    private void OutputCamera()
    {
        //カメラ移動入力値
        Vector2 input = _mouseMove.ReadValue<Vector2>();

        //Debug.Log(input);

        //回転速度倍増
        input *= ROTATE_SPEED;
        //入力値を適した値に設定
        Vector3 inputHorizontal = _cameraX * -input.x;
        Vector3 inputVertical = _cameraY * input.y;

        //反映
        _horizontalObj.eulerAngles += inputHorizontal * Time.deltaTime;
        _verticalObj.eulerAngles += inputVertical * Time.deltaTime;

        //縦軸のカメラ角度を制限
        PermissionVertical();

        CameraTrack();
    }

    /// <summary>
    /// <para>PermissionVertical</para>
    /// <para>カメラの縦軸を制限します</para>
    /// </summary>
    private void PermissionVertical()
    {
        //現在のカメラの縦軸角度を取得
        float verticalAngle = _verticalObj.localEulerAngles.x;
        
        //最大角度を超える
        if(CAMERA_MAXVERTICAL < verticalAngle)
        {
            _verticalObj.eulerAngles = _horizontalObj.eulerAngles + _cameraY * CAMERA_MAXVERTICAL;
        }
        //最小角度を下回る
        else if(verticalAngle < CAMERA_MINVERTICAL)
        {
            _verticalObj.eulerAngles = _horizontalObj.eulerAngles + _cameraY * CAMERA_MINVERTICAL;
        }
    }

    /// <summary>
    /// <para>CameraTrack</para>
    /// <para>カメラをトラックさせます</para>
    /// </summary>
    private void CameraTrack()
    {
        //残り距離を算出
        Vector3 track = _verticalObj.position + _verticalObj.forward * CAMERA_POS - _cameraObj.position;

        Debug.DrawLine(_verticalObj.position, _verticalObj.position + _verticalObj.forward * CAMERA_POS);
        //カメラ位置を調整
        _cameraObj.position += track * TRACK_SPEED * Time.deltaTime;
        //カメラ距離が制限以下なら、位置を固定
        if(track.sqrMagnitude <= TRACK_PERMISSION)
        {
            _cameraObj.position = _verticalObj.position + _verticalObj.forward * CAMERA_POS;
        }
        //カメラの視点を設定
        _cameraObj.LookAt(_playerObj);
    }
    #endregion
}
