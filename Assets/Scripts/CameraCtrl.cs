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

    //最小移動量
    private const float TRACK_PERMISSION = 0.0001f;
    //カメラの移動速度
    private const float TRACK_SPEED = 10f;
    //カメラの回転速度
    private const float ROTATE_SPEED = 5f;
    //基礎ベクトル
    private readonly Vector3 _cameraX = Vector3.up;     //カメラ横
    private readonly Vector3 _cameraY = Vector3.right;  //カメラ縦

    //InputSystem
    private InputAction _mouseMove = default;
    //自身のTransform
    private Transform _horizontalObj = default;
    //縦軸回転オブジェ
    private Transform _verticalObj = default;
    //プレイヤーオブジェ
    [SerializeField]
    private Transform _playerObj = default;
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
    private void Update()
    {
        //カメラ移動
        OutputCamera();
    }

    private void FixedUpdate()
    {
        //位置移動
        PositionTracking();
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
            return;
        }
        
        //移動
        _horizontalObj.Translate((track * TRACK_SPEED) * Time.fixedDeltaTime);
    }

    /// <summary>
    /// <para>OutputCamera</para>
    /// <para>プレイヤーの入力値に対し、カメラを反映させます</para>
    /// </summary>
    private void OutputCamera()
    {
        //カメラ移動入力値
        Vector2 input = _mouseMove.ReadValue<Vector2>();

        Debug.Log(input);

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
    }

    /// <summary>
    /// <para>PermissionVertical</para>
    /// <para>カメラの縦軸を制限します</para>
    /// </summary>
    private void PermissionVertical()
    {
        //現在のカメラの縦軸角度を取得
        float verticalAngle = _verticalObj.eulerAngles.x;
        
        //最大角度を超える
        if(CAMERA_MAXVERTICAL < verticalAngle)
        {
            _verticalObj.eulerAngles = _cameraY * CAMERA_MAXVERTICAL;
        }
        //最小角度を下回る
        else if(verticalAngle < CAMERA_MINVERTICAL)
        {
            _verticalObj.eulerAngles = _cameraY * CAMERA_MINVERTICAL;
        }
    }
    #endregion
}
