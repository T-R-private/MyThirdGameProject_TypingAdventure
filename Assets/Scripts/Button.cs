using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    // 省略のための変数
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void OnSceneTransitionButtonClicked(string sceneName)
    {
        GameManager.instance.isAdventure = false;
        SoundManager.instance.PlaySE(8);
        switch (sceneName)
        {                
            case "SampleScene":
                FadeIOManager.instance.FadeOutToIn(() => Load("SampleScene"));
                break;
            case "Sample2":
                FadeIOManager.instance.FadeOutToIn(() => Load("Sample2"));
                break;
            case "Sample3":
                FadeIOManager.instance.FadeOutToIn(() => Load("Sample3"));
                break;
            case "StageSelectScene":
                gameManager.isEndlessMode = false;
                FadeIOManager.instance.FadeOutToIn(() => Load("StageSelectScene"));
                break;
            case "ResultScene":
                FadeIOManager.instance.FadeOutToIn(() => Load("ResultScene"));
                break;
            case "EndlessMode":
                gameManager.isEndlessMode = true;
                gameManager.endlessModeScore = 0;
                FadeIOManager.instance.FadeOutToIn(() => Load("EndlessMode"));
                break;
            
        }
    }

    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    // アドベンチャーモード
    public void OnAdventureModeButtonClicked()
    {
        gameManager.isAdventure = true;
        //SceneManager.LoadScene();
    }
    
    // ランキング登録
    public void OnRankingRegister()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(gameManager.endlessModeScore, 0);
    }
}
