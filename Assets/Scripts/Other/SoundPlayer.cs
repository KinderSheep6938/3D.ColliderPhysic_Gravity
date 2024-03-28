// -----------------------------------------------------------------
/// SoundPlayer.cs
/// 
/// 作成日：2024/03/28
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    #region 変数
    [SerializeField, Header("効果音")]
    private AudioClip _se = default;
    
    // AudioSource
    private AudioSource _audioSource = default;
    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// <para>Play</para>
    /// <para>効果音を再生します</para>
    /// </summary>
    public void Play()
    {
        _audioSource.PlayOneShot(_se);
    }
    #endregion
}
