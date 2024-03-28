// -----------------------------------------------------------------
/// SoundPlayer.cs
/// 
/// �쐬���F2024/03/28
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    #region �ϐ�
    [SerializeField, Header("���ʉ�")]
    private AudioClip _se = default;
    
    // AudioSource
    private AudioSource _audioSource = default;
    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// <para>Play</para>
    /// <para>���ʉ����Đ����܂�</para>
    /// </summary>
    public void Play()
    {
        _audioSource.PlayOneShot(_se);
    }
    #endregion
}
