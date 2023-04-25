using Helpers;
using UnityEngine;
using UnityEngine.UI;
using World;

namespace UI
{
    public class DiveLogoFadein : MonoBehaviour
    {
        private Image im;
        [SerializeField] private float len = 1f;
        [SerializeField] private float delay;
        
        void Awake()
        {
            im = GetComponent<Image>();
        }
        
        void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += FadeIn;
        }
        
        void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= FadeIn;
        }

        void FadeIn()
        {
            StartCoroutine(Helper.DelayAction(
                delay, () =>
                {
                    print("Fade now");
                    StartCoroutine(Helper.Fade(im, len, Color.white));
                })
            );
        }
    }
}