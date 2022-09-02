using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingButton : MonoBehaviour
{
    //  Pause画面を取得
    public GameObject PauseScreen;

    public void OnPauseClicked()
    {
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
    }

    public void OnPauceBackClicked()
    {
        SoundManager.instance.PlaySE(8);
        Time.timeScale = 1;
        PauseScreen.SetActive(false);
    }

    public void OnPauceStopClicked()
    {
        SoundManager.instance.PlaySE(8);
        // モードをリセット
        GameManager.instance.isEndlessMode = false;
        GameManager.instance.isAdventure = false;

        Time.timeScale = 1;

        // BGMを止める
        SoundManager.instance.StopFieldBGM();
        SoundManager.instance.StopBattleBGM();

        SceneManager.LoadScene("StageSelectScene");
    }
}
