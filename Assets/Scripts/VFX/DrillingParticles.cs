using Helpers;
using Player;
using UnityEngine;

namespace VFX
{
    public class DrillingParticles : MonoBehaviour
    {
        [SerializeField] private bool destroyInsteadOfDisable;
        [SerializeField] private float timeToDestroy = 1;

        private GameTimer checkDestroyTimer;

        private PlayerActor Player => PlayerCore.Actor;

        /*void Awake()
        {
            checkDestroyTimer = GameTimer.StartNewTimer(timeToDestroy, "Check Destroy Timer");
        }*/

        private void OnEnable()
        {
            transform.parent = null;
            checkDestroyTimer.OnFinished += CheckDestroy;
        }

        private void OnDisable()
        {
            checkDestroyTimer.OnFinished -= CheckDestroy;
        }

        private void Update()
        {
            GameTimer.Update(checkDestroyTimer);
        }

        public void Stop() {
            GetComponent<ParticleSystem>().Stop();
            checkDestroyTimer = GameTimer.StartNewTimer(timeToDestroy, "Check Destroy Timer");
        }

        private void CheckDestroy()
        {
            DestroyOrDisable();
        }

        private void DestroyOrDisable()
        {
            if (destroyInsteadOfDisable)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}