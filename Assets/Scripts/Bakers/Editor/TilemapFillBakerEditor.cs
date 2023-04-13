using UnityEditor;
using UnityEngine;

namespace Bakers.Editor
{
    [CustomEditor(typeof(TilemapFillBaker))]
    public class TilemapFillBakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var s = target as TilemapFillBaker;
            if(GUILayout.Button("Set Tile"))
            {
                s.SetTile();
            }
            if(GUILayout.Button("Set Fill Map"))
            {
                s.SetFillMap();
            }
            if(GUILayout.Button("Clear Tiles"))
            {
                s.ClearTiles();
            }
            if(GUILayout.Button("Calculate Points"))
            {
                s.CalculatePoints();
            }
            if(GUILayout.Button("Fill Tiles"))
            {
                s.SetTiles();
            }
            if(GUILayout.Button("Bake Tilemaps"))
            {
                s.Bake();
            }
        }
    }
}