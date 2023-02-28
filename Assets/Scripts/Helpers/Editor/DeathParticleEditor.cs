using UnityEditor;
using UnityEngine;
using VFX;

namespace Helpers.Editor
{
    [CustomEditor(typeof(Death))]
    public class DeathParticleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var death = target as Death;
            if(GUILayout.Button("Launch"))
            {
                death.Launch();
            }
            if(GUILayout.Button("Reset"))
            {
                death.Reset();
            }
        }
    }
}