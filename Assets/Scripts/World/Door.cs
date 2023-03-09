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

        public Vector2 DoorAdjacencyTolerance = Vector2.one * 4;
        public LayerMask DoorLayerMask;

        private void Awake()
       {
           _player = FindObjectOfType<PlayerSpawnManager>(true);
       }

       private void OnTriggerEnter2D(Collider2D other)
       {
           bool isPlayer = other.gameObject == _player.gameObject;
           bool needTransition = _player.CurrentRoom != this;
           if (isPlayer && needTransition)
           {
               r.RoomSetEnable(true);
           }
       }
       
       private void OnTriggerExit2D(Collider2D other)
       {
           bool isPlayer = other.gameObject == _player.gameObject;
           bool needTransition = _player.CurrentRoom != this;
           if (isPlayer && needTransition && !_player.IsTouchingRoom(r))
           {
               r.RoomSetEnable(false);
           }
       }

       public void CalculateDoorsInScene()
       {
           Room[] rooms = FindObjectsOfType<Room>();
           foreach (var room in rooms)
           {
               Door[] adjDoors = room.CalcAdjacentDoors(DoorAdjacencyTolerance, DoorLayerMask);
               foreach (var door in adjDoors)
               {
                   door.r = room;
                    #if UNITY_EDITOR
                   // var serializedDoor = new SerializedObject(door);
                   // SerializedProperty id = serializedDoor.FindProperty("r");
                   // id.objectReferenceValue = room;
                   EditorUtility.SetDirty(door);
                    #endif
               }
           }
       }
    }
}