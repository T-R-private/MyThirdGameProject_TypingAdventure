using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Playerがコライダーを検知した時に発生するイベントのスクリプト*/
public class PlayerEventController : MonoBehaviour
{
    /*
    // ゲームコントローラーの取得
    public  GameController gameController;
    private Player player;
    private KingPlayer kingPlayer;
    private WizzardPlayer wizzardPlayer;
    private PlayerMoveController playerMoveController;

    // enemyオブジェクトの取得
    public GameObject enemyObj;

    // Start is called before the first frame update
    
    void Start()
    {
        SetPlayer(GameManager.instance.playerType);
        playerMoveController = GetComponent<PlayerMoveController>();
    }

    (Player,KingPlayer,WizzardPlayer) SetPlayer(GameManager.PlayerType playerType)
    {
        switch (playerType)
        {
            case GameManager.PlayerType.PLAYER:
                player = GetComponent<Player>();
                return player;
            case GameManager.PlayerType.KINGPLAYER:
                kingPlayer = GetComponent<KingPlayer>();
                return kingPlayer;
            case GameManager.PlayerType.WIZZARDPLAYER:
                wizzardPlayer = GetComponent<WizzardPlayer>();
                return wizzardPlayer;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
                playerMoveController.StopPlayer();
                break;
            case "BranchPoint":
                gameController.Branch();
                enemyObj = collision.gameObject;
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
                PowerUp(2.0f);
                collision.gameObject.SetActive(false);
                SoundManager.instance.PlaySE(3);
                break;
        }

    }

    private void BattleMove(GameObject collision)
    {
        gameController.Battle(collision.gameObject.tag);
        enemyObj = collision.gameObject;
        playerMoveController.StopPlayer();
    }
    
    */
}
