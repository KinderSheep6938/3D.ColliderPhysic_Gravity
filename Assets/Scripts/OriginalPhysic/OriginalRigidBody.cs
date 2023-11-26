/// -----------------------------------------------------------------
/// OriginalRigidBody.cs ��������
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

    //�Vector
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorRight = Vector3.right;
    private readonly Vector3 _vectorUp = Vector3.up;
    private readonly Vector3 _vectorForward = Vector3.forward;

    //Collider�̏Փ˔���ړ��⊮
    public enum InterpolateStatus
    {
        None,
        Interpolate
    }
    [SerializeField, Header("���x�ۊ�")]
    private InterpolateStatus _interpolate = InterpolateStatus.None;

    //���W�Œ�
    private enum Freeze
    {
        None,   //���ɂȂ�
        XOnly,  //X���Œ�
        YOnly,  //Y���Œ�
        ZOnly,  //Z���Œ�
        XtoY,   //X����Y���Œ�
        YtoZ,   //Y����Z���Œ�
        XtoZ,   //X����Z���Œ�
        All     //�S�ČŒ�
    }
    [SerializeField, Header("���W�Œ�")]
    private Freeze _freezeStatus = Freeze.None;

    //���d�͔���
    [SerializeField]
    private bool _noGravity = false;


    //���g�̕����f�[�^
    [SerializeField]
    private PhysicData _myPhysic = new();
    //���g��Collider
    private IColliderInfoAccessible _colliderAccess = default;
    //���g��Transform
    private Transform _transform = default;
    #endregion

    #region �v���p�e�B
    public InterpolateStatus Interpolate { get => _interpolate; }

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
        _colliderAccess = GetComponent<IColliderInfoAccessible>();

        //�����蔻�葤�̐ڑ��C���^�[�t�F�C�X��ݒ�
        _myPhysic.colliderInfo = _colliderAccess;

        //�Q�[���I�u�W�F�N�g������������Ă���ꍇ�͏�������߂�
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _myPhysic.force += new Vector3(0, 1f, 0);
        }
    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    private void FixedUpdate()
    {
        //���W���S�ČŒ肳��Ă���ꍇ�͉������Ȃ�
        if(_freezeStatus == Freeze.All)
        {
            return;
        }

        //�d��
        Gravity();

        //����
        Repulsion();

        //���x���f
        ApplyVelocity();
    }

    /// <summary>
    /// <para>Gravity</para>
    /// <para>���g�ɏd�͂𔽉f�����܂�</para>
    /// </summary>
    private void Gravity()
    {
        //���d�͂ɐݒ肳��Ă���̂ł���Ώ������Ȃ�
        if (_noGravity)
        {
            return;
        }

        //�d�͉����x���Z
        _myPhysic.force = PhysicManager.Gravity(_myPhysic);

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Collider�ɂ�锽���͂��擾���܂�</para>
    /// </summary>
    private void Repulsion()
    {
        //Collider���ݒ肳��Ă��Ȃ� �܂��� �Փ˔��肪�Ȃ���
        if (_colliderAccess == default || !_colliderAccess.Collision.flag)
        {
            //���̂ɉ������Ă���͂����̂܂ܑ��x�Ƃ��ĕt��
            ForceToVelocity();
            return;
        }

        //�Փ˂��������������āA���x���Z�o
        _myPhysic.force = PhysicManager.ChangeForceByPhysicMaterials(_myPhysic);
        //���x�ɔ��f
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
        //������
        _myPhysic.velocity = _myPhysic.force;

        //��C��R���l��
        _myPhysic.velocity += _myPhysic.airResistance * -_myPhysic.velocity;

        return;
    }

    /// <summary>
    /// <para>ApplyVelocity</para>
    /// <para>���g�̑��x�����W�ɔ��f�����܂�</para>
    /// </summary>
    private void ApplyVelocity()
    {
        //�ݒ肳��Ă�����W�Œ�𔽉f

        //���ɂȂ����ݒ肳��Ă���
        if(_freezeStatus == Freeze.None)
        {
            //���x���f
            _transform.position += _myPhysic.velocity;
            return;
        }

        //���f�p
        Vector3 noFreeze = _vectorZero;
        //X�����Œ肳��Ă��Ȃ�
        if(_freezeStatus != Freeze.XOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.XtoZ)
        {
            //���
            noFreeze += _vectorRight * _myPhysic.velocity.x;
        }

        //Y�����Œ肳��Ă��Ȃ�
        if (_freezeStatus != Freeze.YOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.YtoZ)
        {
            //���
            noFreeze += _vectorUp * _myPhysic.velocity.y;
        }

        //Z�����Œ肳��Ă��Ȃ�
        if (_freezeStatus != Freeze.ZOnly && _freezeStatus != Freeze.XtoZ && _freezeStatus != Freeze.YtoZ)
        {
            //���
            noFreeze += _vectorForward * _myPhysic.velocity.z;
        }

        //�Œ肳��Ă��Ȃ������̂ݑ��x�𔽉f
        _transform.position += noFreeze;
    }

    /// <summary>
    /// <para>AddForce</para>
    /// <para>�����ɗ͂������܂�</para>
    /// </summary>
    /// <param name="add">�������</param>
    public void AddForce(Vector3 add)
    {
        //�͂����Z
        _myPhysic.force += add;
    }
    #endregion
}
