using Core;

using System;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

namespace Player {
    //L: I changed the states to be internal so that the public stuff could be private
    public class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerStateMachine.PlayerState, PlayerStateInput> {
        private PlayerActor _player;

        private float _jumpCoyoteTimer = 0f;
        private float _jumpBufferTimer = 0f;
        private bool _jumpedFromGround;

        private bool _canDoubleJump;
        private bool _canDive;
        private HashSet<Spike> _dogoDisableSpikes;

        protected override void SetInitialState() {
            SetCurState<Grounded>();
        }

        public override void Init() {
            _player = GetComponent<PlayerActor>();
        }

        private void JumpFromInput()
        {
            _player.JumpFromInput();
            _jumpedFromGround = true;
        }

        public override void FixedUpdate() {
            if (_jumpBufferTimer > 0) {
                _jumpBufferTimer = Math.Max(0, _jumpBufferTimer - Game.Instance.FixedDeltaTime);
            }
            base.FixedUpdate();
        }

        public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput>
        {
            //L: Changed from abstract functions to virtual bc not every state needs to implement
            public virtual void JumpPressed()
            {
                MySM._jumpBufferTimer = MySM._player.JumpBufferTime;
            }

            public virtual void JumpReleased() { }
            public virtual void DivePressed() { }
            public virtual void SetGrounded(bool isGrounded) { }
            public virtual bool EnterCrystal(Crystal c) { return false; }
            public virtual void MoveX(bool grounded) {MySM._player.MoveX(grounded);}

            public virtual bool EnterSpike(Spike spike) {
                if (spike.Charged) {
                    MySM._player.Die();
                }
                return false;
            }
        }

        public class Grounded : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                MySM._jumpedFromGround = false;
                MySM.Refill();

                MySM._player.Land();
                if (MySM._jumpBufferTimer > 0 && !MySM.PrevStateEquals<Diving>())
                {
                    MySM.JumpFromInput();
                }
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM.JumpFromInput();
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (!isGrounded) MySM.Transition<Airborne>();
            }
        }

        public class Airborne : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                MySM._jumpCoyoteTimer = MySM._player.JumpCoyoteTime;
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                bool justLeftGround = MySM._jumpCoyoteTimer > 0 && !MySM._jumpedFromGround;
                if (justLeftGround)
                {
                    MySM.JumpFromInput();
                }
                else if (MySM._canDoubleJump)
                {
                    MySM._player.DoubleJump();
                    MySM._canDoubleJump = false;
                }
            }

            public override void JumpReleased()
            {
                base.JumpReleased();
               MySM._player.TryJumpCut();
            }

            public override void DivePressed()
            {
                base.DivePressed();
                if (MySM._canDive)
                {
                    MySM.Transition<Diving>();
                }
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (isGrounded)
                {
                    MySM.Transition<Grounded>();
                }
            }

            public override void FixedUpdate()
            {
                MySM._player.Fall();
                if (MySM._jumpCoyoteTimer > 0) {
                    MySM._jumpCoyoteTimer = Math.Max(0, MySM._jumpCoyoteTimer - Game.Instance.FixedDeltaTime);
                }
            }
        }

        public class Diving : PlayerState
        {
            private bool _diveDecelerationDone = false;
            
            public override void Enter(PlayerStateInput i)
            {
                MySM._player.Dive();
                MySM._canDive = false;
                MySM._dogoDisableSpikes = new HashSet<Spike>();
                _diveDecelerationDone = false;
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                if (MySM._canDoubleJump)
                {
                    MySM._player.DoubleJump();
                    MySM._canDoubleJump = false;
                    MySM.Transition<Airborne>();
                }
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (isGrounded) {
                    MySM.Transition<Dogoing>();
                }
            }

            public override void FixedUpdate()
            {
                if (_diveDecelerationDone)
                {
                    MySM._player.Fall();
                }
                else
                {
                    _diveDecelerationDone = MySM._player.DiveDecelUpdate();
                }
            }

            public override bool EnterCrystal(Crystal c)
            {
                MySM.Transition<Airborne>();
                MySM._player.CrystalJump();
                MySM.Refill();
                c.Break();
                return false;
            }

            public override bool EnterSpike(Spike s) {
                MySM._dogoDisableSpikes.Add(s);
                s.DiveEnter();
                return false;
            }
        }

        public class Dogoing : PlayerState {
            private double _oldVelocity;
            private double _dogoXVBufferTimer;
            public override void Enter(PlayerStateInput i) {
                _oldVelocity = MySM._player.Dogo();
                _dogoXVBufferTimer = MySM._player.DogoConserveXV;
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM._player.DogoJump(_dogoXVBufferTimer > 0, _oldVelocity);
                foreach (Spike spike in MySM._dogoDisableSpikes) {
                    spike.Recharge();
                }
                MySM.Refill();
                MySM.Transition<Airborne>();
            }

            public override void FixedUpdate() {
                if (_dogoXVBufferTimer > 0) {
                    _dogoXVBufferTimer = Math.Max(0, _dogoXVBufferTimer - Game.Instance.FixedDeltaTime);
                }

                base.FixedUpdate();
            }

            public override void MoveX(bool grounded) {
            }
        }

        private void Refill() {
            _canDoubleJump = true;
            _canDive = true;
        }
    }

    public class PlayerStateInput : StateInput {
        public List<Spike> DogoDisabledSpikes;
    }
}