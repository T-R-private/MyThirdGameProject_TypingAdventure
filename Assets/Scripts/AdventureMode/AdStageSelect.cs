using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdStageSelect : MonoBehaviour
{
    public void OnCllickFirstBranch(int number)
    {
        switch (number)
        {
            case 0:
                //ステージ0へ移行する処理
                SceneManager.LoadScene("AdventureMode");
                break;
            case 1:
                //ステージ1へ移行する処理
                SceneManager.LoadScene("Stage1");
                break;
            case 2:
                //ステージ2へ移行する処理
                SceneManager.LoadScene("Stage2");
                break;
            case 3:
                //ステージ3へ移行する処理
                SceneManager.LoadScene("Stage3");
                break;
            case 4:
                //ステージ5へ移行する処理
                SceneManager.LoadScene("Stage3");
                break;
            case 5:
                //ステージFinalへ移行する処理
                SceneManager.LoadScene("LastStage");
                break;

        }
    }
}
