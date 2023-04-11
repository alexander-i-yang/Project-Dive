using System;
using System.Collections;

using Helpers;
using MyBox;
using VFX;

using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public partial class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerStateMachine.PlayerState, PlayerStateInput> {
        private PlayerAnimationStateManager _playerAnim;
        private SpriteRenderer _spriteR;
        public event Action OnPlayerRespawn;

        //Expose to inspector
        public UnityEvent<PlayerStateMachine> OnPlayerStateChange;

        public bool UsingDrill => IsOnState<Diving>() || IsOnState<Dogoing>();
        public bool DrillingIntoGround => IsOnState<Dogoing>();

        private PlayerScreenShakeActivator _screenshakeActivator;

        #region Overrides
        protected override void SetInitialState() 
        {
            SetState<Grounded>();
            _playerAnim.Play(PlayerAnimations.SLEEPING);
        }

        protected override void Init()
        {
            _playerAnim = GetComponentInChildren<PlayerAnimationStateManager>();
            _spriteR = GetComponentInChildren<SpriteRenderer>();
            _screenshakeActivator = GetComponent<PlayerScreenShakeActivator>();
            //_drillEmitter = GetComponentInChildren<StudioEventEmitter>();

            OnPlayerRespawn += () =>
            {
                _spriteR.SetAlpha(1);
                PlayerCore.SpawnManager.Respawn();
                Transition<Airborne>();
            };
        }

        protected void OnEnable()
        {
            StateTransition += InvokeUnityStateChangeEvent;
        }

        protected void OnDisable()
        {
            StateTransition -= InvokeUnityStateChangeEvent;
        }

        private void InvokeUnityStateChangeEvent()
        {
            OnPlayerStateChange?.Invoke(this);
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

            if (PlayerCore.Input.RetryStarted())
            {
                PlayerCore.Actor.Die(PlayerCore.Actor.transform.position);
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
            // _spriteR.SetAlpha(0);
            // CurrInput.diePos = diePos;
            Transition<Dead>();
        }

        public void Respawn()
        {
            OnPlayerRespawn?.Invoke();
        }
    }
}