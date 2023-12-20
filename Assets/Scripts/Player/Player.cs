/// -----------------------------------------------------------------
/// Player.cs �v���C���[����
/// 
/// �쐬���F2023/11/28
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region �ϐ�
    [SerializeField, Header("�ړ����x")]
    private float _speed = 1f;

    //�d�͔��]�\
    [SerializeField]
    private bool _canChange = true;
    //�v���C���[�̑���\�t���O
    private bool _canInput = true;
   
    //�J������Transform
    private Transform _cameraObj = default;
    //���g��Transform
    private Transform _transform = default;
    //���g��RigidBody
    private OriginalRigidBody _rigid = default;

    #endregion

    #region �v���p�e�B
    //�d�͐؂�ւ��ݒ�
    public bool CanChange { set => _canChange = value; }
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
        _cameraObj = FindObjectOfType<CameraCtrl>().transform;
        _transform = transform;
        _rigid = GetComponent<OriginalRigidBody>();
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

    }

    /// <summary>
    /// <para>Move</para>
    /// <para>�ړ����s���܂�</para>
    /// </summary>
    /// <param name="input">[Vector2]����</param>
    public void Move(Vector2 input)
    {
        //����s�\�ł��� �܂��� �؂�ւ��s�\�ł���
        if (!_canInput || !_canChange)
        {
            return;
        }

        //���͕���
        Vector3 set = _cameraObj.right * input.x + _cameraObj.forward * -input.y;

        //�ړ����s
        _rigid.AddForce(set * _speed * Time.deltaTime);

        _transform.LookAt(_transform.position + set);
        //Debug.Log(input);
    }

    /// <summary>
    /// <para>ChangeGravity</para>
    /// <para>�d�͂�؂�ւ��܂�</para>
    /// </summary>
    public void ChangeGravity()
    {
        //����s�\�ł���
        if (!_canInput)
        {
            return;
        }

        //�؂�ւ��\�ł���
        if (_canChange)
        {
            //�d�͔��]����
            _rigid.MyGravity = -_rigid.MyGravity;
            _rigid.ResetForce();
            _canChange = false;
            Debug.Log("Change");
        }

    }
    #endregion
}
