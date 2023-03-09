using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustPlayerPosToCrawl : MonoBehaviour
{
    [SerializeField] private Transform playerPosTitle;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 playerPos = PlayerCore.Actor.transform.position;
        PlayerCore.Actor.ApplyVelocity(new Vector2(-PlayerCore.Actor.velocityX, 0));    //Cancel out x velocity
        PlayerCore.Actor.transform.position = new Vector3(playerPosTitle.position.x, playerPos.y, playerPos.z);

    }
}
