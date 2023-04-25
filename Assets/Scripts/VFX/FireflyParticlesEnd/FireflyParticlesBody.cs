using Helpers;
using UnityEngine;
using UnityEngine.Events;
using World;

namespace VFX.FireflyParticlesEnd
{
    public class FireflyParticlesBody : MonoBehaviour
    {
        [SerializeField] private GameObject pSystemPrefab;
        [SerializeField] private int numToEmit;
        [SerializeField] private int setEmission;
        [SerializeField] private float delay;

        [SerializeField] private Transform follow;
        [SerializeField] private Vector3 offset;

        [SerializeField] private UnityEvent onEmit;

        void Awake()
        {
            // _particleSystem = GetComponent<ParticleSystem>();
        }
        
        void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += Emit;
        }
        
        void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= Emit;
        }

        void Emit()
        {
            StartCoroutine(Helper.DelayAction(
                delay,
                () =>
                {
                    ParticleSystem p;
                    if (pSystemPrefab != null)
                    {
                        var g = Instantiate(pSystemPrefab, transform);
                        p = g.GetComponent<ParticleSystem>();
                    }
                    else
                    {
                        p = GetComponent<ParticleSystem>();
                    }
                    
                    p.Emit(numToEmit);
                    onEmit.Invoke();
                    var e = p.emission;
                    e.rateOverTime = setEmission;
                })
            );
        }
    }
}