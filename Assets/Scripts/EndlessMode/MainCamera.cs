using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    //　プレイヤーの取得
    public Player player;
    //  x軸のプレイヤーとの距離
    [SerializeField] private float playerDistance_x = 6.5f;
    [SerializeField] private float playerDistance_y = 3.2f;

    
    private void LateUpdate()
    {
        //横方向だけ追従
        if (!player.isDead)
        {
            transform.position =
            new Vector3(player.transform.position.x + playerDistance_x, player.transform.position.y + playerDistance_y, transform.position.z);
        }
            
        　　　
    }


}
