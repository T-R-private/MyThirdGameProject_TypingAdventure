using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //  Enemyのコンポーネントを取得
    private Animator animator;
    private SpriteRenderer sr;

    // プレイヤーの設定
    private Player player;
    private KingPlayer kingPlayer;

    // エネミーのステータス
    public  float enemyHp;
    public  float maxHp;
    [SerializeField] private int enemyAttackDamage;
    [SerializeField] private float enemyAttackTime;

    // EnemyUIを取得
    public GameObject enemyUI;
    // EnemyのStatusBarをそれぞれ取得
    public Image hpBar;
    public Image attackBar;

    // GameManagerの省略のための変数
    GameManager gameManager;

    private float countTime;

    private void Start()
    {
        // 自身のコンポーネントを取得
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // 初期化
        enemyHp = maxHp;
        gameManager = GameManager.instance;
        gameManager.isEnemyDead = false;

        SetPlayer(gameManager.playerType);
    }

    void SetPlayer(GameManager.PlayerType playerType)
    {
        switch (playerType)
        {
            case GameManager.PlayerType.PLAYER:
                player = GameObject.Find("Player").GetComponent<Player>();
                break;
            case GameManager.PlayerType.KINGPLAYER:
                kingPlayer = GameObject.Find("KingPlayer").GetComponent<KingPlayer>();
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (gameManager.isBattle && sr.isVisible)
        {
            enemyUI.SetActive(true);
            countTime += Time.deltaTime;
            attackBar.fillAmount = countTime / enemyAttackTime;
            if (countTime >= enemyAttackTime)
            {
                countTime = 0.0f;
                Attack();
            }
        }
        else
        {
            countTime = 0.0f;
        }

    }

    
    private void Attack()
    {
        // 敵が倒された後の攻撃しない
        if (gameManager.isEnemyDead) return;
        animator.SetTrigger("Attack");
        if (gameManager.playerType == GameManager.PlayerType.PLAYER)
        {
            player.PlayerDamage(enemyAttackDamage);
        }
        else if (gameManager.playerType == GameManager.PlayerType.KINGPLAYER)
        {
            kingPlayer.PlayerDamage(enemyAttackDamage);
        }

        SoundManager.instance.PlaySE(7);
    }

    public void EnemyDamage(float playerAttackDamage)
    {
        enemyHp -= playerAttackDamage;
        hpBar.fillAmount = (enemyHp / maxHp);
        // ボスでかつHPが0だったらヒットストップ発動
        if (this.gameObject.tag == "BossBattlePoint" && enemyHp <= 0)
        {
            gameManager.isEnemyDead = true;
            animator.speed = 0.2f;
            SoundManager.instance.PlaySE(10);
            animator.SetTrigger("Hurt");
            StartCoroutine(Dead());
        }
        else
        {
            animator.SetTrigger("Hurt");
            if (enemyHp <= 0)
            {
                gameManager.isEnemyDead = true;
                StartCoroutine(Dead());
            }
        }
        
    }
    
    IEnumerator Dead()
    {
        // ヒットポイントの演出待ち時間（BossBattlePointの場合）
        if (this.gameObject.tag == "BossBattlePoint")
        {
            yield return new WaitForSeconds(0.7f);
        }

        animator.speed = 1;
        animator.SetTrigger("Death");
        yield  return new WaitForSeconds(0.5f);

        this.gameObject.SetActive(false);
        gameManager.isBattleClear = true;
        ScorePlus((int)maxHp);
        hpBar.fillAmount = 1;

        if (gameManager.isAdventure)
        {
            PlayerStatusController.instance.GetEp(maxHp);
        }
    }

    private void ScorePlus(int score)
    {
        gameManager.gameRankExpAdd(score);
        gameManager.endlessModeScoreAdd(score);
    }
    
}
