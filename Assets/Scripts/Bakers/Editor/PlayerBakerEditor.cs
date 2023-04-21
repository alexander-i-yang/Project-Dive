using UnityEditor;
using UnityEngine;

namespace Bakers.Editor
{
    [CustomEditor(typeof(PlayerBaker))]
    public class PlayerBakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var playerScript = target as PlayerBaker;
            if(GUILayout.Button("Bake Player"))
            {
                playerScript.Bake();
            }
        }
    }
}