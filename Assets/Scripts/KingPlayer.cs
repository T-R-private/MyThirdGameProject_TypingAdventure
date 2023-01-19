using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/* Kingに関するStatusや戦闘システム、イベント処理などを記述するクラス */
public class KingPlayer : MonoBehaviour
{
    // 自身のコンポーネントの変数
    private Animator animator;
    private string[] attackAnim = { "Attack1", "Attack2", "Attack3" };
    private AudioSource audioSource;
    private PlayerMoveController playerMoveController;

    // プレイヤーのステータス
    public float playerHp;
    public float maxHp;
    public bool  isDead;
    /*
     * Chargeした分のAttackDamage追加
     * Chargeする制限時間
     */
    public float chargeAttackDamage;
    [SerializeField] Image chargeAttackDamageBar;
    [SerializeField] Image chargeTimeBar;
    [SerializeField] float chargeTime;
    [SerializeField] float chargeCountTime;

    // プレイヤーがパワーアイテムを取った時のフラグ
    private bool isPowerUp;
    // パワーアップの秒数
    private float powerUpTime;
    // パワーアップの倍率
    [SerializeField] private float powerUpValue;
    // パワーアップの設定時間
    [SerializeField] private float powerUpCountTime;
    public Text powerUpTimeText;


    // ゲームコントローラーの取得
    public GameController gameController;
    //  HPバーの取得
    public Image hpBar;
    // エネミーを取得
    private GameObject enemy;
    // 倒されたときに画面を揺らすためのカメラ
    [SerializeField] private Transform transformCamera;

    void Start()
    {
        // 初期化処理
        playerHp  = maxHp;
        isPowerUp = false;
        isDead    = false;
        chargeAttackDamageBar.fillAmount = 0;

        // 自身のコンポーネントを取得
        animator    = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerMoveController = GetComponent<PlayerMoveController>();
        animator.SetBool("Grounded", true);

    }

    private void Update()
    {
        if (GameManager.instance.isBattle)
        {
            if (isDead) return;
            chargeCountTime += Time.deltaTime;
            chargeTimeBar.fillAmount = chargeCountTime / chargeTime;
            if (chargeCountTime >= chargeTime)
            {
                chargeCountTime = 0.0f;
                ChargeAttack();
            }
        }
        else
        {
            chargeCountTime = 0.0f;
            chargeTimeBar.fillAmount = chargeCountTime / chargeTime;
            audioSource.Stop();
        }

        PowerUpShowText(isPowerUp);
        hpBar.fillAmount = playerHp / maxHp;
    }

    // パワーアップアイテムを取った時のテキスト処理
    private void PowerUpShowText(bool powerUp)
    {
        if (powerUp)
        {
            powerUpTimeText.text = powerUpTime.ToString();
            powerUpTime -= Time.deltaTime;
            if (powerUpTime <= 0) this.isPowerUp = false;
        }
        else if (GameManager.instance.isEndlessMode && !powerUp)
        {
            powerUpTimeText.text = "";
            powerUpTime = 0.0f;
        }
    }

    // 別クラスへの移行は現在未定
    // 現状、GameControllerと結合度が強いため、別クラスへの移行すると複雑化してしまう
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "BattlePoint":
                BattleMove(collision.gameObject);
                break;
            case "BossBattlePoint":
                BattleMove(collision.gameObject);
                break;
            case "GoalPoint":
                gameController.GameClear();
                playerMoveController.StopPlayer();
                break;
            case "RecoveryItems":
                Recovery(10.0f);
                collision.gameObject.SetActive(false);
                SoundManager.instance.PlaySE(1);
                break;
            case "PerfectRecovery":
                Recovery(100.0f);
                SoundManager.instance.PlaySE(2);
                break;
            case "PowerUpItems":
                PowerUp();
                collision.gameObject.SetActive(false);
                SoundManager.instance.PlaySE(3);
                break;
        }
        
    }

    // バトルした時のPlayerの動き
    private void BattleMove(GameObject collision)
    {
        gameController.Battle(collision.gameObject.tag);
        enemy = collision.gameObject;
        playerMoveController.StopPlayer();
        audioSource.Play();
    }

    /*変更点　ChargeAttack仕様に変更*/
    public void ChargeAttack()
    {
        // 死んだ後に攻撃させないため
        if (isDead || !GameManager.instance.isBattle) return;
        audioSource.Play();

        // 攻撃モーションをランダムに変化
        int rnd = Random.Range(0, 2);
        animator.SetTrigger(attackAnim[rnd]);

        SoundManager.instance.PlaySE(10);
        enemy.SendMessage("EnemyDamage", chargeAttackDamage);

        // ChargePowerをリセットする
        ResetChargeAttackDamage();
    }

    /* chargeAttackDamageの増減メソッド*/
    /// <summary>
    /// KingのCharge攻撃を増加するメソッド
    /// </summary>
    public void IncrementChargeAttackDamage()
    {
        if (isPowerUp)
        {
            chargeAttackDamage += 2 * powerUpValue;
        }
        else
        {
            chargeAttackDamage += 2;
        }
        chargeAttackDamageBar.fillAmount = chargeAttackDamage / 100.0f;
    }

    /// <summary>
    /// KingのCharge攻撃を減少するメソッド
    /// </summary>
    public void DecrementChargeAttackDamage()
    {
        chargeAttackDamage -= 4;
        if (chargeAttackDamage < 0)
        {
            chargeAttackDamage = 0;
        }
        chargeAttackDamageBar.fillAmount = chargeAttackDamage / 100.0f;
    }

    /// <summary>
    /// KingのCharge攻撃を初期化するメソッド
    /// </summary>
    public void ResetChargeAttackDamage()
    {
        chargeAttackDamage = 0;
        chargeAttackDamageBar.fillAmount = chargeAttackDamage / 100.0f;
    }

    public void PlayerDamage(int enemyAttackPoint)
    {
        // 死んだ後に攻撃を受けないようにするため
        if (isDead) return;
        playerHp -= enemyAttackPoint;       
        if (playerHp <= 0)
        {
            isDead = true;
            // これを追加しないと少しHP残った表示のまま死亡する
            hpBar.fillAmount = 0;
            StartCoroutine(Dead());
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private IEnumerator Dead()
    { 
        // ヒットストップ(死亡演出)
        SoundManager.instance.PlaySE(10);
        transformCamera.DOShakePosition(0.3f, 0.7f, 50);
        audioSource.Stop();
        animator.speed = 0.4f;
        Time.timeScale = 0.5f;
        animator.SetTrigger("Death");

        // ヒットストップ演出待ちの時間
        yield return new WaitForSeconds(0.8f);
        // 元に戻す
        animator.speed = 1;
        Time.timeScale = 1f;

        yield return new WaitForSeconds(0.5f);
        gameController.GameOver();
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }

    // 回復アイテムを取った時の効果
    private void Recovery(float recoveryHp)
    {
        playerHp += recoveryHp;
        if (playerHp >= maxHp)
        {
            playerHp = maxHp;
        }      
    }

    // パワーアップアイテムを取った時の効果
    private void PowerUp()
    {      
        powerUpTime = powerUpCountTime;
        isPowerUp = true;
    }
}
