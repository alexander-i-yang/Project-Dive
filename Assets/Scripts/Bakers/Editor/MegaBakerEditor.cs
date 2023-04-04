using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Bakers.Editor
{
    [CustomEditor(typeof(MegaBaker))]
    public class MegaBakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var s = target as MegaBaker;
            if(GUILayout.Button("Bake All"))
            {
                s.BakeAll();
            }
        }

        [Shortcut("Bake All", KeyCode.B, ShortcutModifiers.Action | ShortcutModifiers.Alt)]
        public static void ShortcutBake()
        {
            FindObjectOfType<MegaBaker>().BakeAll();
        }
    }
}