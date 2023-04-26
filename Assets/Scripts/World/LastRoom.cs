using Helpers;
using UnityEngine;

namespace World
{
    public class LastRoom : Room
    {
        [SerializeField] private float delaySwitch = 0.5f;
        private void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += OnEndCutscene;
        }
        
        private void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= OnEndCutscene;
        }

        private void OnEndCutscene()
        {
            StartCoroutine(Helper.DelayAction(delaySwitch, SwitchCamera));
        }
    }
}