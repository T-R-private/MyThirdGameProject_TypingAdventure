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

    // プレイヤーを取得
    private Player player;
    // ChargePlayerを取得
    private KingPlayer kingPlayer;

    //　タイピングソフトを取得
    public TypingManager typingManager;

    //　UIを取得
    public GameObject battleScreen;
    public Text stateText;
    public GameObject gameOverButton;
    public GameObject nextButton;
    public GameObject branchtext;
    //  エンドレスモードのスコアテキストを取得
    public Text endlessModeScoreText;
    //  PowerUpの時間を表示するテキスト
    public Text powerUpCountText;

    //  バトルが始まったかどうか
    //public bool IsBattle;

    // GameManager, SoundManagerの省略のための変数
    GameManager gameManager;
    SoundManager soundManager;

    private void Start()
    {
        gameManager = GameManager.instance;
        soundManager = SoundManager.instance;
        GameStart();
    }

    void SetPlayer(GameManager.PlayerType playerType)
    {
        switch (playerType)
        {
            case GameManager.PlayerType.PLAYER:
                player = GameObject.Find("Player").GetComponent<Player>();
                break;
            case GameManager.PlayerType.KINGPLAYER:
                kingPlayer = GameObject.Find("KingPlayer").GetComponent<KingPlayer>();
                break;
            default:
                break;
        }
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
                if (gameManager.isBattleClear)
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
        if (gameManager.isEndlessMode)
        {
            endlessModeScoreText.text = "Score " + gameManager.endlessModeScore.ToString();
        }
    }

    private void GameStart()
    {
        //ラベルを更新
        stateText.text = "Press Ctrl to Start";

        //初期化処理
        battleScreen.SetActive(false);
        gameManager.isBattle = false;
        player.playerMove = false;
        gameOverButton.SetActive(false);
        nextButton.SetActive(false);
        state = State.GameStart;
    }

    private void Move()
    {
        // bgmを管理
        if (gameManager.isEndlessMode == true)
        {            
            if (gameManager.endlessModeScore >= 1000)
            {
                soundManager.PlayFieldBGM("EndlessMode2");
            }
            else
            {
                soundManager.PlayFieldBGM("EndlessMode");
            }   
        }   
        else
        {
            soundManager.PlayFieldBGM("NormalStage");
        }


        //ラベルを更新
        stateText.text = "";

        player.playerMove = true;
        state = State.Move;
    }

    public void Battle(string battlePointName)
    {
        

        // bgmを管理
        switch (battlePointName)
        {
            case "BattlePoint":
                soundManager.PlayBattleBGM("Battle");
                break;
            case "BossBattlePoint":
                soundManager.PlayBattleBGM("BossBattle");
                break;
        }

        // エネミーが死んだ設定を戻す
        gameManager.isEnemyDead = false;

        gameManager.isBattle = true;
        battleScreen.SetActive(true);
        player.playerMove = false;


        state = State.Battle;
    }
    public void BattleClear()
    {
        // ラベルを更新
        stateText.text = "Battle Clear!";
        // BGMの設定
        soundManager.StopBattleBGM();
        soundManager.PlaySE(4);

        battleScreen.SetActive(false);
        gameManager.isBattle = false;
        gameManager.isBattleClear = false;

        // 問題を設定(KingHero用)
        typingManager.OutPut();

        Invoke("Move", 1.0f);

    }
    public void GameOver()
    {
        // ラベルを更新
        stateText.text = "Game Over";

        if (gameManager.isEndlessMode)
        {
            // ボタンを表示
            nextButton.SetActive(true);
        }
        else
        {   // ボタンを表示
            gameOverButton.SetActive(true);
        }

        Result();

        // BGMの設定
        soundManager.StopBattleBGM();
        soundManager.StopFieldBGM();
        soundManager.PlaySE(6);

        battleScreen.SetActive(false);
        gameManager.isBattle = false;
        state = State.GameOver;
    }

    public void GameClear()
    {
        // ボタンを表示
        nextButton.SetActive(true);

        // ラベルを更新
        stateText.text = "Game Clear!!";

        // BGMの設定
        soundManager.StopFieldBGM();
        soundManager.PlaySE(5);

        Result();
        player.playerMove = false;
    }


    private void Result()
    {
        // 結果の値を取得（正答数、誤字数、正答率、プレイヤーの残りHP）
        gameManager.correctN = typingManager.correctN;
        gameManager.mistakeN = typingManager.mistakeN;
        gameManager.correctAR = typingManager.correctAR;
        gameManager.playerRemainingHp = player.playerHp;
    }

    // AdventureModeの分岐点
    public void Branch()
    {
        player.playerMove = false;
        // 分岐点に到達した時の処理
        branchtext.SetActive(true);
    }
}
