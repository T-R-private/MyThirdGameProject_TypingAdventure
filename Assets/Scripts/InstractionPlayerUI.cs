using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 各プレイヤーの説明画面を操作するスクリプト */
public class InstractionPlayerUI : MonoBehaviour
{
    [SerializeField] GameObject instractionPlayerPanel;

    public void OnInstractionButton()
    {
        instractionPlayerPanel.SetActive(true);
    }
    public void OnCloseButton()
    {
        instractionPlayerPanel.SetActive(false);
    }
}
