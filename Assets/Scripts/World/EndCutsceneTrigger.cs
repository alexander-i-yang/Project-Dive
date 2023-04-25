using System;
using UnityEngine;

namespace World
{
    public class EndCutsceneTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var p = other.gameObject.GetComponent<PlayerActor>();
            if (p != null && p.ShouldEndCutscene())
            {
                EndCutsceneManager.StartCutscene();
            }
        }
    }
}