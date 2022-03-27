using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    public static PlayerStatusController instance = null;

    //アドベンチャーモードのプレイヤーのステータス
    public int   level = 1;
    public float playerHp;
    public float playerAttackDamage;

    //今のEPとレベルアップに必要なEP
    public float nowEp;
    public float levelUpEp = 10;

    // レベルアップしたかどうか
    public bool isLevelUp;

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

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.isAdventure)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }


    }

    //  EP取得処理
    public void GetEp(float getEp)
    {
        nowEp += getEp;
        if (nowEp >= levelUpEp)
        {
            LevelUp();
            levelUpEp += level * 20.0f;
            nowEp = 0;
        }
    }

    //  レベルアップ処理
    private void LevelUp()
    {
        level++;
        playerHp += 5;
        playerAttackDamage += 2;

        isLevelUp = true;
    }



}
