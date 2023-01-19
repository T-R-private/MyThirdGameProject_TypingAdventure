using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{

    // プレイヤーが動いているか
    public bool playerMove;

    // プレイヤーのステータス
    public float playerSpeed;
    public float playerAttackDamage;
    public float playerHp;
    public float maxHp;
    public bool isDead;

    // プレイヤーがパワーアイテムを取った時のイベントの変数
    public bool isPowerUp;
    public float powerUpTime;
    public int powerUpCount;
    public float powerUpCountTime;

    public PlayerModel()
    {
        playerHp = maxHp;
        isPowerUp = false;
        isDead = false;
        playerMove = false;
    }

    // パワーアップアイテムを取った時の効果
    public void PowerUp(float power)
    {
        if (powerUpCount == 0)
        {
            powerUpTime = powerUpCountTime;
            isPowerUp = true;
            playerAttackDamage = playerAttackDamage * power;
            powerUpCount++;
        }
        else if (powerUpCount >= 1)
        {
            // 効果が続いている状態でPowerUpアイテムを取ったらPowerUpTimeを0にする
            powerUpTime = powerUpCountTime;
        }
    }

    public void Recovery(float recoveryHp)
    {
        playerHp += recoveryHp;
        if (playerHp >= maxHp)
        {
            playerHp = maxHp;
        }
    }
}
