using System.Collections.Generic;

using UnityEngine;

using Player;

namespace Mechanics
{
    public class PlayerTimedBreakableResponse : MonoBehaviour, ITimedBreakableResponse
    {
        private HashSet<TimedBreakable> _dogoingBreakables = new();

        /*
        private void Start()
        {
            PlayerCore.StateMachine.OnPlayerDeath += StopBreaking;
            PlayerCore.StateMachine.StateTransition += OnPlayerStateChanged;
        }

        private void OnDisable()
        {
            PlayerCore.StateMachine.OnPlayerDeath -= StopBreaking;
            PlayerCore.StateMachine.StateTransition -= OnPlayerStateChanged;
        }

        private void OnPlayerStateChanged()
        {
            if (PlayerCore.StateMachine.IsOnState<PlayerStateMachine.DogoJumping>())
            {
                StopBreaking();
            }
        }

        private void StopBreaking()
        {
            foreach (TimedBreakable b in _dogoingBreakables)
            {
                b.StopBreaking();
            }
            _dogoingBreakables.Clear();
        }*/

        public bool OnBreakableEnter(TimedBreakable b)
        {
            Debug.Log(PlayerCore.StateMachine.CurrState);
            PlayerStateMachine sm = PlayerCore.StateMachine;
            if (sm.IsOnState<PlayerStateMachine.Dogoing>() || sm.IsOnState<PlayerStateMachine.Diving>())
            {
                b.StartBreaking(_dogoingBreakables);
            }
            return true;
        }
    }
}
