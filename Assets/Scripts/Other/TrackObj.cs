/// -----------------------------------------------------------------
/// TrackObj.cs
/// 
/// �쐬���F2023/12/15
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObj : MonoBehaviour
{
    #region �ϐ�
    //�ǐՑ��x
    private const float TRACK_SPEED = 10f;
    //�ŏ��ǐՋ���
    private const float PERMISSION_TRACK_DISTANCE = 0.001f;

    //���g��Transform
    private Transform _transform = default;
    //�ǐՐ�
    [SerializeField]
    private Transform _track = default;

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
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //�ǐՏ���
        TrackToTransform();
    }

    /// <summary>
    /// <para>TrackToTransform</para>
    /// <para>�ΏۃI�u�W�F�N�g��ǐՂ��܂�</para>
    /// </summary>
    private void TrackToTransform()
    {
        //�ǐՋ����x�N�g�����擾
        Vector3 trackDistance = _track.position - _transform.position;

        //�ŏ��ǐՋ����ȉ��ł���
        if(trackDistance.sqrMagnitude < PERMISSION_TRACK_DISTANCE)
        {
            _transform.position = _track.position;
            return;
        }

        //�ǐ�
        _transform.position += trackDistance * TRACK_SPEED * Time.deltaTime;
    }
    #endregion
}
