using System.Collections.Generic;

using UnityEngine;

using Player;

namespace Mechanics
{
    public class PlayerSpikeResponse : MonoBehaviour, ISpikeResponse
    {
        private IPlayerActionHandler _playerAction;
        private PlayerStateMachine _playerSM;

        private HashSet<Spike> _dogoDisabledSpikes = new HashSet<Spike>();

        private void Awake()
        {
            _playerAction = GetComponent<IPlayerActionHandler>();
            _playerSM = GetComponent<PlayerStateMachine>();
        }

        private void OnEnable()
        {
            _playerSM.OnPlayerDeath += RechargeSpikes;
            _playerSM.StateTransition += OnPlayerStateChanged;
        }

        private void OnDisable()
        {
            _playerSM.OnPlayerDeath -= RechargeSpikes;
            _playerSM.StateTransition -= OnPlayerStateChanged;
        }

        public void OnSpikeEnter(Spike spike)
        {
            if (_playerSM.IsOnState<PlayerStateMachine.Diving>())
            {
                _dogoDisabledSpikes.Add(spike);
                spike.Discharge();
            }
            else if (spike.Charged)
            {
                _playerAction.Die();
            }
        }

        private void OnPlayerStateChanged()
        {
            if (_playerSM.IsOnState<PlayerStateMachine.DogoJumping>())
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
