using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/* Wizzardに関するStatusや戦闘システム、イベント処理などを記述するクラス */
public class WizzardPlayer : MonoBehaviour
{
    // 自身のコンポーネントの変数
    private Animator animator;
    private PlayerMoveController playerMoveController;

    // プレイヤーのステータス
    [SerializeField] float   playerAttackDamage;
    [SerializeField] float   magicPower;
    [SerializeField] float[] wizzardLevel_AttackDamages;
    [SerializeField] float[] wizzardLevel_HealPoints;

    public float playerHp;
    public float maxHp;
    public bool  isDead;

    // モードの種類
    enum WizzardActionMode
    {
        ATTACK,
        HEAL
    }
    WizzardActionMode wizzardActionMode;

    // プレイヤーがパワーアイテムを取った時のフラグ
    private bool isPowerUp;
    // パワーアップの倍率
    [SerializeField] private float powerUpValue;
    // パワーアップの秒数
    private float powerUpTime;
    // パワーアップの設定時間
    [SerializeField] private float powerUpCountTime;
    public Text powerUpTimeText;


    // ゲームコントローラーの取得
    public GameController gameController;
    //  HPバーの取得
    public Image hpBar;
    //  MagicPowerバーの取得
    [SerializeField] Image magicPowerBar;
    // MagicPowerTextの取得
    [SerializeField] Text magicPowerLevelText;
    // 各戦闘ModeのアイコンのGameObject
    [SerializeField] GameObject attackModeObj;
    [SerializeField] GameObject healModeObj;

    // エネミーを取得
    private GameObject enemy;
    // 倒されたときに画面を揺らすためのカメラ
    [SerializeField] private Transform transformCamera;

    void Start()
    {
        // 初期化設定
        playerHp   = maxHp;
        isPowerUp  = false;
        isDead     = false;
        magicPower = 0;
        wizzardActionMode = WizzardActionMode.ATTACK;
        attackModeObj.SetActive(true);

        animator = GetComponent<Animator>();
        playerMoveController = GetComponent<PlayerMoveController>();
        animator.SetBool("Grounded", true);
    }

    private void Update()
    {
        PowerUpShowText(isPowerUp);
        hpBar.fillAmount = playerHp / maxHp;
        ChangeActionMode();
        WizzardAction();
        if (GameManager.instance.isBattle)
        {
            magicPowerBar.fillAmount = magicPower / 50.0f;
        }
        else
        {
            magicPower = 0;
        }
        magicPowerLevelText.text = "Lv. " + CheckMagicPowerLevel(magicPower).ToString();
    }

    private void ChangeActionMode()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDead)
        {
            // Modeの切り替え
            if (wizzardActionMode == WizzardActionMode.ATTACK)
            {
                wizzardActionMode = WizzardActionMode.HEAL;
                // ModeObjの切り替え
                healModeObj.SetActive(true);
                attackModeObj.SetActive(false);
            }
            else if (wizzardActionMode == WizzardActionMode.HEAL)
            {
                wizzardActionMode = WizzardActionMode.ATTACK;
                // ModeObjの切り替え
                attackModeObj.SetActive(true);
                healModeObj.SetActive(false);
            }
        }
    }

    private void WizzardAction()
    {
        if (Input.GetKeyDown(KeyCode.Return) && GameManager.instance.isBattle && !isDead)
        {
            if (wizzardActionMode == WizzardActionMode.ATTACK)
            {
                Attack(CheckMagicPowerLevel(magicPower));
            }
            else if (wizzardActionMode == WizzardActionMode.HEAL)
            {
                Heal(CheckMagicPowerLevel(magicPower));
            }
        }
    }

    // パワーアップアイテムを取った時のテキスト処理
    private void PowerUpShowText(bool powerUp)
    {
        if (powerUp)
        {
            powerUpTimeText.text = powerUpTime.ToString();
            powerUpTime -= Time.deltaTime;
            if (powerUpTime <= 0)
            {
                this.isPowerUp = false;
            }
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

    private void BattleMove(GameObject collision)
    {
        gameController.Battle(collision.gameObject.tag);
        enemy = collision.gameObject;
        playerMoveController.StopPlayer();
    }

    /// <summary>
    /// Wizzardの魔法力を増加するメソッド
    /// </summary>
    public void IncrementMagicPower()
    {
        if (isPowerUp)
        {
            magicPower += powerUpValue;
        }
        else
        {
            magicPower++;
        }
    }

    /// <summary>
    /// Wizzardの魔法力を減少するメソッド
    /// </summary>
    public void DecrementMagicPower()
    {
        magicPower -= 2;
        if (magicPower < 0)
        {
            magicPower = 0;
        }
    }

    private int CheckMagicPowerLevel(float magicPower)
    {
        // 死んだ後にレベルを上げないため
        if (isDead) return 0;

        // 以下のmagicPowerの条件でレベルを返す
        if      (magicPower <= 8)                      return 0;
        else if (magicPower >= 9  && magicPower <= 18) return 1;
        else if (magicPower >= 19 && magicPower <= 30) return 2;
        else if (magicPower >= 31 && magicPower <= 49) return 3;
        else if (magicPower >= 50)                     return 4;
        // それ以外（エラー）
        else return 0;
    }

    public void Attack(int magicPowerLevel)
    {
        // 死んだ後に攻撃させないため
        if (isDead) return;
        // Lv.0なら攻撃できない
        if (magicPowerLevel == 0) return;

        playerAttackDamage = wizzardLevel_AttackDamages[magicPowerLevel];

        this.magicPower = 0;
        SoundManager.instance.PlaySE(13);
        animator.SetTrigger("Attack");
        enemy.SendMessage("EnemyDamage", playerAttackDamage);
    }

    public void Heal(int magicPowerLevel)
    {
        // 死んだ後に回復させないため
        if (isDead) return;
        // Lv.0なら回復できない
        if (magicPowerLevel == 0) return;

        Recovery(wizzardLevel_HealPoints[magicPowerLevel]);

        this.magicPower = 0;
        SoundManager.instance.PlaySE(1);
    }

    public void PlayerDamage(int enemyAttackPoint)
    {
        // 死んだ後に攻撃を受けないようにするため
        if (isDead) return;
        playerHp -= enemyAttackPoint;
        if (playerHp <= 0)
        {
            isDead = true;
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