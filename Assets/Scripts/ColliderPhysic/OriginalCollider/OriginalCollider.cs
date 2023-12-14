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
using ColliderLibrary.DataManager;
using PhysicLibrary.Material;
using PhysicLibrary.CollisionPhysic;

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

    //���g��Transform
    private Transform _transform = default;
    //���g��Renderer
    private MeshRenderer _renderer = default;

    //���g�̕����������
    [SerializeField, Header("�����������")]
    private PhysicMaterials _physicMaterial = new();
    //���g�̓����蔻����
    [SerializeField, Header("�����蔻����")]
    private ColliderData _colliderData = new();
    //�Փˏ��ۑ��p
    [SerializeField, Header("�Փˏ��")]
    private bool _onCollision = false;

    //�Փ˃}�e���A��
    [SerializeField]
    private Material _normal = default;
    [SerializeField]
    private Material _collision = default;
    #endregion

    #region �v���p�e�B
    //���g�̒��_���W���X�g
    Vector3[] IColliderInfoAccessible.Edge { get => _colliderData.edgePos; }

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

        //Physic���ɓ��͒l��ݒ�
        _physicMaterial = new(
            GetComponent<OriginalRigidBody>(),
            ref _transform, 
            ref _physicMaterial.dynamicDrug, 
            ref _physicMaterial.staticDrug, 
            ref _physicMaterial.bounciness, 
            ref _physicMaterial.drugCombine, 
            ref _physicMaterial.bounceCombine
            );
        //Collider����
        _colliderData = ColliderEditor.SetColliderDataByCube(_physicMaterial);


        //�Q�[���I�u�W�F�N�g������������Ă���ꍇ�͏�������߂�
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        //Collider�����Ǘ��}�l�[�W���[�ɐݒ�
        ColliderDataManager.SetColliderToWorld(_colliderData);
        
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
        //�f�o�b�O�p�����ڕύX
        if (_onCollision)
        {
            _renderer.material = _collision;
        }
        else
        {
            _renderer.material = _normal;
        }

        //���܂łɏՓ˂���������
        _onCollision = CollisionPhysicManager.CheckWaitContains(_physicMaterial);

        //Transform��񂪕ς���ĂȂ� ���� ���܂łɏՓ˂��Ȃ�
        if (!_transform.hasChanged)
        {
            return;
        }

        //�Փˊm�F
        CheckCollision();

        //�ύX�t���O������
        _transform.hasChanged = false;
    }

    /// <summary>
    /// <para>CheckCollision</para>
    /// <para>Collider�̏Փ˔�����������܂�</para>
    /// </summary>
    private void CheckCollision()
    {
        //Transform�Ɋ�Â���Collider���쐬����
        _colliderData = ColliderEditor.SetColliderDataByCube(_physicMaterial);

        //���ɏՓ˔��肪����
        if (_onCollision)
        {
            return;
        }
        //�Փ˔�����擾����
        _onCollision = ColliderManager.CheckCollision(_colliderData);
    }

    /// <summary>
    /// <para>CheckCollisionToInterpolate</para>
    /// <para>Collider�̏Փ˔����⊮����Ō������܂�</para>
    /// </summary>
    /// <param name="velocity">���݂̑��x</param>
    /// <returns>�Փ˔���</returns>
    bool IColliderInfoAccessible.CheckCollisionToInterpolate(Vector3 velocity, bool saveCollision)
    {
        Debug.DrawLine(_colliderData.position, _colliderData.position + velocity, Color.yellow);

        //�⊮�ϐ��Ɏ��g�̏���ݒ�
        ColliderData interpolateCol = _colliderData;
        //���_���W���X�g���Q�Ɠn���ł͂Ȃ��A�l�n���ɕϊ�
        interpolateCol.edgePos = (Vector3[])_colliderData.edgePos.Clone();
        //���x����⊮����
        interpolateCol.position += velocity;
        for (int i = 0; i < EdgeLineManager.MaxEdge; i++)
        {
            interpolateCol.edgePos[i] += velocity;
        }

        //�Փ˔�����擾
        _onCollision = ColliderManager.CheckCollision(interpolateCol, velocity, saveCollision);

        //�Փ˔����ԋp
        return _onCollision;
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
        ColliderDataManager.SetColliderToWorld(_colliderData);
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void OnDisable()
    {
        //Collider�����Ǘ��}�l�[�W���[����폜
        ColliderDataManager.RemoveColliderToWorld(_colliderData);
    }

    /// <summary>
    /// �폜�㏈��
    /// </summary>
    private void OnDestroy()
    {
        //Collider�����Ǘ��}�l�[�W���[����폜
        ColliderDataManager.RemoveColliderToWorld(_colliderData);
    }
    #endregion
}
