using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // シングルトン
    //　ゲーム内に一つしか存在しないもの（音を管理するものとか）
    //　利用場所：シーン間でのデータ共有/オブジェクト共有
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // シングルトンの終わり

    public AudioSource[] audioSource; 　　　　// BattleBGM[0], FieldBGM[1], SE[2]のスピーカー
    public AudioClip[] audioClipsBattleBGM; //BattleBGMの素材(必ず先頭からBGM再生)
    public AudioClip[] audioClipsFieldBGM; //FieldBGMの素材(途中からBGM再生可)

    public AudioClip[] audioClipsSE;  //SEの素材(0:)

    /// <summary>
    /// BattleBGMの再生処理
    /// </summary>
    /// <param name="name"> 通常戦闘曲:Battle, ボス戦闘曲:BossBattle</param>
    public void PlayBattleBGM(string name)
    {
        PauseFieldBGM();
        switch (name)
        {
            case "Battle":
                audioSource[0].clip = audioClipsBattleBGM[0];
                break;
            case "BossBattle":
                audioSource[0].clip = audioClipsBattleBGM[1];
                break;
            default:
                Debug.Log("BattleBGMに該当のBGMがありません");
                break;
        }
        audioSource[0].Play();
    }

    /*
     * フィールドのBGMは途中から再生したい
     * AudioSource_2が必要
     */
    /// <summary>
    /// FieldBGMの再生処理
    /// </summary>
    /// <param name="name">
    /// 初級-上級:NormalStage, EndlessMode:EndlessMode, EndlessMode(Score>=1000):EndlessMode2
    /// </param>
    public void PlayFieldBGM(string name)
    {
        StopBattleBGM();
        switch (name)
        {
            case "NormalStage":
                audioSource[1].clip = audioClipsFieldBGM[0];
                break;
            case "EndlessMode":
                audioSource[1].clip = audioClipsFieldBGM[1];
                break;
            case "EndlessMode2":
                audioSource[1].clip = audioClipsFieldBGM[2];
                break;
            default:
                Debug.Log("FieldBGMに該当のBGMがありません");
                break;
        }
        audioSource[1].Play();
    }

    // フィールドBGMを一時停止
    public void PauseFieldBGM()
    {
        audioSource[1].Pause();
    }

    // フィールドBGMを停止
    public void StopFieldBGM()
    {
        audioSource[1].Stop();
    }

    // BattleBGMを停止
    public void StopBattleBGM()
    {
        audioSource[0].Stop();
    }

    // n番目のSEを一回ならす
    /// <summary>
    /// SEを鳴らす処理   * 引数情報はSoundManagerのInspecter参照
    /// </summary>
    public void PlaySE(int index)
    {
        if (index >= audioClipsSE.Length)
        {
            Debug.Log("該当SEがありません");
            return; 
        }
        audioSource[2].PlayOneShot(audioClipsSE[index]);
    }

    public void StopSE()
    {
        audioSource[2].Stop();
    }

}