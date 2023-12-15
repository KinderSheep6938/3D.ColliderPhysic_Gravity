/// -----------------------------------------------------------------
/// UpFloor.cs
/// 
/// �쐬���F2023/12/14
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicLibrary.Manager;

public class GravityFloor : MonoBehaviour
{
    #region �ϐ�
    //���͋���
    private const float GRAVITY_DISTANCE = 10f;
    //�d�͔��]�p�x
    private const float GRAVITY_ANGLE = 180f;
    //�I�u�W�F�N�g
    private const float OBJECT_RANGE = 0.5f;
    //��b�x�N�g��
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorForward = Vector3.forward;

    //���g��Transform
    private Transform _transform = default;
    //�v���C���[��Transform
    private Transform _player = default;
    //�v���C���[��Rigid
    private OriginalRigidBody _playerRigid = default;

    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //����������
        _transform = transform;
        _player = FindObjectOfType<Player>().transform;
        _playerRigid = _player.GetComponent<OriginalRigidBody>();


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
        SetGravity();
    }



    /// <summary>
    /// <para>HeightOverDestroy</para>
    /// <para>�����荂�x�𒴂����ꍇ�A�폜���܂�</para>
    /// </summary>
    private void SetGravity()
    {
        //�v���C���[���W�����[�J����
        Vector3 localPos = _transform.InverseTransformPoint(_player.position);
        //X���͈͓̔�
        bool rangeX = (-OBJECT_RANGE <= localPos.x && localPos.x <= OBJECT_RANGE);
        bool rangeZ = (-OBJECT_RANGE <= localPos.z && localPos.z <= OBJECT_RANGE);

        //�����͈̔͊O�ł���
        if (!rangeX || !rangeZ)
        {
            return;
        }
        Debug.Log("in:" + localPos);

        //�����񂹋��������[�J��������
        float localGravityDistance = GRAVITY_DISTANCE / _transform.localScale.y;
        //��ɏ���Ă��� ���� �����񂹉\�����ł���
        if (OBJECT_RANGE <= localPos.y && localPos.y <= OBJECT_RANGE + localGravityDistance)
        {
            Debug.Log("up");
            _playerRigid.MyGravity = _transform.up * PhysicManager.CommonGravity.y;
            _player.eulerAngles = _vectorZero;
        }

        //���ɏ���Ă��� ���� �����񂹉\�����ł���
        if (localPos.y <= -OBJECT_RANGE && -OBJECT_RANGE - localGravityDistance <= localPos.y)
        {
            Debug.Log("down");
            _playerRigid.MyGravity = -_transform.up * PhysicManager.CommonGravity.y;
            _player.eulerAngles = _vectorForward * GRAVITY_ANGLE;
        }

        Debug.DrawLine(_transform.position, _transform.position + _transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.white);
        Debug.DrawLine(_transform.position, _transform.position + -_transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.black);
    }
    #endregion
}
