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
using PhysicLibrary.Material;
using PhysicLibrary.CollisionPhysic;

public class OriginalRigidBody : MonoBehaviour
{
    #region �ϐ�
    //�ŏ����x
    private const float PERMISSION_MINMAGUNITYDE = 0.0001f;

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

    //�ċA�������񏈗�����
    private bool _isOnce = true;
    //�ʂ̏Փ˔���
    private bool _onSurface = false;
    //�d�͉����x
    private Vector3 _gravity = default;

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
        _gravity = PhysicManager.Gravity(_physicData);

        //�d�͂ɂ���ďՓ˂���
        if (_colliderAccess.CheckCollisionToInterpolate(_gravity))
        {
            //�ʏՓ˔����ݒ�
            _onSurface = true;
            return;
        }
        //�ʏՓ˔��������
        _onSurface = false;
        //�d�͂����Z
        _physicData.force += _gravity;

    }

    /// <summary>
    /// <para>Requlsion</para>
    /// <para>Collider���Փ˂����ۂ̔���������s���܂�</para>
    /// </summary>
    private void ColliderRepulsion()
    {
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
        if (!_onSurface && !_colliderAccess.CheckCollisionToInterpolate(interpolateVelocity, true))
        {
            //���̂܂ܑ��x�Ƃ��ĕt��
            _physicData.velocity = interpolateVelocity;
            return;
        }

        Debug.Log("-----------------------------------------------------");
        //���񏈗��ݒ�
        _isOnce = true;
        //�Փˏ�������
        CheckCollisionData(_physicData.colliderInfo.material);
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
    /// <para>CheckCollisionData</para>
    /// <para>�����Ώۂ��o�^����Ă���Փ˃f�[�^���甽����̗͂��Z�o���܂�</para>
    /// <para>�ċN����</para>
    /// </summary>
    /// <param name="data">�����Ώ�</param>
    private void CheckCollisionData(PhysicMaterials data)
    {
        //�o�^����Ă��Ȃ��ꍇ�͏������Ȃ�
        if (!CollisionPhysicManager.CheckWaitContains(data))
        {
            //���񏈗���
            if (_isOnce)
            {
                _isOnce = false;
                //�d�͂ł����镨�̂�����
                _colliderAccess.CheckCollisionToInterpolate(_gravity, true);
                //�ċN����
                CheckCollisionData(data);
            }
            return;
        }
        //���񏈗��������
        _isOnce = false;

        //�g�p����Փ˃f�[�^���擾
        OtherPhysicData use = CollisionPhysicManager.GetCollision(data);
        Debug.Log("m" + use.interpolate + "f" + _physicData.force.sqrMagnitude + "Us" + use.point);
        Debug.Log(_physicData.force);
        //���C�͎擾
        Vector3 friction = PhysicManager.FrictionForceByPhysicMaterials(_physicData, use);
        _physicData.force += friction;
        Debug.Log("Fri" + friction);
        //�����͎擾
        Vector3 repulsion = PhysicManager.RepulsionForceByPhysicMaterials(_physicData, use);
        Debug.Log("Repu" + repulsion);
        //���ۂ̗͂ɔ��f
        _physicData.force = repulsion;
        Debug.Log(_physicData.force);


        //�߂荞�ݐ���
        Vector3 intrusion = PhysicManager.NoForceToCollision(_physicData, use);
        Debug.Log("CollFor :" + intrusion);
        //�߂荞�ݖh�~�͂�ݒ�
        _physicData.force += intrusion;
        

        //�܂��o�^����Ă���
        if (CollisionPhysicManager.CheckWaitContains(data))
        {
            //�ċN����
            CheckCollisionData(data);
        }
        return;

    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>���̂ɗ^����ꂽ�͂𑬓x�ɕϊ����܂�</para>
    /// </summary>
    private Vector3 ForceToVelocity()
    {
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
