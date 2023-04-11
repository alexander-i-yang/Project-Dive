using UnityEditor;
using UnityEngine;
using World;

namespace Bakers
{
    public class DoorBaker : MonoBehaviour, IBaker
    {
        public Vector2 DoorAdjacencyTolerance = Vector2.one * 4;
        public LayerMask DoorLayerMask;
        public LayerMask RoomLayerMask;

        
        public void Bake()
        {
            Room[] rooms = FindObjectsOfType<Room>();
            foreach (var room in rooms)
            {
                Door[] adjDoors = room.CalcAdjacentDoors(DoorAdjacencyTolerance, DoorLayerMask);
                foreach (var door in adjDoors)
                {
                    door.connectRoom = room;
                    #if UNITY_EDITOR
                    EditorUtility.SetDirty(door);
                    #endif
                }

                Room[] adjRooms = room.CalcAdjacentRooms(DoorAdjacencyTolerance, RoomLayerMask);
                room.AdjacentRooms = adjRooms;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(room);
                #endif
            }

            foreach (var door in FindObjectsOfType<Door>())
            {
                door.CalcTransitionRooms(Core.Game.Instance.ScreenSize, RoomLayerMask);
            }
        }
    }
}