using Helpers;
using UnityEngine;

namespace VFX
{
    public class DustImpactSpawner : MonoBehaviour
    {
        private ParticleSystem _psystem;
        
        [SerializeField] private int numToEmit;
        [SerializeField] private float duration;

        public void Init()
        {
            _psystem = GetComponent<ParticleSystem>();
            Emit();
        }

        public void ReInit(Vector3 coord)
        {
            gameObject.SetActive(true);
            transform.position = coord;
            _psystem.Clear();
            Emit();
        }

        private void Emit()
        {
            _psystem.Emit(numToEmit);
            StartCoroutine(Helper.DelayAction(duration, () =>
            {
                gameObject.SetActive(false);
            }));
        }
    }
}