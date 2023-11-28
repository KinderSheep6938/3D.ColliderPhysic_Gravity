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
    //�ŏ����x
    private const float PERMISSION_MINMAGUNITYDE= 0.00001f;

    //�Vector
    private readonly Vector3 _vectorZero = Vector3.zero;
    private readonly Vector3 _vectorRight = Vector3.right;
    private readonly Vector3 _vectorUp = Vector3.up;
    private readonly Vector3 _vectorForward = Vector3.forward;

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

    //�ʂ̏Փ˔���
    private bool _onSurface = false;
    //�ŏ��d�͉����x
    private Vector3 _minGravity = default;

    //���g�̕����f�[�^
    [SerializeField]
    private PhysicData _physicData = new(PhysicManager.CommonMass, PhysicManager.CommonGravity);
    //���g��Collider
    private IColliderInfoAccessible _colliderAccess = default;
    //���g��Transform
    private Transform _transform = default;
    #endregion

    #region �v���p�e�B
    //���݂̑��x
    public Vector3 Velocity { get => _physicData.velocity; }
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
        _physicData.colliderInfo = _colliderAccess;


        //�Q�[���I�u�W�F�N�g������������Ă���ꍇ�͏�������߂�
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _physicData.force += new Vector3(0, 10f, 0);
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

        //�d�͐���
        Gravity();

        //�ՓˁE��������
        ColliderRepulsion();

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
        Vector3 gravity = PhysicManager.Gravity(_physicData);

        //�d�͂ɂ���ďՓ˂���
        if (_colliderAccess.CheckCollisionToInterpolate(gravity))
        {
            //�ʏՓ˔����ݒ�
            _onSurface = true;
            return;
        }
        //�ʏՓ˔��������
        _onSurface = false;
        //�d�͂����Z
        _physicData.force += gravity;

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Collider���Փ˂����ۂ̔���������s���܂�</para>
    /// </summary>
    private void ColliderRepulsion()
    {
        //���݂������Ă���͂��Ȃ�
        if(_physicData.force == _vectorZero)
        {
            //���x��������
            _physicData.velocity = _vectorZero;
            return;
        }

        //���݂������Ă���͂𑬓x�ɕϊ�
        Vector3 interpolateVelocity = ForceToVelocity();

        //Collider���ݒ肳��Ă��Ȃ�
        if (_colliderAccess == default)
        {
            //���̂܂ܑ��x�Ƃ��ĕt��
            _physicData.velocity = interpolateVelocity;
            return;
        }

        //�ʐڐG���肪�Ȃ� ���� ���ݑ��x�ŏՓ˔��肪�N���Ȃ�
        if (!_onSurface && !_colliderAccess.CheckCollisionToInterpolate(interpolateVelocity))
        {
            //���̂܂ܑ��x�Ƃ��ĕt��
            _physicData.velocity = interpolateVelocity;
            return;
        }

        //Debug.Log("col");
        //�ŏ��d�͉����x�ݒ�
        //_minGravity = PhysicManager.Gravity(_physicData);
        //_physicData.force += _minGravity;
        //�Փ˂��������������āA���x���Z�o
        _physicData.force = PhysicManager.ChangeForceByPhysicMaterials(_physicData);
        //_physicData.force += _minGravity;

        //������̗͂��Œ�l�ȉ��ł���Ε����ɂ�����͂���������
        if (_physicData.force.sqrMagnitude <= PERMISSION_MINMAGUNITYDE)
        {
            _physicData.force = _vectorZero;
        }
        //������̗͂𑬓x�ɔ��f
        _physicData.velocity = ForceToVelocity();

        //�Փ˂��Ă��镨�̂̏󋵂��������āA���x���Z�o
        //_myPhysic.velocity = -(_myPhysic.reboundRatio * _myPhysic.velocity);
    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>���̂ɗ^����ꂽ�͂𑬓x�ɕϊ����܂�</para>
    /// </summary>
    private Vector3 ForceToVelocity()
    {
        //�����ɂ������Ă���͂��Ȃ�
        if (_physicData.force == _vectorZero)
        {
            return _vectorZero;
        }

        //�ԋp�p
        Vector3 returnVelocity = _physicData.force;
        
        //��C��R���l��
        returnVelocity += _physicData.airResistance * -returnVelocity;

        return returnVelocity;
    }

    /// <summary>
    /// <para>ApplyVelocity</para>
    /// <para>���g�̑��x�����W�ɔ��f�����܂�</para>
    /// </summary>
    private void ApplyVelocity()
    {
        //�����������x���Ȃ��Ƃ��͏������Ȃ�
        if(_physicData.velocity == _vectorZero)
        {
            return;
        }

        //�ݒ肳��Ă�����W�Œ�𔽉f

        //���ɂȂ����ݒ肳��Ă���
        if(_freezeStatus == Freeze.None)
        {
            //���x���f
            _transform.position += _physicData.velocity;
            return;
        }

        //���f�p
        Vector3 noFreeze = _vectorZero;
        //X�����Œ肳��Ă��Ȃ�
        if(_freezeStatus != Freeze.XOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.XtoZ)
        {
            //���
            noFreeze += _vectorRight * _physicData.velocity.x;
        }

        //Y�����Œ肳��Ă��Ȃ�
        if (_freezeStatus != Freeze.YOnly && _freezeStatus != Freeze.XtoY && _freezeStatus != Freeze.YtoZ)
        {
            //���
            noFreeze += _vectorUp * _physicData.velocity.y;
        }

        //Z�����Œ肳��Ă��Ȃ�
        if (_freezeStatus != Freeze.ZOnly && _freezeStatus != Freeze.XtoZ && _freezeStatus != Freeze.YtoZ)
        {
            //���
            noFreeze += _vectorForward * _physicData.velocity.z;
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
        _physicData.force += add;
    }
    #endregion
}
