using System.Collections.Generic;

using Helpers;

using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerStateMachine.PlayerState, PlayerStateInput> {
        private IInputController _input;
        private PlayerActor _player;
        private SpriteRenderer _spriteR;

        #region Overrides
        protected override void SetInitialState()
        {
            SetState<Grounded>();
        }

        protected override void Init()
        {
            _input = GetComponent<IInputController>();
            _player = GetComponent<PlayerActor>();
            _spriteR = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            if (_input.JumpStarted())
            {
                CurrState.JumpPressed();
            }

            if (_input.JumpFinished())
            {
                CurrState.JumpReleased();
            }

            if (_input.DiveStarted())
            {
                CurrState.DivePressed();
            }

            CurrInput.moveDirection = _input.GetMovementInput();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            GameTimer.FixedUpdate(CurrInput.jumpBufferTimer);
            CurrState.SetGrounded(_player.IsGrounded());
            CurrState.MoveX(CurrInput.moveDirection);
        }
        #endregion
    }
}