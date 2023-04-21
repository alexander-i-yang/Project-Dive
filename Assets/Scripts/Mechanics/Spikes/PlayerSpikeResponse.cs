using System.Collections.Generic;

using UnityEngine;

using Player;

namespace Mechanics
{
    public class PlayerSpikeResponse : MonoBehaviour, ISpikeResponse
    {
        private HashSet<Spike> _dogoDisabledSpikes = new HashSet<Spike>();

        private void Start()
        {
            PlayerCore.StateMachine.StateTransition += OnPlayerStateChanged;
        }

        private void OnDisable()
        {
            PlayerCore.StateMachine.StateTransition -= OnPlayerStateChanged;
        }

        public void OnSpikeEnter(Spike spike)
        {
            if (PlayerCore.StateMachine.UsingDrill)
            {
                spike.Discharge(_dogoDisabledSpikes);
            }
            else if (spike.Charged)
            {
                bool shouldDie = true;
                var directionalSpike = spike as DirectionalSpike;
                if (directionalSpike != null)
                {
                    shouldDie = directionalSpike.ShouldDieFromVelocity(PlayerCore.Actor.velocity);
                }
                
                if (shouldDie) PlayerCore.Actor.Die(spike.RecoilFunc);
            }
        }

        private void OnPlayerStateChanged()
        {
            if (!PlayerCore.StateMachine.UsingDrill)
            {
                RechargeSpikes();
            }
        }

        private void RechargeSpikes()
        {
            foreach (Spike spike in _dogoDisabledSpikes)
            {
                if (spike != null) spike.Recharge();
            }
            _dogoDisabledSpikes.Clear();
        }
    }
}
