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

public class OriginalCollider : MonoBehaviour
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

    //�Փ˔���
    [SerializeField, Header("�Փ˔���"),ReadOnly]
    private bool _isCollision = false;

    //���g��Transform
    private Transform _transform = default;
    //���g��ColliderData
    [SerializeField,Header("����������")]
    private ColliderData _myCol = new();


    #endregion

    #region �v���p�e�B
    //ColliderData�擾
    public ColliderData Data { get => _myCol; }
    //Transform�擾
    public Transform MyTransform { get => _transform; }
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

        //Collider����
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);
        //Collider�����Ǘ��}�l�[�W���[�ɐݒ�
        ColliderManager.SetColliderToWorld(this);
        
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
        CheckColliderUpdata();
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
        CheckColliderUpdata();
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
        CheckColliderUpdata();
    }

    /// <summary>
    /// <para>CheckColliderUpdata</para>
    /// <para>Collider�����X�V����K�v�����邩�������܂�</para>
    /// <para>�܂��K�v���������ꍇ�́A�X�V�������s���܂�</para>
    /// </summary>
    private void CheckColliderUpdata()
    {
        //Transform��񂪕ς���ĂȂ�
        if (!_transform.hasChanged)
        {
            return;
        }

        //Transform�Ɋ�Â���Collider���쐬����
        _myCol = ColliderEditor.SetColliderDataByCube(_transform);

        //�Փ˔�����擾����
        _isCollision = ColliderManager.CheckCollision(this);

        //�ύX�t���O������
        _transform.hasChanged = false;
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
        ColliderManager.SetColliderToWorld(this);
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void OnDisable()
    {
        //Collider�����Ǘ��}�l�[�W���[����폜
        ColliderManager.RemoveColliderToWorld(this);
    }

    #endregion
}
