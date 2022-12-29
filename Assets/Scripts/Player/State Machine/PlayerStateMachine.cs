using Helpers;
using Mechanics;
using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerStateMachine.PlayerState, PlayerStateInput> {
        private IInputController _input;
        private IPlayerInfoProvider _playerInfo;
        private IPlayerActionHandler _playerAction;
        private PlayerSpawnManager _spawnManager;
        private SpriteRenderer _spriteR;

        #region Overrides
        protected override void SetInitialState()
        {
            SetState<Grounded>();
        }

        protected override void Init()
        {
            _input = GetComponent<IInputController>();
            _playerInfo = GetComponent<IPlayerInfoProvider>();
            _playerAction = GetComponent<IPlayerActionHandler>();
            _spawnManager = GetComponent<PlayerSpawnManager>();
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
            CurrState.SetGrounded(_playerInfo.Grounded);
            CurrState.MoveX(CurrInput.moveDirection);
        }

        public void OnDeath()
        {
            CurrState.OnDeath();
        }

        public bool EnterCrystal(Crystal c)
        {
            return CurrState.EnterCrystal(c);
        }

        public bool EnterSpike(Spike s)
        {
            return CurrState.EnterSpike(s);
        }
        #endregion
    }
}