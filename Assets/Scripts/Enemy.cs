using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //  Enemyのコンポーネントを取得
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer sr;

    // プレイヤーの設定
    private Player player;

    // エネミーのステータス
    public  float enemyHp;
    public  float maxHp;
    [SerializeField] private float enemyAttackDamage;
    [SerializeField] private float enemyAttackTime;

    // EnemyUIを取得
    public GameObject enemyUI;
    // EnemyのStatusBarをそれぞれ取得
    public Image hpBar;
    public Image attackBar;
    // attack効果音の設定
    public AudioClip attackSound;

    private float countTime;

    private void Start()
    {
        // Playerの情報を取得
        GameObject playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Player>();

        // 自身のコンポーネントを取得
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();

        // 初期化
        enemyHp = maxHp;
        
    }

    private void Update()
    {
        if (GameManager.instance.isBattle && sr.isVisible)
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
        animator.SetTrigger("Attack");
        player.SendMessage("PlayerDamage", enemyAttackDamage);
        audioSource.PlayOneShot(attackSound);
        
    }

    public void EnemyDamage(float playerAttackDamage)
    {
        enemyHp -= playerAttackDamage;
        hpBar.fillAmount = (enemyHp / maxHp);
        animator.SetTrigger("Hurt");
        if (enemyHp <= 0)
        {
            animator.SetTrigger("Death");
            Invoke("Dead", 0.5f);
        }
    }
    
    private void Dead()
    {
        this.gameObject.SetActive(false);
        GameManager.instance.isBattleClear = true;
        EndlessModeScorePlus((int)maxHp);
        hpBar.fillAmount = 1;

        if (GameManager.instance.isAdventure)
        {
            PlayerStatusController.instance.GetEp(maxHp);
        }
    }

    private void EndlessModeScorePlus(int score)
    {
        GameManager.instance.endlessModeScore += score;
    }
    
}
