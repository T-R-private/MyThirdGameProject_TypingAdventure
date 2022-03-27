using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 自身のコンポーネントの変数
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private string[] attackAnim = { "Attack1", "Attack2", "Attack3" };

    // プレイヤーが動いているか
    public bool playerMove = false;

    // プレイヤーのステータス
    [SerializeField] private float playerSpeed;
    [SerializeField] float playerAttackDamage;
    public float playerHp;
    [SerializeField] private float maxHp;

    // プレイヤーがパワーアイテムを取った時のイベントの変数
    private bool isPowerUp;
    private float powerUpTime;
    private int powerUpCount;
    [SerializeField]private float powerUpCountTime;
     

    // ゲームコントローラーの取得
    public GameController gameController;
    //  HPバーの取得
    public Image hpBar;
    //エネミーを取得
    private GameObject enemy;

    //  効果音の設定
    public AudioClip attackSound;
    public AudioClip recoverSound;
    public AudioClip perfectRecoverSound;
    public AudioClip powerUpSound;

    //  エフェクトの設定
    public ParticleSystem recoverEffect;
    public ParticleSystem perfectRecoverEffect;
    public ParticleSystem powerUpEffect;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.isAdventure)
        {
            playerHp = PlayerStatusController.instance.playerHp;
            playerAttackDamage = PlayerStatusController.instance.playerAttackDamage;
        }
        else
        {
            playerHp = maxHp;
        }

        // 自身のコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.SetBool("Grounded", true);
        
    }

    private void Update()
    {
        
        /*if (PlayerStatusController.instance.isLevelUp)
        {
            maxHp = PlayerStatusController.instance.playerHp;
            playerAttackDamage = PlayerStatusController.instance.playerAttackDamage;

            PlayerStatusController.instance.isLevelUp = false;
        }*/

        if (isPowerUp)
        {
            powerUpTime += Time.deltaTime;
            if (powerUpTime >= powerUpCountTime)
            {
                playerAttackDamage = playerAttackDamage / 2.0f;
                isPowerUp = false;
            }
        }
        else
        {
            powerUpTime = 0.0f;
            powerUpCount = 0;
        }

        hpBar.fillAmount = playerHp * 0.01f;

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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BattlePoint")
        { 
            gameController.Battle();
            enemy = collision.gameObject;
            animator.SetInteger("AnimState", 0);
        }
        else if (collision.gameObject.tag == "GoalPoint")
        {        
            gameController.GameClear();
            animator.SetInteger("AnimState", 0);
        }
        else if (collision.gameObject.tag == "BranchPoint")
        {
            gameController.Branch();
            animator.SetInteger("AnimState", 0);
        }
        else if (collision.gameObject.tag == "RecoveryItems")
        {
            Recovery(10.0f);
            collision.gameObject.SetActive(false);
            audioSource.PlayOneShot(recoverSound);
            recoverEffect.Play();
        }
        else if (collision.gameObject.tag == "PerfectRecovery")
        {
            Recovery(100.0f);
            audioSource.PlayOneShot(perfectRecoverSound);
            perfectRecoverEffect.Play();
        }
        else if (collision.gameObject.tag == "PowerUpItems")
        {
            PowerUp(2.0f);
            collision.gameObject.SetActive(false);
            audioSource.PlayOneShot(powerUpSound);
            powerUpEffect.Play();
        }

        rb.velocity = new Vector2(0.0f, 0.0f);
    }

    private void Attack()
    {
        // 攻撃モーションをランダムに変化
        int rnd = Random.Range(0, 2);
        animator.SetTrigger(attackAnim[rnd]);

        audioSource.PlayOneShot(attackSound);
        enemy.SendMessage("EnemyDamage", playerAttackDamage);
    }

    public void PlayerDamage(int enemyAttackPoint)
    {
        playerHp -= enemyAttackPoint;
        animator.SetTrigger("Hurt");
        if (playerHp <= 0)
        {
            animator.SetTrigger("Death");
            Invoke("Dead", 1.0f);
        }
    }

    private void Dead()
    {
        gameController.GameOver();
        Invoke("Stop", 0.5f);
    }

    private void Stop()
    {
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

    private void PowerUp(float power)
    {
        
        if (powerUpCount == 0)
        {
            isPowerUp = true;
            playerAttackDamage = playerAttackDamage * power;
            powerUpCount++;
        }
        else if (powerUpCount >= 1)
        {
            // 効果が続いている状態でPowerUpアイテムを取ったらPowerUpTimeを0にする
            powerUpTime = 0.0f;
        }

        
    }
}
