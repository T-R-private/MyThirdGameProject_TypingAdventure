
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //  ラジオボタンの状態
    public bool isEnglish;
    //　エンドレスモードの設定
    public bool isEndlessMode;
    //  アドベンチャーモードの設定
    public bool isAdventure;

    //  バトルが始まったかどうか
    public bool isBattle;
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


}
