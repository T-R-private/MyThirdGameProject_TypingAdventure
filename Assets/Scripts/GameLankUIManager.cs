using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* GameLankのUIを操作するスクリプト */
public class GameLankUIManager : MonoBehaviour
{
    [SerializeField] Text rankVauleText;
    [SerializeField] Image expCircleGage;

    [SerializeField] GameObject RankUpPanel;
    [SerializeField] Text prevRankText;
    [SerializeField] Text nextRankText;
    [SerializeField] GameObject finalRankText;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
        expCircleGage = expCircleGage.GetComponent<Image>();
    }

    private void Update()
    {
        rankVauleText.text = gameManager.gameRank.ToString();
        expShowUI();
        if (gameManager.isLevelUp)
        {
            RankUpEvent();
            gameManager.isLevelUp = false;
        }
    }

    // 円ゲージUIの操作
    public void expShowUI()
    {
        float gagePoint;
        gagePoint = (float)gameManager.currentGameRankExp / (gameManager.gameRank * 100);

        expCircleGage.fillAmount = gagePoint;
    }

    public void RankUpEvent()
    {       
        RankUpPanel.SetActive(true);
        // 99になったらThank you for Playingのテキストを出す
        if(gameManager.gameRank >= 99)
        {
            finalRankText.SetActive(true);
        }
        prevRankText.text = gameManager.prevRank.ToString();
        nextRankText.text = gameManager.gameRank.ToString();
        SoundManager.instance.PlaySE(9);
    }

    public void OnCloseButtonClick()
    {
        RankUpPanel.SetActive(false);
    }

}
