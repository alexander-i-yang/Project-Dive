using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    //L: Another script I stole from Boomo
    public class DrillingParticles : MonoBehaviour
    {
        [SerializeField] private bool destroyInsteadOfDisable;
        [SerializeField] private float timeToDestroy = 1;

        private PlayerActor p;

        void Start()
        {
            StartCoroutine(DestroyMe(timeToDestroy));
            p = GetComponentInParent<PlayerActor>();
        }

        public void Stop() {
            GetComponent<ParticleSystem>().Stop();
            transform.parent = null;
        }

        private IEnumerator DestroyMe(float time)
        {
            yield return new WaitForSeconds(time);

            if (p.IsDrilling()) {
                StartCoroutine(DestroyMe(timeToDestroy));
                GetComponent<ParticleSystem>().Play();
                transform.parent = p.transform;
            } else {
                if (destroyInsteadOfDisable)
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);
            }
        }
    }
}