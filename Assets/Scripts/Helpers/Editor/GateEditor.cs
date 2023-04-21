using Mechanics;
using Player;
using UnityEditor;
using UnityEngine;

namespace Helpers.Editor
{
    [CustomEditor(typeof(Gate))]
    public class GateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var gate = target as Gate;
            if(GUILayout.Button("Open"))
            {
                gate.OnRequirementMet(PlayerCore.Actor);
            }
            if(GUILayout.Button("Special Finish"))
            {
                gate.IndicatorShowFinish(PlayerCore.Instance.GetComponent<PlayerInventory>(), PlayerCore.Actor);
            }
            if(GUILayout.Button("Close"))
            {
                gate.Reset();
            }
        }
    }
}