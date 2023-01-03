using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using Core;

namespace Mechanics
{
    public class Gate : MonoBehaviour
    {
        [SerializeField] private int requiredFireflies;

        [SerializeField] private GateAnimator gateL;
        [SerializeField] private GateAnimator gateR;


        public bool Opened { get; private set; }

        public void OnRequirementMet()
        {
            //This seems stupid, but there's gonna be more complicated stuff later.
            Open();
        }

        public void Open()
        {
            gateL.PlayAnimation(OnFinishOpen);
            gateR.PlayAnimation();
            Opened = true;
            Debug.Log("Gate Opened");
        }

        public void OnFinishOpen()
        {

        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!Opened)
            {
                PlayerInventory inventory = other.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    int numFireflies = inventory.NumCollectibles("Firefly");

                    if (numFireflies >= requiredFireflies)
                    {
                        OnRequirementMet();
                    }
                }
            }
        }
    }
}