using UnityEditor;
using UnityEngine;

namespace Bakers.Editor
{
    [CustomEditor(typeof(DoorBaker))]
    public class DoorBakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var doorScript = target as DoorBaker;
            if(GUILayout.Button("Calculate Doors"))
            {
                doorScript.Bake();
            }
        }
    }
}