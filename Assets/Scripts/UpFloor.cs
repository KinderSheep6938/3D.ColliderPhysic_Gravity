/// -----------------------------------------------------------------
/// UpFloor.cs
/// 
/// �쐬���F2023/12/14
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpFloor : MonoBehaviour
{
    #region �ϐ�
    //���͋���
    private const float GRAVITY_DISTANCE = 5f;
    //�I�u�W�F�N�g
    private const float OBJECT_RANGE = 0.5f;
    //��b�x�N�g��
    private readonly Vector3 _vectorUp = Vector3.up;

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
        if (!rangeX && !rangeZ)
        {
            return;
        }

        //�����񂹋��������[�J��������
        float localGravityDistance = GRAVITY_DISTANCE / _transform.localScale.y;
        //��ɏ���Ă��� ���� �����񂹉\�����ł���
        if (OBJECT_RANGE <= localPos.y && localPos.y <= OBJECT_RANGE + localGravityDistance)
        {

        }

        //��ɏ���Ă��� ���� �����񂹉\�����ł���
        if (localPos.y <= -OBJECT_RANGE && -OBJECT_RANGE - localGravityDistance <= localPos.y)
        {

        }
    }
    #endregion
}
