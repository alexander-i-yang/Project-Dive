using UnityEditor;
using UnityEngine;

namespace Bakers.Editor
{
    [CustomEditor(typeof(WaterfallBaker))]
    public class WaterfallBakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var s = target as WaterfallBaker;
            if(GUILayout.Button("Bake Waterfalls"))
            {
                s.Bake();
            }
        }
    }
}