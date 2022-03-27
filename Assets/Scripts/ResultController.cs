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

    private float rankData;
    private char rankAlphabet;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.isEndlessMode)
        {
            StartCoroutine("EndlessScore");
        }
        else
        {
            StartCoroutine("Rank");
        }
        
        backButton.SetActive(false);
        rankingButton.SetActive(false);
    }

    IEnumerator Rank()
    {
        // ランクを計算
        rankData = GameManager.instance.correctAR + GameManager.instance.playerRemainingHp;
        if (rankData >= 150)
        {
            rankAlphabet = 'S';
        }
        else if (rankData >= 120)
        {
            rankAlphabet = 'A';
        }
        else if (rankData >= 90)
        {
            rankAlphabet = 'B';
        }
        else
        {
            rankAlphabet = 'C';
        }

        yield return new WaitForSeconds(0.5f);

        // ランクを表示
        rankText.text = "Rank" + rankAlphabet;

        yield return new WaitForSeconds(0.5f);

        // 結果とボタンの表示
        backButton.SetActive(true);
        Result();
    }

    IEnumerator EndlessScore()
    {
        yield return new WaitForSeconds(0.5f);

        // スコアの表示
        rankText.text = "Score : " + GameManager.instance.endlessModeScore;

        yield return new WaitForSeconds(0.5f);

        // 結果とボタンの表示
        backButton.SetActive(true);
        rankingButton.SetActive(true);
        Result();
    }

    void Result()
    {
        correctN_Text.text = GameManager.instance.correctN.ToString();
        mistakeN_Text.text = GameManager.instance.mistakeN.ToString();
        correctAR_Text.text = GameManager.instance.correctAR.ToString() + "%";
    }
}
