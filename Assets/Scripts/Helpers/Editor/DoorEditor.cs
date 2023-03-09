using Mechanics;
using UnityEditor;
using UnityEngine;
using World;

namespace Helpers.Editor
{
    [CustomEditor(typeof(Door))]
    public class DoorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var doorScript = target as Door;
            if(GUILayout.Button("Calculate Doors"))
            {
                doorScript.CalculateDoorsInScene();
            }
        }
    }
}