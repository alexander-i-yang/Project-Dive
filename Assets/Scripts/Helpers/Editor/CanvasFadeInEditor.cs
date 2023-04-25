using UI;
using UnityEditor;
using UnityEngine;
using VFX;

namespace Helpers.Editor
{
    [CustomEditor(typeof(CanvasFadeIn))]
    public class CanvasFadeInEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var targ = target as CanvasFadeIn;
            if(GUILayout.Button("Fade In"))
            {
                targ.FadeIn();
            }
        }
    }
}