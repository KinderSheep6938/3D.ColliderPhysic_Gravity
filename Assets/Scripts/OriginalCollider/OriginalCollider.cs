/// -----------------------------------------------------------------
/// OriginalCollider.cs�@Collider����
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;
using ColliderLibrary.Editor;
using ColliderLibrary.Manager;
using PhysicLibrary.Material;
using PhysicLibrary.DataManager;

public class OriginalCollider : MonoBehaviour, IColliderInfoAccessible
{
    #region �ϐ�
    //Collider�̍X�V�p�x
    private enum UpdateStatus
    {
        Once,       //��x����
        Update,     //Update��
        LateUpdate, //LateUpdate��
        FixedUpdate //FixedUpdate��
    }
    [SerializeField, Header("�X�V�p�x")]
    private UpdateStatus _updateStatus = UpdateStatus.FixedUpdate;

    //���̋��L����
    private bool _isSetWorld = false;

    //���g��Transform
    private Transform _transform = default;
    //���g��Renderer
    private MeshRenderer _renderer = default;
    //���g��Rigidbody
    private OriginalRigidBody _rigid = default;

    //���g�̕����������
    [SerializeField, Header("�����������")]
    private PhysicMaterials _physicMaterial = new();
    //���g�̓����蔻����
    [SerializeField, Header("�����蔻����")]
    private ColliderData _colliderData = new();
    //�Փˏ��ۑ��p
    private CollisionData _collisionData = new();
    //���W�⊮�p�̓����蔻��
    private ColliderData _interpolateCol = new();

    //�Փ˃}�e���A��
    [SerializeField]
    private Material _normal = default;
    [SerializeField]
    private Material _collision = default;


    #endregion

    #region �v���p�e�B
    //���g��Transform���擾
    Transform IColliderInfoAccessible.Collider { get => _colliderData.transform; }
    //�Փˏ��擾
    CollisionData IColliderInfoAccessible.Collision { get => _collisionData; }
    //�Փˈʒu�擾
    Vector3 IColliderInfoAccessible.Point { get => _collisionData.point - _collisionData.interpolate; }
    //���g�̕������
    PhysicMaterials IColliderInfoAccessible.material { get => _physicMaterial; }
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //������
        _transform = transform;
        _transform.hasChanged = false;
        _renderer = _transform.GetComponent<MeshRenderer>();
        _rigid = _transform.GetComponent<OriginalRigidBody>();

        //�⊮���̈ړ��ʂ�������
        _collisionData.interpolate = Vector3.zero;

        //Collider����
        _colliderData = ColliderEditor.SetColliderDataByCube(_transform);
        //Physic���Ɏ��g��ݒ�
        _physicMaterial.transform = _transform;

        //�Q�[���I�u�W�F�N�g������������Ă���ꍇ�͏�������߂�
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        //Collider�����Ǘ��}�l�[�W���[�ɐݒ�
        ColliderManager.SetColliderToWorld(_colliderData);
        //Physic�����Ǘ��}�l�[�W���[�ɐݒ�
        PhysicDataManager.SetPhysicToWorld(_physicMaterial);
        
    }
    
    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //�X�V�p�x�� Update �łȂ���Ώ������Ȃ�
        if (_updateStatus != UpdateStatus.Update)
        {
            return;
        }

        //Collider���̍X�V
        CheckColliderUpdate();
    }

    /// <summary>
    /// �X�V�㏈��
    /// </summary>
    private void LateUpdate()
    {
        //�X�V�p�x�� LateUpdate �łȂ���Ώ������Ȃ�
        if (_updateStatus != UpdateStatus.LateUpdate)
        {
            return;
        }

        //Collider���̍X�V
        CheckColliderUpdate();
    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    private void FixedUpdate()
    {
        //�X�V�p�x�� FixedUpdate �łȂ���Ώ������Ȃ�
        if (_updateStatus != UpdateStatus.FixedUpdate)
        {
            return;
        }

        //Collider���̍X�V
        CheckColliderUpdate();
    }

    /// <summary>
    /// <para>CheckColliderUpdate</para>
    /// <para>Collider�����X�V����K�v�����邩�������܂�</para>
    /// <para>�܂��K�v���������ꍇ�́A�X�V�������s���܂�</para>
    /// </summary>
    private void CheckColliderUpdate()
    {
        //Transform��񂪕ς���ĂȂ�
        if (!_transform.hasChanged)
        {
            return;
        }

        //Transform�Ɋ�Â���Collider���쐬����
        _colliderData = ColliderEditor.SetColliderDataByCube(_transform);

        //���W�⊮����Փˊm�F
        CheckCollisionToInterpolate();

        //�ύX�t���O������
        _transform.hasChanged = false;
    }

    /// <summary>
    /// <para>CheckCollisionToInterpolate</para>
    /// <para>Collider�̏Փ˔�����������܂�</para>
    /// <para>�܂��ړ��⊮������ꍇ�́A�ړ�����⊮������ԂŌ������܂�</para>
    /// </summary>
    private void CheckCollisionToInterpolate()
    {
        //�⊮�����Ȃ� �܂��� RigidBody���t�����Ă��Ȃ�
        if(_rigid == default || _rigid.Interpolate == OriginalRigidBody.InterpolateStatus.None)
        {
            //�Փ˔�����擾����
            _collisionData = ColliderManager.CheckCollision(_colliderData);

        }
        else
        {
            //�⊮�ϐ��Ɏ��g�̏���ݒ�
            _interpolateCol = _colliderData;
            //���x����⊮����
            _interpolateCol.position += _rigid.Velocity;
            for(int i = 0;i < EdgeLineManager.MaxEdge; i++)
            {
                _interpolateCol.edgePos[i] += _rigid.Velocity;
            }

            //�Փ˔�����擾
            _collisionData = ColliderManager.CheckCollision(_interpolateCol);
            //�⊮���̈ړ��ʂ�ݒ�
            _collisionData.interpolate = _rigid.Velocity;
        }

        //�f�o�b�O�p�����ڕύX
        if (_collisionData.flag)
        {
            _renderer.material = _collision;
        }
        else
        {
            _renderer.material = _normal;
        }
    }

    /// <summary>
    /// �`�揈��
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmo�`��
        //�������p
        Matrix4x4 cache = Gizmos.matrix;
        //�F�ݒ�
        Gizmos.color = Color.green;
        //���[�J���ݒ�
        Gizmos.matrix = Matrix4x4.TRS(transform.position,transform.rotation,transform.lossyScale);
        //�`��
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        //�O���[�o���ݒ�
        Gizmos.matrix = cache;
    }

    /// <summary>
    /// �L��������
    /// </summary>
    private void OnEnable()
    {
        //Collider�����Ǘ��}�l�[�W���[�ɐݒ�
        ColliderManager.SetColliderToWorld(_colliderData);
        //Physic�����Ǘ��}�l�[�W���[�ɐݒ�
        PhysicDataManager.SetPhysicToWorld(_physicMaterial);
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void OnDisable()
    {
        //Collider�����Ǘ��}�l�[�W���[����폜
        ColliderManager.RemoveColliderToWorld(_colliderData);
        //Physic�����Ǘ��}�l�[�W���[����폜
        PhysicDataManager.RemovePhysicToWorld(_physicMaterial);
    }
    #endregion
}
