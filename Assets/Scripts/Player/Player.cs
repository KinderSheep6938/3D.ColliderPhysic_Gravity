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
   
    //�J������Transform
    private Transform _cameraObj = default;
    //���g��Transform
    private Transform _transform = default;
    //���g��RigidBody
    private OriginalRigidBody _rigid = default;

    #endregion

    #region �v���p�e�B

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
        if(input.sqrMagnitude == 0)
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
        //�ړ�������
        _rigid.MyGravity = -_rigid.MyGravity;
        Debug.Log("Change");
    }
    #endregion
}
