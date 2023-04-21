using System;
using UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VFX
{
    public class ShadowCastDisabler : MonoBehaviour
    {
        [SerializeField] private ShadowCaster2D[] _casters;
        
        void Awake()
        {
            _casters = FindObjectsOfType<ShadowCaster2D>();
        }

        private void OnEnable()
        {
            OptionsController.GraphicsQualityToggleEvent += SetShadowsEnabled;
        }
        
        private void OnDisable()
        {
            OptionsController.GraphicsQualityToggleEvent -= SetShadowsEnabled;
        }

        public void SetShadowsEnabled(bool e)
        {
            foreach (var caster in _casters)
            {
                caster.enabled = e;
            }
        }
    }
}