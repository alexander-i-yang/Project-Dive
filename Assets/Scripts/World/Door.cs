using System;
using Player;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace World
{
    [Serializable]
    public class Door : MonoBehaviour
    {
        [SerializeField] public Room r;
        private PlayerSpawnManager _player;
        
        private void Awake()
       {
           _player = FindObjectOfType<PlayerSpawnManager>(true);
       }

       private void OnTriggerEnter2D(Collider2D other)
       {
           bool isPlayer = other.gameObject == _player.gameObject;
           bool needTransition = _player.CurrentRoom != r;
           if (isPlayer && needTransition)
           {
               print(_player.CurrentRoom);
               print(r);
               r.RoomSetEnable(true);
           }
       }
       
       private void OnTriggerExit2D(Collider2D other)
       {
           bool isPlayer = other.gameObject == _player.gameObject;
           bool needTransition = _player.CurrentRoom != r;
           if (isPlayer && needTransition && !_player.IsTouchingRoom(r))
           {
               r.RoomSetEnable(false);
           }
       }
    }
}