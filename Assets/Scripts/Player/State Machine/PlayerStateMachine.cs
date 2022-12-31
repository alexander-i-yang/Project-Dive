using System;

using UnityEngine;

using Helpers;

namespace Player
{
    public partial class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerStateMachine.PlayerState, PlayerStateInput> {
        private PlayerAnimationStateManager _playerAnim;
        private SpriteRenderer _spriteR;

        public event Action OnPlayerDeath;

        #region Overrides
        protected override void SetInitialState() 
        {
            SetState<Grounded>();
            _playerAnim.SetInitialState(PlayerAnimations.IDLE);
        }

        protected override void Init()
        {
            _playerAnim = GetComponentInChildren<PlayerAnimationStateManager>();
            _spriteR = GetComponentInChildren<SpriteRenderer>();

        }

        protected override void Update()
        {
            base.Update();

            if (PlayerCore.Input.JumpStarted())
            {
                CurrState.JumpPressed();
            }

            if (PlayerCore.Input.JumpFinished())
            {
                CurrState.JumpReleased();
            }

            if (PlayerCore.Input.DiveStarted())
            {
                CurrState.DivePressed();
            }

            CurrInput.moveDirection = PlayerCore.Input.GetMovementInput();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            GameTimer.FixedUpdate(CurrInput.jumpBufferTimer);
            CurrState.SetGrounded(PlayerCore.Actor.IsGrounded(), PlayerCore.Actor.IsMovingUp);;
            CurrState.MoveX(CurrInput.moveDirection);
        }
        #endregion

        public void RefreshAbilities()
        {
            CurrState.RefreshAbilities();
        }

        public void OnDeath()
        {
            CurrState.OnDeath();
        }
    }
}