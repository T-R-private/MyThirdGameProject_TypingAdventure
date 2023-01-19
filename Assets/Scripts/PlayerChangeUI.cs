using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerChangeUI : MonoBehaviour
{
    private Dropdown playerChangeUI;
    [SerializeField] GameObject[] PlayerImage;

    private void Start()
    {
        playerChangeUI = this.GetComponent<Dropdown>();
        SetPlayer();
    }

    // ドロップダウンUIがStage画面に進むたび、PlayerTypeを初期化するのを防ぐために、
    // 最初にGamaManagerのplayerTypeで判別しvalueに値を入れる
    private void SetPlayer()
    {
        switch (GameManager.instance.playerType)
        {
            // value (Knight:0, King:1, Wizzard:2)
            case GameManager.PlayerType.KNIGHT:
                playerChangeUI.value = 0;
                AllSetActiveFalse();
                PlayerImage[0].SetActive(true);
                break;
            case GameManager.PlayerType.KINGPLAYER:
                playerChangeUI.value = 1;
                AllSetActiveFalse();
                PlayerImage[1].SetActive(true);
                break;
            case GameManager.PlayerType.WIZZARDPLAYER:
                playerChangeUI.value = 2;
                AllSetActiveFalse();
                PlayerImage[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    // Playerを選択したら他のImageは非アクティブにする
    public void OnPlayerSelected()
    {
        switch (playerChangeUI.options[playerChangeUI.value].text)
        {
            case "Knight":
                GameManager.instance.playerType = GameManager.PlayerType.KNIGHT;
                AllSetActiveFalse();
                PlayerImage[0].SetActive(true);
                break;
            case "King":
                GameManager.instance.playerType = GameManager.PlayerType.KINGPLAYER;
                AllSetActiveFalse();
                PlayerImage[1].SetActive(true);
                break;
            case "Wizzard":
                GameManager.instance.playerType = GameManager.PlayerType.WIZZARDPLAYER;
                AllSetActiveFalse();
                PlayerImage[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    private void AllSetActiveFalse()
    {
        for (int i = 0; i < PlayerImage.Length; i++)
        {
            PlayerImage[i].SetActive(false);
        }
    }
}
