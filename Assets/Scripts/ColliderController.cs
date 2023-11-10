/// -----------------------------------------------------------------
/// ColliderController.cs�@Collider����
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;

public class ColliderController : MonoBehaviour
{
    #region �ϐ�
    [SerializeField, Header("StaticCollider")]
    private bool _isStatic = false;
    //���g��Transform
    private Transform _transform = default;
    //���g��ColliderData
    [SerializeField,Header("ColliderData")]
    private ColliderData _myCol = new();

    #endregion

    #region �v���p�e�B
    //ColliderData�擾
    public ColliderData Data { get => _myCol; }
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

    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    private void FixedUpdate()
    {
        //�ÓI�ł���ꍇ�͏������Ȃ�
        if (!_isStatic)
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

        //�ύX�t���O������
        _transform.hasChanged = false;
    }

    /// <summary>
    /// <para>CheckCollision</para>
    /// <para>Collider���Փ˂������ǂ����������܂�</para>
    /// </summary>
    private void CheckCollision()
    {
        
    }

    /// <summary>
    /// �`�揈��
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmo�`��
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_myCol.position, _myCol.localScale);
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
