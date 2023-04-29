using ASK.UI;
using UnityEditor;
using UnityEngine;
using VFX;

namespace VFX.Editor
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