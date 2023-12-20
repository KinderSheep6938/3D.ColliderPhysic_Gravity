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
    //スローモーション
    private const float SLOW_TIME = 0.2f;
    private const float NORMAL_TIME = 1.0f;
    //フェードアウトの所要時間
    private const float FADEOUT_TIME = 1.0f * SLOW_TIME;

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
        _sceneAnim = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<Animator>();

        //シーン切り替え後に処理を実行
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// <para>FadeOutWait</para>
    /// <para>フェードアウト待機後、シーンをロードします</para>
    /// </summary>
    /// <param name="loadScene">次のシーン</param>
    /// <returns></returns>
    private IEnumerator FadeOutWait(Scene loadScene)
    {
        //アニメーション制御
        _sceneAnim.SetBool("onFadeOut", true);
        _sceneAnim.SetBool("onFadeIn", false);

        //アニメーション終了まで待機
        yield return new WaitForSeconds(FADEOUT_TIME);

        //待機後、次のシーンをロード
        SceneManager.LoadScene(loadScene.name);
    }

    /// <summary>
    /// <para>StageRetry</para>
    /// <para>ステージを再起します</para>
    /// </summary>
    void IRetryble.StageRetry()
    {
        //プレイヤー操作を不可に
        FindObjectOfType<Player>().enabled = false;
        //スローモーション
        Time.timeScale = SLOW_TIME;

        //コルーチンで同じシーンをロード
        StartCoroutine(FadeOutWait(SceneManager.GetActiveScene()));
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
        _sceneAnim.SetBool("onFadeOut", false);
        _sceneAnim.SetBool("onFadeIn", true);
    }

    /// <summary>
    /// <para>NextStage</para>
    /// <para>次のステージへ進行します</para>
    /// </summary>
    public void NextStage()
    {
        //プレイヤー操作を不可に
        FindObjectOfType<Player>().enabled = false;

        //現在のシーンのbuildIndexを取得
        int nowIndex = SceneManager.GetActiveScene().buildIndex;
        //次のシーンを取得
        Scene next = SceneManager.GetSceneByBuildIndex(nowIndex++);

        //コルーチンで次のシーンをロード
        StartCoroutine(FadeOutWait(next));

    }
    #endregion
}
