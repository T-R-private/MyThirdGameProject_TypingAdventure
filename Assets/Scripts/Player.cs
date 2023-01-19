using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/* Knightに関するStatusや戦闘システム、イベント処理などを記述するクラス */
public class Player : MonoBehaviour
{
    // 自身のコンポーネントの変数
    private Animator animator;
    private string[] attackAnim = { "Attack1", "Attack2", "Attack3" };
    private PlayerMoveController playerMoveController;

    // プレイヤーのステータス
    [SerializeField] float playerAttackDamage;
    public float playerHp;
    [SerializeField] private float maxHp;
    public bool isDead;

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
    // エネミーを取得
    private GameObject enemy;
    // 倒されたときに画面を揺らすためのカメラ
    [SerializeField] private Transform transformCamera;

    //  エフェクトの設定
    /*public GameObject recoverEffect;
    public GameObject perfectRecoverEffect;
    public GameObject powerUpEffect;*/

    // Start is called before the first frame update
    void Start()
    {
        // 初期値代入
        playerHp  = maxHp;
        isPowerUp = false;
        isDead    = false;

        // 自身のコンポーネントを取得
        animator = GetComponent<Animator>();
        animator.SetBool("Grounded", true);
        playerMoveController = GetComponent<PlayerMoveController>();
    }

    private void Update()
    {
        PowerUpShowText(isPowerUp);
        hpBar.fillAmount = playerHp * 0.01f;
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
        else
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
                BattleStartMove(collision.gameObject);
                break;
            case "BossBattlePoint":
                BattleStartMove(collision.gameObject);
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

    // バトルする際の初めの処理
    private void BattleStartMove(GameObject collision)
    {
        gameController.Battle(collision.gameObject.tag);
        enemy = collision.gameObject;
        playerMoveController.StopPlayer();
    }

    public void Attack()
    {
        // 死んだ後に攻撃させないため
        if (isDead) return;

        // 攻撃モーションをランダムに変化
        int rnd = Random.Range(0, 2);
        animator.SetTrigger(attackAnim[rnd]);

        //audioSource.PlayOneShot(attackSound);
        SoundManager.instance.PlaySE(0);
        
        if (isPowerUp)
        {
            // PowerUpを取った状態の攻撃力
            float powerUpAttackDamage = playerAttackDamage * powerUpValue;
            enemy.SendMessage("EnemyDamage", powerUpAttackDamage);
        }
        else
        {
            enemy.SendMessage("EnemyDamage", playerAttackDamage);
        }

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
        Time.timeScale = 1f;
        animator.speed = 1;

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