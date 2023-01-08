using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Mechanics
{
    public class ConnectedSpike : DirectionalSpike
    {
        public override void Discharge(HashSet<Spike> dogoDisabledSpikes)
        {
            if (!Charged) return;
            Vector3 offsetLeft = new Vector3(8, -3.5f);
            Vector3 offsetRight = new Vector3(-8, -3.5f);
            
            DischargeNext(dogoDisabledSpikes, offsetLeft, 1);
            DischargeNext(dogoDisabledSpikes, offsetRight, 1);
        }

        public void DischargeNext(HashSet<Spike> dogoDisabledSpikes, Vector3 offset, int index)
        {
            Action dischargeAnimation = () => StartCoroutine(Helper.DelayAction(index * 0.025f, DischargeAnimation));
            DischargeLogic(dogoDisabledSpikes, dischargeAnimation);
            
            Vector3 checkPos = transform.position + offset;
            ConnectedSpike s = Helper.OnComponent<ConnectedSpike>(checkPos);
            if (s != null && s.Charged)
            {
                s.DischargeNext(dogoDisabledSpikes, offset, index+1);
            }
        }
    }
}