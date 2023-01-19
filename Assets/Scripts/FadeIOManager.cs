using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeIOManager : MonoBehaviour
{

    //　シングルトン化
    public static FadeIOManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CanvasGroup canvasGroup;
    //時間設定
    public float fadeTime = 2f;

    /// <summary>
    /// 画面暗転のアニメーション処理
    /// </summary>
    public void FadeOut()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1, fadeTime)
            .OnComplete(() => canvasGroup.blocksRaycasts = false);
    }

    /// <summary>
    /// 画面明転のアニメーション処理
    /// </summary>
    public void FadeIn()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(0, fadeTime)
             .OnComplete(() => canvasGroup.blocksRaycasts = false);
    }

    /// <summary>
    /// 画面暗転から明転のアニメーション処理
    /// </summary>
    public void FadeOutToIn(TweenCallback action)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1, fadeTime)
            .OnComplete(() =>
            {
                action();
                FadeIn();
            });           
    }
}
