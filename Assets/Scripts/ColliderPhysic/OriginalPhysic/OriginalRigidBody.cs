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
    //��ԍŌ�Ɏg�p�����Փ˃f�[�^
    private OtherPhysicData _usedCollisionData = default;

    #endregion

    #region �v���p�e�B
    //�d��
    public Vector3 MyGravity { get => _physicData.gravity; set => _physicData.gravity = value; }
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
        if (_colliderAccess.CheckCollisionToInterpolate(_gravity, true))
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
        Vector3 interpolateVelocity = ForceToVelocity(_physicData.force);

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

        //Debug.Log("-----------------------------------------------------");
        //���񏈗��ݒ�
        _isOnce = true;
        //�Փˏ�������
        CheckCollisionData(_physicData.colliderInfo.Material);
        //_physicData.force += _minGravity;

        //������̗͂��Œ�l�ȉ��ł���Ε����ɂ�����͂���������
        if (_physicData.force.sqrMagnitude <= PERMISSION_MINMAGUNITYDE)
        {
            _physicData.force = _vectorZero;
        }
        //������̗͂𑬓x�ɔ��f
        _physicData.velocity = ForceToVelocity(_physicData.force);

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
                //Debug.Log("once");
                _isOnce = false;
                //�d�͂ł����镨�̂�����
                _colliderAccess.CheckCollisionToInterpolate(ForceToVelocity(_gravity), true);
                //�ċN����
                CheckCollisionData(data);
            }
            else 
            {
                //���ۂ̗͂ɔ��f
                _physicData.force = ChangeForceByPhysic(_physicData, _usedCollisionData);
            }
            return;
        }
        //���񏈗��������
        _isOnce = false;

        //�g�p����Փ˃f�[�^���擾
        _usedCollisionData = CollisionPhysicManager.GetCollision(data);
        //Debug.Log("m" + _usedCollisionData.interpolate + "f" + _physicData.force.sqrMagnitude + "Us" + _usedCollisionData.point);
        //Debug.Log(_physicData.force);
        //���ۂ̗͂ɔ��f
        _physicData.force = ChangeForceByPhysic(_physicData, _usedCollisionData);
        //Debug.Log(_physicData.force);

        //�܂��o�^����Ă���
        if (CollisionPhysicManager.CheckWaitContains(data))
        {
            //�ċN����
            CheckCollisionData(data);
        }
        return;
    }

    /// <summary>
    /// <para>ChangeForceByPhysic</para>
    /// <para>�����ɂ�����͂�</para>
    /// </summary>
    /// <param name="myPhysic">���g�̕���</param>
    /// <param name="environment">�Փˊ�</param>
    /// <returns>���e����̗�</returns>
    private Vector3 ChangeForceByPhysic(PhysicData myPhysic, OtherPhysicData environment)
    {
        //Debug.Log(myPhysic.force);
        //�ʂɑ΂����������̗͎擾
        Vector3 horizontalForce = PhysicManager.HorizontalForceByPhysicMaterials(myPhysic, environment);
        //Debug.Log("Hor" + horizontalForce);
        //�ʂɑ΂����������̗͎擾
        Vector3 verticalForce = PhysicManager.VerticalForceByPhysicMaterials(myPhysic, environment);
        //Debug.Log("Repu" + verticalForce);
        //����
        Vector3 returnForce = horizontalForce + verticalForce;

        //���������ɓ����͂��Ȃ�
        if (verticalForce == _vectorZero)
        {
            //�ʂɐڂ���悤�ɗ͂�������
            Vector3 intrusion = PhysicManager.NoForceToCollision(myPhysic, environment);
            //Debug.Log("CollFor :" + intrusion);

            returnForce += intrusion;
        }

        return returnForce; 
    }

    /// <summary>
    /// <para>ForceToVelocity</para>
    /// <para>���̂ɗ^����ꂽ�͂𑬓x�ɕϊ����܂�</para>
    /// </summary>
    private Vector3 ForceToVelocity(Vector3 force)
    {
        //�ԋp�p
        Vector3 returnVelocity = force;
        
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
