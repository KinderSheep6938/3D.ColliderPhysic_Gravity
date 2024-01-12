/// -----------------------------------------------------------------
/// MainSystem.cs
/// 
/// 作成日：2023/12/15
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour, IRetryble
{
    #region 変数
    //タグまたはシーン名
    private const string SCENECHANGER_TAG_NAME = "SceneChanger";
    private const string TITLESCENE_NAME = "Title";
    private const string ANIMATION_FADEOUT = "onFadeOut";
    private const string ANIMATION_FADEIN = "onFadeIn";
    //スローモーション
    private const float SLOW_TIME = 0.2f;
    private const float NORMAL_TIME = 1.0f;
    //フェードアウトの所要時間
    private const float FADEOUT_TIME = 1.0f * SLOW_TIME;

    //コルーチン待機時間処理
    readonly WaitForSeconds _wait = new WaitForSeconds(FADEOUT_TIME);

    //プレイ可能なステージ数
    [SerializeField]
    private int _playableStage = 0;
    //タイトルシーンのbuildインデックス
    private int _titleIndex = 0;

    //シーン切り替えのアニメーター
    private Animator _sceneAnim = default;
    #endregion

    #region プロパティ

    #endregion

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //削除されないように設定
        DontDestroyOnLoad(gameObject);

        //初期化
        _sceneAnim = GameObject.FindGameObjectWithTag(SCENECHANGER_TAG_NAME).GetComponent<Animator>();

        //タイトルシーンのインデックスを取得
        _titleIndex = (SceneManager.sceneCountInBuildSettings - 1) - _playableStage;

        //シーン切り替え後に処理を実行
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        //タイトル読み込み
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
    /// <para>フェードアウト待機後、シーンをロードします</para>
    /// </summary>
    /// <param name="loadScene">読み込むシーン</param>
    /// <returns></returns>
    private IEnumerator FadeOutWait(int loadScene)
    {
        //Debug.Log(loadScene);
        //スローモーション
        Time.timeScale = SLOW_TIME;
        //アニメーション制御
        AnimationFade(false);

        //アニメーション終了まで待機
        yield return _wait;

        //Debug.Log(loadScene);
        //待機後、次のシーンをロード
        SceneManager.LoadScene(loadScene);
    }

    /// <summary>
    /// <para>StageRetry</para>
    /// <para>ステージを再起します</para>
    /// </summary>
    void IRetryble.StageRetry()
    {
        //コルーチンで同じシーンをロード
        StartCoroutine(FadeOutWait(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    /// <para>ActiveSceneChanged</para>
    /// <para>シーンの切り替え終了時に呼び出されます</para>
    /// </summary>
    /// <param name="thisScene">現在のシーン</param>
    /// <param name="nextScene">切り替え後のシーン</param>
    private void ActiveSceneChanged(Scene thisScene, Scene nextScene)
    {
        //通常再生
        Time.timeScale = NORMAL_TIME;
        //アニメーション制御
        AnimationFade(true);
    }

    /// <summary>
    /// <para>NextStage</para>
    /// <para>次のステージへ進行します</para>
    /// </summary>
    public void NextStage()
    {
        //現在のシーンのbuildIndexを取得
        int nowIndex = SceneManager.GetActiveScene().buildIndex;
        //次のシーンを設定
        nowIndex++;

        //Debug.Log(SceneManager.GetActiveScene().name + nowIndex);
        //Debug.Log("t:" + _titleIndex + "|" + (_titleIndex + _playableStage) + " n:" + nowIndex);
        //設定されているプレイ可能なステージ数以上である
        if ((_titleIndex + _playableStage) < nowIndex)
        {
            //タイトルに戻す
            StartCoroutine(FadeOutWait(_titleIndex));
            return;
        }
        //次のシーンをロード
        StartCoroutine(FadeOutWait(nowIndex));
    }

    /// <summary>
    /// <para>AnimationFade</para>
    /// <para>画面のフェード遷移を制御します</para>
    /// </summary>
    /// <param name="inOut">イン・アウト判定</param>
    private void AnimationFade(bool inOut)
    {
        //フェードインか
        if (inOut)
        {
            _sceneAnim.SetBool(ANIMATION_FADEOUT, false);
            _sceneAnim.SetBool(ANIMATION_FADEIN, true);
            return;
        }

        //フェードアウト
        _sceneAnim.SetBool(ANIMATION_FADEOUT, true);
        _sceneAnim.SetBool(ANIMATION_FADEIN, false);
    }
    #endregion
}
