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
    //��b�x�N�g��
    private readonly Vector3 _vectorRight = Vector3.right;
    private readonly Vector3 _vectorUp = Vector3.up;
    private readonly Vector3 _vectorForward = Vector3.forward;

    [SerializeField, Header("�ړ����x")]
    private float _speed = 1f;
    [SerializeField, Header("������")]
    private float _jumpPower = 1f;


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
        Vector3 set = _vectorRight * input.x + _vectorForward * input.y;

        //�ړ�������
        _rigid.AddForce(set * _speed * Time.deltaTime);
        Debug.Log(input);
    }

    /// <summary>
    /// <para>Jump</para>
    /// <para>������s���܂�</para>
    /// </summary>
    public void Jump()
    {
        //�ړ�������
        _rigid.AddForce(_vectorUp * _jumpPower);
        Debug.Log("jump");
    }
    #endregion
}