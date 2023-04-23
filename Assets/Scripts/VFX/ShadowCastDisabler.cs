using System;
using UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VFX
{
    public class ShadowCastDisabler : MonoBehaviour
    {
        [SerializeField] private ShadowCaster2D[] casters;

        private void OnEnable()
        {
            GraphicsQualityToggleReceiver.ToggleEvent += SetShadowsEnabled;
        }
        
        private void OnDisable()
        {
            GraphicsQualityToggleReceiver.ToggleEvent -= SetShadowsEnabled;
        }

        public void SetShadowsEnabled(bool e)
        {
            foreach (var caster in casters)
            {
                if (caster != null) caster.enabled = e;
            }
        }

        public void Bake()
        {
            casters = FindObjectsOfType<ShadowCaster2D>();
            print($"Baked {casters.Length} ShadowCasters into the SC disabler");
        }
    }
}