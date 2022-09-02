using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{
    // PlayerModelを取得
    PlayerModel playerModel;
    // 自身のコンポーネントの変数
    private Rigidbody2D rb;
    private Animator animator;
    private string[] attackAnim = { "Attack1", "Attack2", "Attack3" };

    // プレイヤーが動いているか
    public bool playerMove = false;

    // プレイヤーのステータス
    [SerializeField] private float playerSpeed;
    [SerializeField] float playerAttackDamage;
    public float playerHp;
    [SerializeField] private float maxHp;
    public bool isDead;

    // プレイヤーがパワーアイテムを取った時のイベントの変数
    private bool isPowerUp;
    private float powerUpTime;
    private int powerUpCount;
    [SerializeField]private float powerUpCountTime;
    public Text powerUpTimeText;
     

    // ゲームコントローラーの取得
    public GameController gameController;
    //  HPバーの取得
    public Image hpBar;
    // エネミーを取得
    private GameObject enemy;
    // 倒されたときに画面を揺らすためのカメラ
    [SerializeField] private Transform playerDeadPanel;

    //  エフェクトの設定
    /*public GameObject recoverEffect;
    public GameObject perfectRecoverEffect;
    public GameObject powerUpEffect;*/

    // Start is called before the first frame update
    void Start()
    {
        playerHp = maxHp;
        isPowerUp = false;

        // 自身のコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerModel = new PlayerModel();
        
        animator.SetBool("Grounded", true);
        isDead = false;
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
                playerAttackDamage = playerAttackDamage / 2.0f;
                this.isPowerUp = false;
            }

        }
        else
        {
            powerUpTimeText.text = "";
            powerUpTime = 0.0f;
            powerUpCount = 0;
        }
    }

    private void FixedUpdate()
    {
        if (playerMove && GameManager.instance.isAdventure)
        {
            float horizontalKey = Input.GetAxis("Horizontal");
            float xSpeed = 0.0f;

            if (horizontalKey > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                animator.SetInteger("AnimState", 1);
                xSpeed = playerSpeed;
            }
            else if (horizontalKey < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                animator.SetInteger("AnimState", 1);
                xSpeed = -playerSpeed;
            }
            else
            {
                animator.SetInteger("AnimState", 0);
                xSpeed = 0.0f;
            }
            rb.velocity = new Vector2(xSpeed, 0.0f);

        }
        else if (playerMove)
        {
            animator.SetInteger("AnimState", 1);
            rb.velocity = new Vector2(playerSpeed, 0.0f);
        }        
    }

    // プレイヤーを停止させる
    private void StopPlayer()
    {
        animator.SetInteger("AnimState", 0);
        rb.velocity = new Vector2(0.0f, 0.0f);
    }

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
                StopPlayer();
                break;
            case "BranchPoint":
                gameController.Branch();
                enemy = collision.gameObject;
                StopPlayer();
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
                PowerUp(2.0f);
                collision.gameObject.SetActive(false);
                SoundManager.instance.PlaySE(3);
                break;
        }
        
    }

    private void BattleMove(GameObject collision)
    {
        gameController.Battle(collision.gameObject.tag);
        enemy = collision.gameObject;
        StopPlayer();
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
        enemy.SendMessage("EnemyDamage", playerAttackDamage);
    }

    public void PlayerDamage(int enemyAttackPoint)
    {
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
        // ヒットストップ
        SoundManager.instance.PlaySE(10);
        animator.speed = 0.4f;
        animator.SetTrigger("Death");
        playerDeadPanel.DOShakePosition(0.50f, 0.3f, 10, 10);

        // ヒットストップ演出待ちの時間
        yield return new WaitForSeconds(0.8f);

        animator.speed = 1;     
        yield return new WaitForSeconds(0.5f);
        gameController.GameOver();
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }

    private void Recovery(float recoveryHp)
    {
        playerHp += recoveryHp;
        if (playerHp >= maxHp)
        {
            playerHp = maxHp;
        }      
    }

    // パワーアップアイテムを取った時の効果
    private void PowerUp(float power)
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
}
