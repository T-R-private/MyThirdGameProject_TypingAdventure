using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TwitterManager : MonoBehaviour
{
    public void OnClickTwitterButton()
    {
        //urlの作成
        string esctext = UnityWebRequest.EscapeURL("私のEndlessModeのスコアは"+GameManager.instance.endlessModeScore+
            "!あなたはタイピングでどのくらい得点を出せる？\nhttps://unityroom.com/games/mythirdgame");
        string esctag = UnityWebRequest.EscapeURL("TypingAdventure");
        string url = "https://twitter.com/intent/tweet?text=" + esctext + "&hashtags=" + esctag;

        //Twitter投稿画面の起動
        Application.OpenURL(url);

        //ここに報酬の処理を記載
        
    }
}
