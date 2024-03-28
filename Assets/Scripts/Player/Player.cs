/// -----------------------------------------------------------------
/// Player.cs �v���C���[����
/// 
/// �쐬���F2023/11/28
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OriginalMath;
using ColliderLibrary.DataManager;
using ColliderLibrary;

public class Player : MonoBehaviour
{
    #region �ϐ�
    //�����ȗ͂̍ŏ�������
    private const float PERMISSION_VERTICAL_MINMAGNITUDE = 1f;

    //��b�x�N�g��
    private readonly Vector3 _vectorUp = Vector3.up;

    [SerializeField, Header("�ړ����x")]
    private float _speed = 1f;
    //�d�͔��]�\
    [SerializeField]
    private int _onFloorCnt = 0;

    //�v���C���[�̑���\�t���O
    private bool _canInput = true;
    //��x��������
    private bool _isPlay = false;
   
    //�J������Transform
    private Transform _cameraObj = default;
    //���g��Transform
    private Transform _transform = default;
    //���g��RigidBody
    private OriginalRigidBody _rigid = default;
    //���ʉ��Đ��N���X
    private SoundPlayer _se = default;

    #endregion

    #region �v���p�e�B
    //�d�͐؂�ւ��ݒ�
    public int OnFloor { get => _onFloorCnt; set => _onFloorCnt = value; }
    //����s�\�ݒ�
    public bool SetStopInput { set => _canInput = value; }
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //������
        _transform = transform;
        _rigid = GetComponent<OriginalRigidBody>();
        _se = GetComponent<SoundPlayer>();

        //�J��������
        CameraCtrl camera = FindObjectOfType<CameraCtrl>();
        //�J�����������ݒ肳��Ă���
        if (camera)
        {
            //�J������Transform���擾
            _cameraObj = FindObjectOfType<CameraCtrl>().transform;
        }

    }

    /// <summary>
    /// <para>Move</para>
    /// <para>�ړ����s���܂�</para>
    /// </summary>
    /// <param name="input">[Vector2]����</param>
    public void Move(Vector2 input)
    {
        //����s�\�ł��� �܂��� �؂�ւ��s�\�ł���
        if (!_canInput || _onFloorCnt == 0)
        {
            //��x���������s���Ă��Ȃ�
            if (!_isPlay)
            {
                _isPlay = true;
                _rigid.ResetForce();
            }
            return;
        }

        //�J�����̌�������e�����̈ړ��ʂ��擾
        Vector3 set = _cameraObj.right * input.x + _cameraObj.forward * input.y;

        //�ړ����s
        _rigid.AddForce(set * _speed * Time.deltaTime);
        //�L�������ړ������Ɍ�����
        _transform.LookAt(_transform.position + set);
        _isPlay = false;
    }

    /// <summary>
    /// <para>ChangeGravity</para>
    /// <para>�d�͂�؂�ւ��܂�</para>
    /// </summary>
    public void ChangeGravity()
    {
        //����s�\�ł���
        if (!_canInput || PERMISSION_VERTICAL_MINMAGNITUDE < Mathf.Abs(GetTo.V3Projection(_rigid.Velocity, _vectorUp).sqrMagnitude))
        {
            return;
        }

        //�؂�ւ��\�ł���
        if (_onFloorCnt != 0)
        {
            //�d�͔��]����
            _rigid.MyGravity = -_rigid.MyGravity;
            _rigid.ResetForce();
            //���ʉ��Đ�
            _se.Play();
        }

    }

    private void OnDrawGizmos()
    {
        foreach(ColliderData a in ColliderDataManager.GetColliderToWorld())
        {
            Gizmos.DrawWireCube(a.position, a.localScale);
        }
    }
    #endregion
}
