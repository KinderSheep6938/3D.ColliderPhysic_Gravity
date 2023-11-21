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
using PhysicLibrary.Manager;


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
    //���݂̑��x
    public Vector3 Velocity { get => _myPhysic.velocity; }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _myPhysic.velocity = new Vector3(0, 1f, 0);
        }
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
        //�d�͉����x���Z
        _myPhysic.force = PhysicManager.Gravity(_myPhysic);

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Collider�ɂ�锽���͂��擾���܂�</para>
    /// </summary>
    private void Repulsion(OriginalCollider collider)
    {
        //Collider���ݒ肳��Ă��Ȃ� �܂��� �Փ˔��肪�Ȃ���
        if (collider == default || !collider.CollisionData.collision)
        {
            //���̂ɉ������Ă���͂����̂܂ܑ��x�Ƃ��ĕt��
            ForceToVelocity();
            return;
        }

        //�Փ˂��������������āA���x���Z�o
        _myPhysic.force = PhysicManager.RepulsionForceByCollider(_myPhysic, collider.CollisionData);
        ForceToVelocity();

        //�Փ˂��Ă��镨�̂̏󋵂��������āA���x���Z�o
        //_myPhysic.velocity = -(_myPhysic.reboundRatio * _myPhysic.velocity);
    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>���̂ɗ^����ꂽ�͂𑬓x�ɕϊ����܂�</para>
    /// </summary>
    private void ForceToVelocity()
    {
        //�͂𑬓x�ɑ��
        _myPhysic.velocity = _myPhysic.force;

        //��R�͂�����
        _myPhysic.velocity -= _myPhysic.resistance * _myPhysic.velocity;

        return;
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
