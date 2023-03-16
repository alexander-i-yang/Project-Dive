using UnityEditor;
using UnityEngine;
using World;

namespace Helpers.Bakers
{
    public class DoorBaker : MonoBehaviour
    {
        public Vector2 DoorAdjacencyTolerance = Vector2.one * 4;
        public LayerMask DoorLayerMask;

        
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
                    EditorUtility.SetDirty(door);
                    #endif
                }
            }
        }
    }
}