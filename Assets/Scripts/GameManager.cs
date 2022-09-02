
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ゲーム全体の管理
public class GameManager : MonoBehaviour
{
    //  ラジオボタンの状態
    public bool isEnglish;

    // Playerの判別
    public enum PlayerType
    {
        PLAYER,
        KINGPLAYER,
    }
    public PlayerType playerType;

    // gameRank
    public int gameRank = 1;
    public int currentGameRankExp = 0;
    public int prevRank;
    public bool isLevelUp = false; 

    //　エンドレスモードの設定
    public bool isEndlessMode;
    //  アドベンチャーモードの設定
    public bool isAdventure;

    //  バトルが始まったかどうか
    public bool isBattle;
    //  敵を倒したかどうか
    public bool isEnemyDead;
    //　バトルをクリアしたか
    public bool isBattleClear;

    //  エンドレスモードのスコア
    public int endlessModeScore = 0;

    //  結果の保持
    public int correctN;
    public int mistakeN;
    public float correctAR;
    public float playerRemainingHp;

    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {      
        Load();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " scene loaded");
        if (scene.name == "StageSelectScene")
        {
            SoundManager.instance.StopSE();
            StartCoroutine(gameRankUpdate());
        }
    }

    IEnumerator gameRankUpdate()
    {
        yield return new WaitForSeconds(1.0f);
        // SceneNameがStageSelectSceneだったら、チェック＆セーブ       
         CheckGameRankUp();
         Save();
    }

    private void Load()
    {
        // スコアのロード
        gameRank = PlayerPrefs.GetInt("SCORE0", 1);
        currentGameRankExp = PlayerPrefs.GetInt("SCORE1", 0);
    }

    private void Save()
    {
        // スコアを保存
        PlayerPrefs.SetInt("SCORE0", gameRank);
        PlayerPrefs.SetInt("SCORE1", currentGameRankExp);
        PlayerPrefs.Save();
    }


    public void gameRankExpAdd(int gameRankPoint)
    {
        currentGameRankExp += gameRankPoint;
    }

    public void endlessModeScoreAdd(int point)
    {
        endlessModeScore += point;
    }

    private void CheckGameRankUp()
    {
        prevRank = gameRank;
        if(gameRank == 0)
        {
            return;
        }
        // 必要なExp
        int necessaryPoint = gameRank * 100;
        while (currentGameRankExp >= necessaryPoint)
        {
            GameRankUp();
            currentGameRankExp -= necessaryPoint;
            if (currentGameRankExp < 0)
            {
                currentGameRankExp = 0;
            }
            necessaryPoint = gameRank * 100;
        }
    }

    public void GameRankUp()
    {
        // 99以上になったらもう上がらない
        if (gameRank >= 99) return;
        gameRank++;
        // Sound/Effectなど追加
        isLevelUp = true;
    }
}
