using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class InputDisabler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col) {
        // disables player input for start menu
        PlayerCore.Input.enabled = false;
    }
}
