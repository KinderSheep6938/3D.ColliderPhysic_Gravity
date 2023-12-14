/// -----------------------------------------------------------------
/// CameraCtrl.cs
/// 
/// �쐬���F2023/12/05
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCtrl : MonoBehaviour
{
    #region �ϐ�
    //�c���̍ő�E�ŏ��p�x
    private const float CAMERA_MAXVERTICAL = 60f;
    private const float CAMERA_MINVERTICAL = 10f;
    //�J��������
    private const float CAMERA_POS = -25f;

    //�ŏ��ړ���
    private const float TRACK_PERMISSION = 0.0001f;
    //�J�����̈ړ����x
    private const float TRACK_SPEED = 10f;
    //�J�����̉�]���x
    private const float ROTATE_SPEED = 5f;
    //��b�x�N�g��
    private readonly Vector3 _cameraX = Vector3.up;     //�J������
    private readonly Vector3 _cameraY = Vector3.right;  //�J�����c
    private readonly Vector3 _cameraHorizontal = Vector3.up + Vector3.right;
    private readonly Vector3 _cameraGravity = Vector3.forward; //�J�������]


    //InputSystem
    private InputAction _mouseMove = default;
    //���g��Transform
    private Transform _horizontalObj = default;
    //�c����]�I�u�W�F
    private Transform _verticalObj = default;
    //�v���C���[�I�u�W�F
    [SerializeField]
    private Transform _playerObj = default;
    //�J�����I�u�W�F
    [SerializeField]
    private Transform _cameraObj = default;
    #endregion

    #region �v���p�e�B
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        _horizontalObj = transform;
        _verticalObj = _horizontalObj.GetChild(0);
        _mouseMove = FindObjectOfType<PlayerInput>().actions["CameraMove"];
    }

    /// <summary>
    /// �X�V�O����
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void LateUpdate()
    {
        //�J�����ړ�
        OutputCamera();
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);

    }

    /// <summary>
    /// <para>PositionTracking</para>
    /// <para>�J�����ʒu���v���C���[�̈ʒu�ֈړ������܂�</para>
    /// </summary>
    private void PositionTracking()
    {
        //�J�����ʒu�ƃv���C���[�Ƃ̍����x�N�g�����擾
        Vector3 track = _playerObj.position - _horizontalObj.position;

        //�ړ��ʂ��K��l�����������珈�����Ȃ�
        if(track.sqrMagnitude < TRACK_PERMISSION)
        {
            _horizontalObj.position = _playerObj.position;
            return;
        }
        
        //�ړ�
        _horizontalObj.position += (track * TRACK_SPEED) * Time.deltaTime;


    }

    /// <summary>
    /// <para>OutputCamera</para>
    /// <para>�v���C���[�̓��͒l�ɑ΂��A�J�����𔽉f�����܂�</para>
    /// </summary>
    private void OutputCamera()
    {
        //�J�����ړ����͒l
        Vector2 input = _mouseMove.ReadValue<Vector2>();

        //Debug.Log(input);

        //��]���x�{��
        input *= ROTATE_SPEED;
        //���͒l��K�����l�ɐݒ�
        Vector3 inputHorizontal = _cameraX * -input.x;
        Vector3 inputVertical = _cameraY * input.y;

        //���f
        _horizontalObj.eulerAngles += inputHorizontal * Time.deltaTime;
        _verticalObj.eulerAngles += inputVertical * Time.deltaTime;

        //�c���̃J�����p�x�𐧌�
        PermissionVertical();

        CameraTrack();
    }

    /// <summary>
    /// <para>PermissionVertical</para>
    /// <para>�J�����̏c���𐧌����܂�</para>
    /// </summary>
    private void PermissionVertical()
    {
        //���݂̃J�����̏c���p�x���擾
        float verticalAngle = _verticalObj.localEulerAngles.x;
        
        //�ő�p�x�𒴂���
        if(CAMERA_MAXVERTICAL < verticalAngle)
        {
            _verticalObj.eulerAngles = _horizontalObj.eulerAngles + _cameraY * CAMERA_MAXVERTICAL;
        }
        //�ŏ��p�x�������
        else if(verticalAngle < CAMERA_MINVERTICAL)
        {
            _verticalObj.eulerAngles = _horizontalObj.eulerAngles + _cameraY * CAMERA_MINVERTICAL;
        }
    }

    /// <summary>
    /// <para>CameraTrack</para>
    /// <para>�J�������g���b�N�����܂�</para>
    /// </summary>
    private void CameraTrack()
    {
        //�c�苗�����Z�o
        Vector3 track = _verticalObj.position + _verticalObj.forward * CAMERA_POS - _cameraObj.position;

        Debug.DrawLine(_verticalObj.position, _verticalObj.position + _verticalObj.forward * CAMERA_POS);
        //�J�����ʒu�𒲐�
        _cameraObj.position += track * TRACK_SPEED * Time.deltaTime;
        //�J���������������ȉ��Ȃ�A�ʒu���Œ�
        if(track.sqrMagnitude <= TRACK_PERMISSION)
        {
            _cameraObj.position = _verticalObj.position + _verticalObj.forward * CAMERA_POS;
        }
        //�J�����̎��_��ݒ�
        _cameraObj.LookAt(_playerObj);
    }
    #endregion
}
