using Mechanics;
using UnityEditor;
using UnityEngine;

namespace Helpers.Editor
{
    [CustomEditor(typeof(MushroomUnlockLogic))]
    public class MushroomUnlockEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var mushroomScript = target as MushroomUnlockLogic;
            if(GUILayout.Button("Unlock"))
            {
                mushroomScript.UnlockAllMushrooms();
            }
        }
    }
}