using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using Core;
using Helpers;
using UI;
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

        private FireflyIndicatorController _indicatorController;

        void Awake()
        {
            _indicatorController = GetComponentInChildren<FireflyIndicatorController>();
        }

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!Opened)
            {
                PlayerInventory inventory = other.GetComponent<PlayerInventory>();
                PlayerActor actor = other.GetComponent<PlayerActor>();
                if (inventory != null && actor != null)
                {
                    int numFireflies = inventory.NumCollectibles("Firefly");
                    _indicatorController.Show(numFireflies, () => IndicatorShowFinish(inventory, actor));
                }
            }
        }

        public void IndicatorShowFinish(PlayerInventory inventory, PlayerActor actor) {
            int numFireflies = inventory.NumCollectibles("Firefly");
            if (numFireflies >= RequiredFireflies)
            {
                if (_openRoutine == null)
                {
                    _indicatorController.SpecialFinish(delayTime);
                    _openRoutine = StartCoroutine(Helper.DelayAction(delayTime, () => OnRequirementMet(actor)));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!Opened)
            {
                PlayerInventory inventory = other.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    _indicatorController.Hide();
                }
            }
        }
    }
}