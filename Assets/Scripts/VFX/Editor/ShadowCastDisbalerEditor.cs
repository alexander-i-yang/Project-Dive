using UnityEditor;
using UnityEngine;

namespace VFX.Editor
{
    [CustomEditor(typeof(ShadowCastDisabler))]
    public class ShadowCastDisbalerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var targ = target as ShadowCastDisabler;
            if(GUILayout.Button("Enable"))
            {
                targ.SetShadowsEnabled(true);
            }
            
            if(GUILayout.Button("Disable"))
            {
                targ.SetShadowsEnabled(false);
            }
        }
    }
}