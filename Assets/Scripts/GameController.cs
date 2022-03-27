using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //ゲームステート
    enum State
    {
        GameStart,
        Move,
        Battle,
        GameOver
    }
    State state;

    //　プレイヤーを取得
    public Player player;

    //　タイピングソフトを取得
    public TypingSoft typingSoft;

    //　UIを取得
    public GameObject battleScreen;
    public Text stateText;
    public GameObject gameOverButton;
    public GameObject nextButton;
    public GameObject branchtext;
    //  エンドレスモードのスコアテキストを取得
    public Text endlessModeScoreText;

    //  バトルが始まったかどうか
    //public bool IsBattle;

    public AudioSource[] audios;

    //  BGMを取得
    public AudioClip battleClearAudio;
    public AudioClip gameOverAudio;
    public AudioClip gameClearAudio;

    private void Start()
    {
        audios = GetComponents<AudioSource>();
        GameStart();
    }

    private void LateUpdate()
    {
        //ゲームのステートごとにイベントを監視
        switch (state)
        {
            case State.GameStart:
                if (Input.GetButtonDown("Fire1")) Move();
                break;
            case State.Move:

                break;
            case State.Battle:
                if (GameManager.instance.isBattleClear)
                {
                    BattleClear();
                }
                break;
            case State.GameOver:
                break;

        }
    }

    private void Update()
    {
        if (GameManager.instance.isEndlessMode)
        {
            endlessModeScoreText.text = "Score " + GameManager.instance.endlessModeScore.ToString();
        }
    }
    private void GameStart()
    {
        //ラベルを更新
        stateText.text = "Press Ctrl to Start";

        //初期化処理
        battleScreen.SetActive(false);
        GameManager.instance.isBattle = false;
        player.playerMove = false;
        gameOverButton.SetActive(false);
        nextButton.SetActive(false);
        state = State.GameStart;
    }

    private void Move()
    {
        // bgmを管理
        audios[1].Stop();
        audios[0].Play();

        //ラベルを更新
        stateText.text = "";

        player.playerMove = true;
        state = State.Move;
    }

    public void Battle()
    {
        // bgmを管理
        audios[0].Stop();
        audios[1].Play();

        GameManager.instance.isBattle = true;
        battleScreen.SetActive(true);
        player.playerMove = false;


        state = State.Battle;
    }
    public void BattleClear()
    {
        // ラベルを更新
        stateText.text = "Battle Clear!";
        // BGMの設定
        audios[1].Stop();
        audios[2].PlayOneShot(battleClearAudio);

        battleScreen.SetActive(false);
        GameManager.instance.isBattle = false;
        GameManager.instance.isBattleClear = false;
        Invoke("Move", 1.0f);

    }
    public void GameOver()
    {
        // ラベルを更新
        stateText.text = "Game Over";

        if (GameManager.instance.isEndlessMode)
        {
            // ボタンを表示
            nextButton.SetActive(true);
        }
        else
        {   // ボタンを表示
            gameOverButton.SetActive(true);
        }

        // 結果の値を取得（正答数、誤字数、正答率、プレイヤーの残りHP）
        GameManager.instance.correctN = typingSoft.correctN;
        GameManager.instance.mistakeN = typingSoft.mistakeN;
        GameManager.instance.correctAR = typingSoft.correctAR;
        GameManager.instance.playerRemainingHp = player.playerHp;


        // BGMの設定
        audios[1].Stop();
        audios[2].PlayOneShot(gameOverAudio);

        battleScreen.SetActive(false);
        GameManager.instance.isBattle = false;
        state = State.GameOver;
    }

    public void GameClear()
    {
        // ボタンを表示
        nextButton.SetActive(true);

        // ラベルを更新
        stateText.text = "Game Clear!!";

        // BGMの設定
        audios[0].Stop();
        audios[2].PlayOneShot(gameClearAudio);

        // 結果の値を取得（正答数、誤字数、正答率、プレイヤーの残りHP）
        GameManager.instance.correctN = typingSoft.correctN;
        GameManager.instance.mistakeN = typingSoft.mistakeN;
        GameManager.instance.correctAR = typingSoft.correctAR;
        GameManager.instance.playerRemainingHp = player.playerHp;

        player.playerMove = false;
    }

    public void Branch()
    {
        player.playerMove = false;
        // 分岐点に到達した時の処理
        branchtext.SetActive(true);
    }
}
