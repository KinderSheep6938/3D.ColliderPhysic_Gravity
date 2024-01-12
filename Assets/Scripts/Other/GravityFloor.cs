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
    //�I�u�W�F�N�g�͈�
    private const float OBJECT_RANGE = 0.5f;

    //��x��������
    private bool _isOnePlay = false;
    //�؂�ւ�����\����
    private float _canChangeDistance = 0.55f;

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

        //�����񂹉\�����Ƀv���C���[�̏c�������̋����𑫂�
        _canChangeDistance += _player.transform.localScale.y / 2;
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
        //Z���͈͓̔�
        bool rangeZ = (-OBJECT_RANGE <= localPos.z && localPos.z <= OBJECT_RANGE);

        //�����͈̔͊O�ł��� ���� ��x�ڂ̏����ł���
        if ((!rangeX || !rangeZ) && _isOnePlay)
        {
            DeleteConect();
            CheckGravityCanChange();
            return;
        }
        //Debug.Log("in:" + localPos);

        //�����񂹋��������[�J��������
        float localGravityDistance = _canChangeDistance / _transform.localScale.y;
        //��ɏ���Ă��� ���� �����񂹉\�����ł��� ���� �d�͕������Ԃ�������ł���
        if (OBJECT_RANGE <= Mathf.Abs(localPos.y) && Mathf.Abs(localPos.y) <= OBJECT_RANGE + localGravityDistance)
        {
            CheckGravityCanChange();
            return;
        }

        DeleteConect();
        Debug.DrawLine(_transform.position, _transform.position + _transform.up * (_transform.localScale.y / 2 + _canChangeDistance),Color.white);
        Debug.DrawLine(_transform.position, _transform.position + -_transform.up * (_transform.localScale.y / 2 + _canChangeDistance),Color.black);
    }

    /// <summary>
    /// <para>GetGravityOnVertical</para>
    /// <para>�d�͂����������ɑ΂��Ăǂ̂悤�ɓ����Ă��邩���ׂ܂�</para>
    /// </summary>
    private void CheckGravityCanChange()
    {
        //�Փ˔��肪���� ���� �܂���x���������s���Ă��Ȃ�
        if (_collider.Collision && !_isOnePlay)
        {
            //�ڒn�J�E���g�𑝉�
            _player.OnFloor += 1;
            _isOnePlay = true;
            //Debug.Log("on");
        }
    }

    /// <summary>
    /// <para>DeleteConect</para>
    /// <para>�ڒn������폜����</para>
    /// </summary>
    private void DeleteConect()
    {
        //�������s����
        if (_isOnePlay)
        {
            //�ڒn�J�E���g������
            _player.OnFloor -= 1;
            _isOnePlay = false;
        }

    }
    #endregion
}
