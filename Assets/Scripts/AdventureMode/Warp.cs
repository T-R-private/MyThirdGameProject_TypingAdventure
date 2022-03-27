using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private float warpTransform_x;
    [SerializeField] private float warpTransform_y;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.tag == "Player")
        {
            player.transform.position = new Vector2(warpTransform_x, warpTransform_y);
        }
    }
}
