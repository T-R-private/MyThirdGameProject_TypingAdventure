using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingButton : MonoBehaviour
{
    //  Pause画面を取得
    public GameObject PauseScreen;
    //　ゲームコントローラーの取得
    public GameController gameController;

    public void OnPauseClicked()
    {
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
    }

    public void OnPauceBackClicked()
    {
        Time.timeScale = 1;
        PauseScreen.SetActive(false);
    }

    public void OnPauceStopClicked()
    {
        // モードをリセット
        GameManager.instance.isEndlessMode = false;
        GameManager.instance.isAdventure = false;

        Time.timeScale = 1;

        // BGMを止める
        gameController.audios[0].Stop();
        gameController.audios[1].Stop();

        SceneManager.LoadScene("StageSelectScene");
    }
}
