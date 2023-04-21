using UnityEditor;
using UnityEngine;
using VFX;

namespace Helpers.Editor
{
    [CustomEditor(typeof(DeathAnimationManager))]
    public class DeathParticleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var death = target as DeathAnimationManager;
            if(GUILayout.Button("Launch"))
            {
                death.Launch(Vector3.zero);
            }
        }
    }
}