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
    //�^�O�܂��̓V�[����
    private const string SCENECHANGER_TAG_NAME = "SceneChanger";
    private const string TITLESCENE_NAME = "Title";
    private const string ANIMATION_FADEOUT = "onFadeOut";
    private const string ANIMATION_FADEIN = "onFadeIn";
    //�X���[���[�V����
    private const float SLOW_TIME = 0.2f;
    private const float NORMAL_TIME = 1.0f;
    //�t�F�[�h�A�E�g�̏��v����
    private const float FADEOUT_TIME = 1.0f * SLOW_TIME;

    //�R���[�`���ҋ@���ԏ���
    readonly WaitForSeconds _wait = new WaitForSeconds(FADEOUT_TIME);

    //�v���C�\�ȃX�e�[�W��
    [SerializeField]
    private int _playableStage = 0;
    //�^�C�g���V�[����build�C���f�b�N�X
    private int _titleIndex = 0;

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
        _sceneAnim = GameObject.FindGameObjectWithTag(SCENECHANGER_TAG_NAME).GetComponent<Animator>();

        //�^�C�g���V�[���̃C���f�b�N�X���擾
        _titleIndex = (SceneManager.sceneCountInBuildSettings - 1) - _playableStage;

        //�V�[���؂�ւ���ɏ��������s
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        //�^�C�g���ǂݍ���
        SceneManager.LoadScene(TITLESCENE_NAME);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextStage();
        }
    }

    /// <summary>
    /// <para>FadeOutWait</para>
    /// <para>�t�F�[�h�A�E�g�ҋ@��A�V�[�������[�h���܂�</para>
    /// </summary>
    /// <param name="loadScene">�ǂݍ��ރV�[��</param>
    /// <returns></returns>
    private IEnumerator FadeOutWait(int loadScene)
    {
        //Debug.Log(loadScene);
        //�X���[���[�V����
        Time.timeScale = SLOW_TIME;
        //�A�j���[�V��������
        AnimationFade(false);

        //�A�j���[�V�����I���܂őҋ@
        yield return _wait;

        //Debug.Log(loadScene);
        //�ҋ@��A���̃V�[�������[�h
        SceneManager.LoadScene(loadScene);
    }

    /// <summary>
    /// <para>StageRetry</para>
    /// <para>�X�e�[�W���ċN���܂�</para>
    /// </summary>
    void IRetryble.StageRetry()
    {
        //�R���[�`���œ����V�[�������[�h
        StartCoroutine(FadeOutWait(SceneManager.GetActiveScene().buildIndex));
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
        AnimationFade(true);
    }

    /// <summary>
    /// <para>NextStage</para>
    /// <para>���̃X�e�[�W�֐i�s���܂�</para>
    /// </summary>
    public void NextStage()
    {
        //���݂̃V�[����buildIndex���擾
        int nowIndex = SceneManager.GetActiveScene().buildIndex;
        //���̃V�[����ݒ�
        nowIndex++;

        //Debug.Log(SceneManager.GetActiveScene().name + nowIndex);
        //Debug.Log("t:" + _titleIndex + "|" + (_titleIndex + _playableStage) + " n:" + nowIndex);
        //�ݒ肳��Ă���v���C�\�ȃX�e�[�W���ȏ�ł���
        if ((_titleIndex + _playableStage) < nowIndex)
        {
            //�^�C�g���ɖ߂�
            StartCoroutine(FadeOutWait(_titleIndex));
            return;
        }
        //���̃V�[�������[�h
        StartCoroutine(FadeOutWait(nowIndex));
    }

    /// <summary>
    /// <para>AnimationFade</para>
    /// <para>��ʂ̃t�F�[�h�J�ڂ𐧌䂵�܂�</para>
    /// </summary>
    /// <param name="inOut">�C���E�A�E�g����</param>
    private void AnimationFade(bool inOut)
    {
        //�t�F�[�h�C����
        if (inOut)
        {
            _sceneAnim.SetBool(ANIMATION_FADEOUT, false);
            _sceneAnim.SetBool(ANIMATION_FADEIN, true);
            return;
        }

        //�t�F�[�h�A�E�g
        _sceneAnim.SetBool(ANIMATION_FADEOUT, true);
        _sceneAnim.SetBool(ANIMATION_FADEIN, false);
    }
    #endregion
}
