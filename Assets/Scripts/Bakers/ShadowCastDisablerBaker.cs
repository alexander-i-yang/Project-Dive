using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VFX;

namespace Bakers
{
    public class ShadowCastDisablerBaker : MonoBehaviour, IBaker
    {
        public void Bake()
        {
            var disabler = FindObjectOfType<ShadowCastDisabler>();
            disabler.Bake();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(disabler);
            #endif
        }
    }
}