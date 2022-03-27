using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    //  LevelBarを取得
    public Image levelBar;
    //  LevelTextを取得
    public Text levelText;

    private void Update()
    {
        levelText.text = "Lv. " + PlayerStatusController.instance.level.ToString();
        levelBar.fillAmount = PlayerStatusController.instance.nowEp / PlayerStatusController.instance.levelUpEp;
    }




}
