/// -----------------------------------------------------------------
/// OriginalRigidBody.cs
/// 
/// �쐬���F2023/11/17
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicLibrary;

public class OriginalRigidBody : MonoBehaviour
{
    #region �ϐ�
    //���g�̕����f�[�^
    [SerializeField]
    private PhysicData _myPhysic = new();

    //���g��Collider
    private OriginalCollider _collider = default;
    //���g��Transform
    private Transform _transform = default;
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
        _collider = _transform.GetComponent<OriginalCollider>();
    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    private void FixedUpdate()
    {
        //�d��
        Gravity();

        //����
        Repulsion(_collider);

        //���x���f
        ApplyVelocity();
    }

    /// <summary>
    /// <para>Gravity</para>
    /// <para>���g�ɏd�͂𔽉f�����܂�</para>
    /// </summary>
    private void Gravity()
    {
        //�d�͉��Z
        _myPhysic.velocity = PhysicManager.Gravity(_myPhysic);
    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Collider�ɂ�锽���͂��擾���܂�</para>
    /// </summary>
    private void Repulsion(OriginalCollider collider)
    {
        //Collider���ݒ肳��Ă��Ȃ��ꍇ�͏������Ȃ�
        if(collider == default)
        {
            return;
        }

        //�Փ˔��肪���邩
        if (collider.Collision)
        {
            //���̂܂܋t�֔���
            _myPhysic.velocity = -_myPhysic.velocity;
        }
    }

    /// <summary>
    /// <para>ApplyVelocity</para>
    /// <para>���g�̑��x�����W�ɔ��f�����܂�</para>
    /// </summary>
    private void ApplyVelocity()
    {
        //���x���f
        _transform.position += _myPhysic.velocity;
    }
    #endregion
}
