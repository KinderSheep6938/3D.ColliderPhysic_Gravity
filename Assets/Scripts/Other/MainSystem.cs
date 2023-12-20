/// -----------------------------------------------------------------
/// MainSystem.cs
/// 
/// �쐬���F2023/12/15
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour, IRetryble
{
    #region �ϐ�
    //�X���[���[�V����
    private const float SLOW_TIME = 0.2f;
    private const float NORMAL_TIME = 1.0f;
    //�t�F�[�h�A�E�g�̏��v����
    private const float FADEOUT_TIME = 1.0f * SLOW_TIME;

    //�V�[���؂�ւ��̃A�j���[�^�[
    private Animator _sceneAnim = default;
    

    
    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //�폜����Ȃ��悤�ɐݒ�
        DontDestroyOnLoad(gameObject);

        //������
        _sceneAnim = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<Animator>();

        //�V�[���؂�ւ���ɏ��������s
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
    /// <para>FadeOutWait</para>
    /// <para>�t�F�[�h�A�E�g�ҋ@��A�V�[�������[�h���܂�</para>
    /// </summary>
    /// <param name="loadScene">���̃V�[��</param>
    /// <returns></returns>
    private IEnumerator FadeOutWait(Scene loadScene)
    {
        //�A�j���[�V��������
        _sceneAnim.SetBool("onFadeOut", true);
        _sceneAnim.SetBool("onFadeIn", false);

        //�A�j���[�V�����I���܂őҋ@
        yield return new WaitForSeconds(FADEOUT_TIME);

        //�ҋ@��A���̃V�[�������[�h
        SceneManager.LoadScene(loadScene.name);
    }

    /// <summary>
    /// <para>StageRetry</para>
    /// <para>�X�e�[�W���ċN���܂�</para>
    /// </summary>
    void IRetryble.StageRetry()
    {
        //�v���C���[�����s��
        FindObjectOfType<Player>().enabled = false;
        //�X���[���[�V����
        Time.timeScale = SLOW_TIME;

        //�R���[�`���œ����V�[�������[�h
        StartCoroutine(FadeOutWait(SceneManager.GetActiveScene()));
    }

    /// <summary>
    /// <para>ActiveSceneChanged</para>
    /// <para>�V�[���̐؂�ւ��I�����ɌĂяo����܂�</para>
    /// </summary>
    /// <param name="thisScene">���݂̃V�[��</param>
    /// <param name="nextScene">�؂�ւ���̃V�[��</param>
    private void ActiveSceneChanged(Scene thisScene, Scene nextScene)
    {
        //�ʏ�Đ�
        Time.timeScale = NORMAL_TIME;
        //�A�j���[�V��������
        _sceneAnim.SetBool("onFadeOut", false);
        _sceneAnim.SetBool("onFadeIn", true);
    }

    /// <summary>
    /// <para>NextStage</para>
    /// <para>���̃X�e�[�W�֐i�s���܂�</para>
    /// </summary>
    public void NextStage()
    {
        //�v���C���[�����s��
        FindObjectOfType<Player>().enabled = false;

        //���݂̃V�[����buildIndex���擾
        int nowIndex = SceneManager.GetActiveScene().buildIndex;
        //���̃V�[�����擾
        Scene next = SceneManager.GetSceneByBuildIndex(nowIndex++);

        //�R���[�`���Ŏ��̃V�[�������[�h
        StartCoroutine(FadeOutWait(next));

    }
    #endregion
}
