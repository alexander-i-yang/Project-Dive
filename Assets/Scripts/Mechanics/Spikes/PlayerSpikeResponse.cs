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
            PlayerCore.StateMachine.OnPlayerDeath += RechargeSpikes;
            PlayerCore.StateMachine.StateTransition += OnPlayerStateChanged;
        }

        private void OnDisable()
        {
            PlayerCore.StateMachine.OnPlayerDeath -= RechargeSpikes;
            PlayerCore.StateMachine.StateTransition -= OnPlayerStateChanged;
        }

        public void OnSpikeEnter(Spike spike)
        {
            if (PlayerCore.StateMachine.IsOnState<PlayerStateMachine.Diving>())
            {
                _dogoDisabledSpikes.Add(spike);
                spike.Discharge();
            }
            else if (spike.Charged)
            {
                PlayerCore.Actor.Die();
            }
        }

        private void OnPlayerStateChanged()
        {
            if (PlayerCore.StateMachine.IsOnState<PlayerStateMachine.DogoJumping>())
            {
                RechargeSpikes();
            }
        }

        private void RechargeSpikes()
        {
            foreach (Spike spike in _dogoDisabledSpikes)
            {
                spike.Recharge();
            }
            _dogoDisabledSpikes.Clear();
        }
    }
}
