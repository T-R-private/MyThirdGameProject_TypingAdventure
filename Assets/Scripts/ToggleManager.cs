using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 英語かローマ字入力を切り替えるチェックボックスを操作するスクリプト */
public class ToggleManager : MonoBehaviour
{ 
    public void onPointerClick()
    {
        if (this.gameObject.name == "English")
        {
            GameManager.instance.isEnglish = true;          
        }
        else
        {
            GameManager.instance.isEnglish = false;           
        }       
    }

}
