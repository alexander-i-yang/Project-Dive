using Mechanics;
using Player;
using UI;
using UnityEditor;
using UnityEngine;

namespace Helpers.Editor
{
    [CustomEditor(typeof(FireflyIndicatorController))]
    public class FIControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var targ = target as FireflyIndicatorController;
            if(GUILayout.Button("Show"))
            {
                targ.Show(4, () => {});
            }
            if(GUILayout.Button("Hide"))
            {
                targ.Hide();
            }
        }
    }
}