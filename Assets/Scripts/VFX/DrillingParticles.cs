using Player;
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

        private GameTimer checkDestroyTimer;

        private PlayerActor Player => PlayerCore.Actor;

        void Awake()
        {
            checkDestroyTimer = GameTimer.StartNewTimer(timeToDestroy, "Check Destroy Timer");
        }

        private void OnEnable()
        {
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
            transform.parent = null;
        }

        private void CheckDestroy()
        {
            if (Player.IsDrilling())
            {
                GetComponent<ParticleSystem>().Play();
                //transform.parent = P.transform;
                GameTimer.Reset(checkDestroyTimer);
            } else
            {
                GameTimer.Clear(checkDestroyTimer);
                DestroyOrDisable();
            }
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