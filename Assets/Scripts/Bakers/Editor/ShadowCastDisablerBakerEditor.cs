using UnityEditor;
using UnityEngine;

namespace Bakers.Editor
{
    [CustomEditor(typeof(ShadowCastDisablerBaker))]
    public class ShadowCastDisablerBakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var targ = target as ShadowCastDisablerBaker;
            if(GUILayout.Button("Bake SCD"))
            {
                targ.Bake();
            }
        }
    }
}