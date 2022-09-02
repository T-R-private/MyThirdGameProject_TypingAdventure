using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    // UIを取得
    public Text correctN_Text;
    public Text mistakeN_Text;
    public Text correctAR_Text;
    public Text rankText;
    public GameObject backButton;
    public GameObject rankingButton;
    public GameObject twitterButton;

    private float rankData;
    private char rankAlphabet;

    // GameManagerの省略のための変数
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        if (gameManager.isEndlessMode)
        {
            StartCoroutine("EndlessScore");
        }
        else
        {
            StartCoroutine("Rank");
        }
        
        backButton.SetActive(false);
        rankingButton.SetActive(false);
        twitterButton.SetActive(false);
    }

    IEnumerator Rank()
    {
        // ランクを計算
        rankData = gameManager.correctAR + gameManager.playerRemainingHp;

        rankAlphabet = CalcResult(rankData);

        yield return new WaitForSeconds(0.5f);

        // ランクを表示
        rankText.text = "Rank" + rankAlphabet;

        yield return new WaitForSeconds(0.5f);

        // 結果とボタンの表示
        backButton.SetActive(true);
        Result();
    }

    private char CalcResult(float rankData)
    {
        if (rankData >= 150)
        {
            return 'S';
        }
        else if (rankData >= 120)
        {
            return 'A';
        }
        else if (rankData >= 90)
        {
            return 'B';
        }
        else
        {
            return 'C';
        }
    }

    IEnumerator EndlessScore()
    {
        yield return new WaitForSeconds(0.5f);

        // スコアの表示
        rankText.text = "Score : " + gameManager.endlessModeScore;

        yield return new WaitForSeconds(0.5f);

        // 結果とボタンの表示
        backButton.SetActive(true);
        rankingButton.SetActive(true);
        twitterButton.SetActive(true);
        Result();
    }

    void Result()
    {
        correctN_Text.text = gameManager.correctN.ToString();
        mistakeN_Text.text = gameManager.mistakeN.ToString();
        correctAR_Text.text = gameManager.correctAR.ToString() + "%";
    }
}
