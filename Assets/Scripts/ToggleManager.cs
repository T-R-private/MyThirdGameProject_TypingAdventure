using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
