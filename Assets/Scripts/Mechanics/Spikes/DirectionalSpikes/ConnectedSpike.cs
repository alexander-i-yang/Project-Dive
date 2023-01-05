using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Mechanics
{
    public class ConnectedSpike : DirectionalSpike
    {
        public override void Discharge(HashSet<Spike> dogoDisabledSpikes) {
            if (Charged == true)
            {
                base.Discharge(dogoDisabledSpikes);
                Vector3 checkPosRight = transform.position + new Vector3(8, -3.5f);
                Vector3 checkPosLeft = transform.position + new Vector3(-8, -3.5f);
                
                DisableSpike(checkPosRight, dogoDisabledSpikes);
                DisableSpike(checkPosLeft, dogoDisabledSpikes);
            }
        }

        private void DisableSpike(Vector3 checkPos, HashSet<Spike> dogoDisabledSpikes)
        {
            ConnectedSpike s = Helper.OnComponent<ConnectedSpike>(checkPos);
            if (s != null)
            {
                s.Discharge(dogoDisabledSpikes);
            }
        }
    }
}