using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class StartMenuInputTrigger : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private void OnTriggerEnter2D(Collider2D col) {
        // disables player input for start menu
        player.GetComponent<PlayerInputController>().enabled = false;
    }
}
