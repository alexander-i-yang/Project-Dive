using UnityEditor;
using UnityEngine;

namespace Bakers
{
    public class PlayerBaker : MonoBehaviour, IBaker
    {
        public Vector2 PlayerPos;

        public void Bake()
        {
            PlayerActor player = FindObjectOfType<PlayerActor>();
            if (player != null)
            {
                player.transform.position = PlayerPos;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(player);
                #endif
            }
        }
    }
}