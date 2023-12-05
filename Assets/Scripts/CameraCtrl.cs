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

    //�ŏ��ړ���
    private const float TRACK_PERMISSION = 0.0001f;
    //�J�����̈ړ����x
    private const float TRACK_SPEED = 10f;
    //�J�����̉�]���x
    private const float ROTATE_SPEED = 5f;
    //��b�x�N�g��
    private readonly Vector3 _cameraX = Vector3.up;     //�J������
    private readonly Vector3 _cameraY = Vector3.right;  //�J�����c

    //InputSystem
    private InputAction _mouseMove = default;
    //���g��Transform
    private Transform _horizontalObj = default;
    //�c����]�I�u�W�F
    private Transform _verticalObj = default;
    //�v���C���[�I�u�W�F
    [SerializeField]
    private Transform _playerObj = default;
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
    private void Update()
    {
        //�J�����ړ�
        OutputCamera();
    }

    private void FixedUpdate()
    {
        //�ʒu�ړ�
        PositionTracking();
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
            return;
        }
        
        //�ړ�
        _horizontalObj.Translate((track * TRACK_SPEED) * Time.fixedDeltaTime);
    }

    /// <summary>
    /// <para>OutputCamera</para>
    /// <para>�v���C���[�̓��͒l�ɑ΂��A�J�����𔽉f�����܂�</para>
    /// </summary>
    private void OutputCamera()
    {
        //�J�����ړ����͒l
        Vector2 input = _mouseMove.ReadValue<Vector2>();

        Debug.Log(input);

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
    }

    /// <summary>
    /// <para>PermissionVertical</para>
    /// <para>�J�����̏c���𐧌����܂�</para>
    /// </summary>
    private void PermissionVertical()
    {
        //���݂̃J�����̏c���p�x���擾
        float verticalAngle = _verticalObj.eulerAngles.x;
        
        //�ő�p�x�𒴂���
        if(CAMERA_MAXVERTICAL < verticalAngle)
        {
            _verticalObj.eulerAngles = _cameraY * CAMERA_MAXVERTICAL;
        }
        //�ŏ��p�x�������
        else if(verticalAngle < CAMERA_MINVERTICAL)
        {
            _verticalObj.eulerAngles = _cameraY * CAMERA_MINVERTICAL;
        }
    }
    #endregion
}
