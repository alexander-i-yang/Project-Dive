using Core;

using System;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerStateMachine.PlayerState, PlayerStateInput> {
        public PlayerActor Player { get; private set; }
        public float JumpBufferTimer { get; set; }

        private bool _jumpedFromGround;
        private bool _canDoubleJump;
        private bool _canDive;

        public HashSet<Spike> DogoDisableSpikes { get; private set; }

        protected override void SetInitialState() {
            SetState<Grounded>();
        }

        protected override void Init() {
            Player = GetComponent<PlayerActor>();
        }

        protected override void FixedUpdate() {
            if (JumpBufferTimer > 0)
            {
                JumpBufferTimer = Math.Max(0, JumpBufferTimer - Game.Instance.FixedDeltaTime);
            }
            base.FixedUpdate();
        }

        public void Refill()
        {
            _canDoubleJump = true;
            _canDive = true;
        }

        public void JumpFromInput()
        {
            Player.JumpFromInput();
            _jumpedFromGround = true;
        }
    }
}