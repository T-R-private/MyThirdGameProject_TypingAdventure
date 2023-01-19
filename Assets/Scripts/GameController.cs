using System;
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
    private Player     player;
    private GameObject playerObj;
    // KingPlayerを取得
    private KingPlayer kingPlayer;
    private GameObject kingPlayerObj;
    // WizzardPlayerを取得
    private WizzardPlayer wizzardPlayer;
    private GameObject    wizzardPlayerObj;
    //　タイピングソフトを取得
    public TypingManager typingManager;
    // PlayerMoveControllerを取得
    private PlayerMoveController playerMoveController;

    //　UIを取得
    public GameObject battleScreen;
    public Text       stateText;
    public GameObject gameOverButton;
    public GameObject nextButton;
    public GameObject branchtext;
    //  エンドレスモードのスコアテキストを取得
    public Text endlessModeScoreText;
    //  PowerUpの時間を表示するテキスト
    public Text powerUpCountText;

    // GameManager, SoundManagerの省略のための変数
    GameManager  gameManager;
    SoundManager soundManager;

    private void Start()
    {
        // コード省略のための初期化
        gameManager  = GameManager.instance;
        soundManager = SoundManager.instance;
        // 各プレイヤーのObjを見つける
        playerObj        = GameObject.Find("Player");
        kingPlayerObj    = GameObject.Find("KingPlayer");
        wizzardPlayerObj = GameObject.Find("WizzardPlayer");

        SetPlayer(gameManager.playerType);
        GameStart();
    }

    // 決められたタイプのPlayerとPlayerMoveControllerを取得し、他の種類のプレイヤーは非表示にする
    void SetPlayer(GameManager.PlayerType playerType)
    {
        switch (playerType)
        {
            case GameManager.PlayerType.KNIGHT:
                player = playerObj.GetComponent<Player>();
                playerMoveController = playerObj.GetComponent<PlayerMoveController>();
                kingPlayerObj.SetActive(false);
                wizzardPlayerObj.SetActive(false);
                break;
            case GameManager.PlayerType.KINGPLAYER:
                kingPlayer = kingPlayerObj.GetComponent<KingPlayer>();
                playerMoveController = kingPlayerObj.GetComponent<PlayerMoveController>();
                playerObj.SetActive(false);
                wizzardPlayerObj.SetActive(false);
                break;
            case GameManager.PlayerType.WIZZARDPLAYER:
                wizzardPlayer = wizzardPlayerObj.GetComponent<WizzardPlayer>();
                playerMoveController = wizzardPlayerObj.GetComponent<PlayerMoveController>();
                kingPlayerObj.SetActive(false);
                playerObj.SetActive(false);
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
        // EndlessModeのスコアテキスト更新
        if (gameManager.isEndlessMode)
        {
            endlessModeScoreText.text = "Score " + gameManager.endlessModeScore.ToString();
        }
    }

    private void GameStart()
    {
        // ラベルを更新
        stateText.text = "Press Ctrl to Start";

        // 初期化処理
        gameManager.isBattle = false;
        playerMoveController.ChangePlayerMove(false);
        battleScreen.SetActive(false);
        gameOverButton.SetActive(false);
        nextButton.SetActive(false);
        state = State.GameStart;
    }

    private void Move()
    {
        // StageBGMの設定
        if (gameManager.isEndlessMode && gameManager.endlessModeScore >= 1000)
        {
            soundManager.PlayFieldBGM("EndlessMode2");
        }
        else if (gameManager.isEndlessMode)
        {
            soundManager.PlayFieldBGM("EndlessMode");
        }   
        else
        {
            soundManager.PlayFieldBGM("NormalStage");
        }

        //ラベルを更新
        stateText.text = "";

        playerMoveController.ChangePlayerMove(true);
        state = State.Move;
    }

    /// <summary>
    /// バトル開始時の処理(BGM, フラグ設定など)
    /// </summary>
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
        playerMoveController.ChangePlayerMove(false);
        state = State.Battle;
    }

    /// <summary>
    /// バトルクリア時の処理(BGM, フラグ設定など)
    /// </summary>
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

        // バトルクリア後に新しい問題を設定する
        // Playerによっては問題文の途中からになるのを防ぐため
        typingManager.OutPut();

        Invoke("Move", 1.0f);
    }

    /// <summary>
    /// ゲームオーバー時の処理(BGM, フラグ設定など)
    /// </summary>
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

        GetResultData();

        // BGMの設定
        soundManager.StopBattleBGM();
        soundManager.StopFieldBGM();
        soundManager.PlaySE(6);

        battleScreen.SetActive(false);
        gameManager.isBattle = false;
        state = State.GameOver;
    }

    /// <summary>
    /// ステージクリア時の処理(BGM, フラグ設定など)
    /// </summary>
    public void GameClear()
    {
        // ボタンを表示
        nextButton.SetActive(true);

        // ラベルを更新
        stateText.text = "Game Clear!!";

        // BGMの設定
        soundManager.StopFieldBGM();
        soundManager.PlaySE(5);

        GetResultData();
        playerMoveController.ChangePlayerMove(false);
    }
 
    // ResultSceneで結果データを扱うためにGameManagerに移行する必要あり
    private void GetResultData()
    {
        // TypingManagerから結果データを取得（正答数、誤字数、正答率、プレイヤーの残りHP）
        gameManager.correctN  = typingManager.correctN;
        gameManager.mistakeN  = typingManager.mistakeN;
        gameManager.correctAR = typingManager.correctAR;

        // 全キャラの残ったHPの割合をGamaManagerに代入
        // 百分率に合わせるためにKingとWizzardを調整している
        if (gameManager.playerType == GameManager.PlayerType.KNIGHT)
            gameManager.playerRemainingHp = player.playerHp;
        else if (gameManager.playerType == GameManager.PlayerType.KINGPLAYER)
            gameManager.playerRemainingHp = kingPlayer.playerHp / (kingPlayer.maxHp / 100);
        else
            gameManager.playerRemainingHp = wizzardPlayer.playerHp / (wizzardPlayer.maxHp / 100);
    }
}
