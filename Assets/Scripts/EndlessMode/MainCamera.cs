using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MainCameraの制御を行うスクリプト */
public class MainCamera : MonoBehaviour
{
    //　プレイヤーの取得
    public Player        player;
    public KingPlayer    kingPlayer;
    public WizzardPlayer wizzardPlayer;

    //  x軸のプレイヤーとの距離
    [SerializeField] private float playerDistance_x = 6.5f;
    [SerializeField] private float playerDistance_y = 3.2f;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;       
    }

    private void LateUpdate()
    {
        //横方向だけ追従
        if (player.isDead || kingPlayer.isDead || wizzardPlayer.isDead)
        {
            // 何もしない
        }
        else
        {
            if (gameManager.playerType == GameManager.PlayerType.KNIGHT)
            {
                transform.position = new Vector3(player.transform.position.x + playerDistance_x, player.transform.position.y + playerDistance_y, transform.position.z);
            }
            else if (gameManager.playerType == GameManager.PlayerType.KINGPLAYER)
            {
                transform.position = new Vector3(kingPlayer.transform.position.x + playerDistance_x, kingPlayer.transform.position.y + playerDistance_y - 1.0f, transform.position.z);
            }
            else if (gameManager.playerType == GameManager.PlayerType.WIZZARDPLAYER)
            {
                transform.position = new Vector3(wizzardPlayer.transform.position.x + playerDistance_x, wizzardPlayer.transform.position.y + playerDistance_y - 1.0f, transform.position.z);
            }
        }    　　　
    }
}
