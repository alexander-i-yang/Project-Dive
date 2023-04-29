using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASK.Helpers
{

    //L: Another script I stole from Boomo
    public class DestroySelf : MonoBehaviour
    {
        [SerializeField] private bool destroyInsteadOfDisable;
        [SerializeField] private float timeToDestroy = 5;

        void Start()
        {
            StartCoroutine(DestroyMe(timeToDestroy));
        }

        private IEnumerator DestroyMe(float time)
        {
            yield return new WaitForSeconds(time);

            if (destroyInsteadOfDisable)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}