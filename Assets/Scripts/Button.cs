using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    public void OnTitleButtonClicked()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    public void OnStageSelectButtonClicked(int number)
    {
        GameManager.instance.isAdventure = false;
        switch (number)
        {
            case 0:
                SceneManager.LoadScene("SampleScene");
               break;
            case 1:
                SceneManager.LoadScene("Sample2");
               break;
            case 2:
                SceneManager.LoadScene("Sample3");
                break;
                
        }
    }

    public void OnBackButtonClicked()
    {
        GameManager.instance.isAdventure = false;
        GameManager.instance.isEndlessMode = false;
        SceneManager.LoadScene("StageSelectScene");
    }

    public void OnNextButtonClicked()
    {
        GameManager.instance.isAdventure = false;
        SceneManager.LoadScene("ResultScene");
    }

    public void OnEndlessModeButtonClicked()
    {
        GameManager.instance.isEndlessMode = true;
        GameManager.instance.endlessModeScore = 0;
        SceneManager.LoadScene("EndlessMode");
    }

    public void OnAdventureModeButtonClicked()
    {
        GameManager.instance.isAdventure = true;
        //SceneManager.LoadScene();
    }

    public void OnRankingRegister()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(GameManager.instance.endlessModeScore, 0);
    }
}
