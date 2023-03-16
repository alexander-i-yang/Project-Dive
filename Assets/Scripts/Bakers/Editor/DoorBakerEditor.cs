using Helpers.Bakers;
using UnityEditor;
using UnityEngine;
using World;

namespace Bakers.Editor
{
    [CustomEditor(typeof(DoorBaker))]
    public class DoorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var doorScript = target as DoorBaker;
            if(GUILayout.Button("Calculate Doors"))
            {
                doorScript.CalculateDoorsInScene();
            }
        }
    }
}