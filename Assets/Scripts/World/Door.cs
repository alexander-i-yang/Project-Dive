using System;
using System.Collections.Generic;
using Player;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace World
{
    [Serializable]
    public class Door : MonoBehaviour
    {
        [FormerlySerializedAs("r")] [SerializeField] public Room connectRoom;
        private PlayerSpawnManager _player;

        [FormerlySerializedAs("TransitionRooms")] public List<Room> AdjacentRooms = new();
        
        private void Awake()
       {
           _player = FindObjectOfType<PlayerSpawnManager>(true);
       }

       private void OnTriggerEnter2D(Collider2D other)
       {
           /*bool isPlayer = other.gameObject == _player.gameObject;
           bool needTransition = _player.CurrentRoom != connectRoom;
           if (isPlayer && needTransition)
           {
               connectRoom.RoomSetEnable(true);
               foreach (var r in AdjacentRooms) r.RoomSetEnable(true);
           }*/
       }
       
       private void OnTriggerExit2D(Collider2D other)
       {
           /*bool isPlayer = other.gameObject == _player.gameObject;
           bool needTransition = _player.CurrentRoom != connectRoom;
           if (isPlayer && needTransition && !_player.IsTouchingRoom(connectRoom))
           {
               connectRoom.RoomSetEnable(false);
               foreach (var r in AdjacentRooms) {
                   r.RoomSetEnable(false);
               }
           }*/
       }

       public void CalcTransitionRooms(Vector2 screenSize, LayerMask roomLayerMask)
       {
           AdjacentRooms.Clear();
           Room myRoom = GetComponentInParent<Room>();
           Vector2 doorPos = GetComponent<Collider2D>().bounds.center;
           Collider2D[] overlapRooms = Physics2D.OverlapAreaAll(
               doorPos - screenSize / 2, 
               doorPos + screenSize / 2, 
               roomLayerMask
           );
           foreach (var c in overlapRooms)
           {
               var r = c.GetComponent<Room>();
               if (r != null && r != myRoom && r != connectRoom) AdjacentRooms.Add(r);
           }
       }
    }
}