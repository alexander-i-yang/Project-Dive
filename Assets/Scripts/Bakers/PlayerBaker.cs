using UnityEngine;

namespace Bakers
{
    public class PlayerBaker : MonoBehaviour
    {
        public Vector2 PlayerPos;

        public void ResetPlayerPos()
        {
            PlayerActor player = FindObjectOfType<PlayerActor>();
            if (player != null)
            {
                player.transform.position = PlayerPos;
            }
        }
    }
}