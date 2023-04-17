using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using Core;
using Helpers;
using UnityEngine.Serialization;
using VFX;

namespace Mechanics
{
    public class Gate : MonoBehaviour
    {
        public int RequiredFireflies;

        [SerializeField] private GateAnimator gateL;
        [SerializeField] private GateAnimator gateR;

        [SerializeField] private FadeLightAnimator[] otherLights;

        [SerializeField] private float delayTime = 1f;
        private Coroutine _openRoutine;

        public bool Opened { get; private set; }

        public void OnRequirementMet(PlayerActor p)
        {
            Open();
            GetComponent<Animator>().Play("Gate_Open");
            p.FloorDisappear();
        }

        public void Open()
        {
            gateL.gameObject.SetActive(false);
            gateR.gameObject.SetActive(false);
            Opened = true;
            var emissionModule = GetComponentInChildren<ParticleSystem>().emission;
            emissionModule.rateOverTime = 0;
            GetComponentInChildren<FadeLightAnimator>().FadeOut();
            foreach (var f in otherLights) f.FadeOut();
        }

        public void Reset()
        {
            gateL.gameObject.SetActive(true);
            gateR.gameObject.SetActive(true);
            GetComponent<Animator>().Play("Gate_None");
            Opened = false;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!Opened)
            {
                PlayerInventory inventory = other.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    int numFireflies = inventory.NumCollectibles("Firefly");
                    if (numFireflies >= RequiredFireflies)
                    {
                        PlayerActor p = other.GetComponent<PlayerActor>();
                        if (p != null && _openRoutine == null)
                        {
                            _openRoutine = StartCoroutine(Helper.DelayAction(delayTime, () => OnRequirementMet(p)));
                        }
                    }
                }
            }
        }
    }
}