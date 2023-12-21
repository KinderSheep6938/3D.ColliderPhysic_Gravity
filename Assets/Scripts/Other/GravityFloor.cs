/// -----------------------------------------------------------------
/// UpFloor.cs
/// 
/// �쐬���F2023/12/14
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OriginalMath;

public class GravityFloor : MonoBehaviour
{
    #region �ϐ�
    //���͋���
    private const float GRAVITY_DISTANCE = 2f;
    //�I�u�W�F�N�g�͈�
    private const float OBJECT_RANGE = 0.5f;

    //��x��������
    private bool _isOnce = false;

    //���g��Transform
    private Transform _transform = default;
    //���g��Collider
    private OriginalCollider _collider = default;
    //�v���C���[��Transform
    private Player _player = default;

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
        _player = FindObjectOfType<Player>().GetComponent<Player>();
        _collider = GetComponent<OriginalCollider>();
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
        Vector3 localPos = _transform.InverseTransformPoint(_player.transform.position);
        //X���͈͓̔�
        bool rangeX = (-OBJECT_RANGE <= localPos.x && localPos.x <= OBJECT_RANGE);
        bool rangeZ = (-OBJECT_RANGE <= localPos.z && localPos.z <= OBJECT_RANGE);

        //�����͈̔͊O�ł���
        if ((!rangeX || !rangeZ) && _isOnce)
        {
            _player.OnFloor -= 1;
            _isOnce = false;
            CheckGravityCanChange();
            return;
        }
        //Debug.Log("in:" + localPos);

        //�����񂹋��������[�J��������
        float localGravityDistance = GRAVITY_DISTANCE / _transform.localScale.y;
        //��ɏ���Ă��� ���� �����񂹉\�����ł��� ���� �d�͕������Ԃ�������ł���
        if (OBJECT_RANGE <= Mathf.Abs(localPos.y) && Mathf.Abs(localPos.y) <= OBJECT_RANGE + localGravityDistance)
        {
            CheckGravityCanChange();
            return;
        }


        _isOnce = false;
        Debug.DrawLine(_transform.position, _transform.position + _transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.white);
        Debug.DrawLine(_transform.position, _transform.position + -_transform.up * (_transform.localScale.y / 2 + GRAVITY_DISTANCE),Color.black);
    }

    /// <summary>
    /// <para>GetGravityOnVertical</para>
    /// <para>�d�͂����������ɑ΂��Ăǂ̂悤�ɓ����Ă��邩���ׂ܂�</para>
    /// </summary>
    private void CheckGravityCanChange()
    {
        //�Փ˔��肪���� ���� �܂���x���������s���Ă��Ȃ�
        if (_collider.Collision && !_isOnce)
        {
            //�؂�ւ��\��
            _player.OnFloor += 1;
            _isOnce = true;
            Debug.Log("on");
        }
    }
    #endregion
}
