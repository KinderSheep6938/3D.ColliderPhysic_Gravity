/// -----------------------------------------------------------------
/// Cannon.cs
/// 
/// �쐬���F2023/12/20
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    #region �ϐ�
    //��b�x�N�g��
    private readonly Vector3 _vectorZero = Vector3.zero;

    [SerializeField, Header("�e��")]
    private Vector3 _bulletMoveValue = default;
    [SerializeField, Header("�e�̑��ݎ���")]
    private float _bulletLifeTime = default;
    [SerializeField, Header("�e�I�u�W�F")]
    private GameObject _bullet = default;

    //�e�̈ړ�����
    private MoveObj _bulletMove = default;
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

        //�e����
        _bullet = Instantiate(_bullet,_transform.position,Quaternion.identity);
        //�e�̈ړ�����N���X���擾
        _bulletMove = _bullet.GetComponent<MoveObj>();

        //�R���[�`���J�n
        StartCoroutine(LoopFire());
    }

    /// <summary>
    /// �X�V�O����
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// <para>LoopFire</para>
    /// <para>�e�̔��ˏ����������Ȃ��܂�</para>
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoopFire()
    {
        //�J��Ԃ�
        while (true)
        {
            //�e�̈ʒu��������
            _bullet.transform.position = _transform.position;
            _bulletMove.Speed = SpeedLocalize(_bulletMoveValue);

            //�������Ԃ܂őҋ@
            yield return new WaitForSeconds(_bulletLifeTime);
        }
    }

    /// <summary>
    /// <para>SpeedLocalize</para>
    /// <para>�e�̈ړ��ʂ��C�̌����ɍ��킹�ă��[�J�������܂�</para>
    /// </summary>
    /// <param name="speed">�e�̈ړ���</param>
    /// <returns>���[�J�������ꂽ�ړ���</returns>
    private Vector3 SpeedLocalize(Vector3 speed)
    {
        //�ԋp�p���v�l
        Vector3 sum = _vectorZero;

        //�e���̈ړ��ʂ��v�Z
        sum += _transform.right * speed.x;
        sum += _transform.up * speed.y;
        sum += _transform.forward * speed.z;

        //�ԋp
        return sum;
    }

    #endregion
}
