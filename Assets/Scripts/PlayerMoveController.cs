using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*各プレイヤーの移動に関する処理*/
public class PlayerMoveController : MonoBehaviour
{
    // 自身のコンポーネントの変数
    private Rigidbody2D rb;
    private Animator    animator;

    // プレイヤーが動いている状態であるかどうか
    private bool playerMove = false;
    // プレイヤーの移動速度  標準 : 7
    [SerializeField] private float playerSpeed;

    void Start()
    {
        // 自身のコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Grounded", true);
    }

    private void FixedUpdate()
    {
        // AdventureModeの際には動き方が変わる
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
    public void StopPlayer()
    {
        animator.SetInteger("AnimState", 0);
        rb.velocity = new Vector2(0.0f, 0.0f);
    }

    /// <summary>
    /// playerが動いている状態かどうかを変更する関数
    /// GameController内のみ使用
    /// </summary>
    /// <param name="isPlayerMove"></param>
    public void ChangePlayerMove(bool isPlayerMove)
    {
        playerMove = isPlayerMove;
    }
}
