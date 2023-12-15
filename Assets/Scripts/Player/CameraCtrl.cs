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
    //�J�����ړ����͂̍ŏ��f�[�^�l
    private const float DEAD_MINVALUE = 0.9f;
    //�J�����p�x
    private const float CAMERA_MOVE_ANGLE = 90f;
    //�J��������
    private const float CAMERA_DISTANCE = -40f;
    //�ő�J�����ʒu
    private const int MAX_POS = 3;

    //�ŏ��ړ���
    private const float TRACK_PERMISSION = 0.0001f;
    //�J�����̈ړ����x
    private const float TRACK_SPEED = 10f;
    //��b�x�N�g��
    private readonly Vector3 _cameraX = Vector3.up;     //�J������


    //�ړ��\�t���O
    private bool _canMove = true;
    //�J�����̊p�x�ʒu
    private int _cameraPos = 0;

    //InputSystem
    private InputAction _mouseMove = default;
    //���g��Transform
    private Transform _horizontalObj = default;
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
        RotateCamera();

        //�J�����g���b�N
        CameraTracking();

    }

    /// <summary>
    /// <para>CameraTracking</para>
    /// <para>�J�����ʒu��K�����p�x�ֈړ������܂�</para>
    /// </summary>
    private void CameraTracking()
    {
        //�J�����ʒu
        Vector3 trackPos = _horizontalObj.position + _horizontalObj.forward * CAMERA_DISTANCE;
        //�J�����ʒu�ƃv���C���[�Ƃ̍����x�N�g�����擾
        Vector3 trackVelocity = trackPos - _cameraObj.position;

        //�ړ��ʂ��K��l�����������珈�����Ȃ�
        if(trackVelocity.sqrMagnitude < TRACK_PERMISSION)
        {
            _cameraObj.position = trackPos;
            return;
        }
        
        //�ړ�
        _cameraObj.position += (trackVelocity * TRACK_SPEED) * Time.deltaTime;
        _cameraObj.LookAt(_horizontalObj);

    }

    /// <summary>
    /// <para>RotateCamera</para>
    /// <para>�v���C���[�̓��͒l�ɑ΂��A�J�����𔽉f�����܂�</para>
    /// </summary>
    private void RotateCamera()
    {
        //�J�����ړ����͒l
        Vector2 input = _mouseMove.ReadValue<Vector2>();

        //�ړ��\�ł͂Ȃ�
        if (!_canMove)
        {
            //���͒l���Ȃ��ꍇ�A�����͂��Ȃ�
            if (Mathf.Abs(input.x) < DEAD_MINVALUE)
            {
                //�ړ��\��
                _canMove = true;
            }
            return;
        }

        //���͑ҋ@
        if(Mathf.Abs(input.x) < DEAD_MINVALUE)
        {
            return;
        }

        //���͒l�ɂ���Ĉʒu���ړ�
        _cameraPos -= (int)Mathf.Sign(input.x);

        //���K��
        if(MAX_POS < _cameraPos)
        {
            _cameraPos = 0;
        }
        if(_cameraPos < 0)
        {
            _cameraPos = MAX_POS;
        }

        //���f
        _horizontalObj.eulerAngles = _cameraX * _cameraPos * CAMERA_MOVE_ANGLE;

        _canMove = false;
    }



    #endregion
}
