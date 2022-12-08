using System;
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
        
        protected override void SetInitialState() {
            SetCurState<Grounded>();
        }

        public override void Init() {
            _player = GetComponent<PlayerActor>();
        }

        private void JumpFromGround()
        {
            _player.Jump();
            _jumpedFromGround = true;
        }

        public override void FixedUpdate() {
            if (_jumpBufferTimer > 0) {
                _jumpBufferTimer = Math.Max(0, _jumpBufferTimer - Game.FixedDeltaTime);
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
        }

        public class Grounded : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                MySM._jumpedFromGround = false;
                MySM._canDoubleJump = true;
                MySM._canDive = true;

                MySM._player.Land();
                if (MySM._jumpBufferTimer > 0 && !MySM.PrevStateEquals<Diving>())
                {
                    MySM.JumpFromGround();
                }
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM.JumpFromGround();
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
                    MySM.JumpFromGround();
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
                if (MySM._jumpCoyoteTimer > 0)
                {
                    MySM._jumpCoyoteTimer = Math.Max(0, MySM._jumpCoyoteTimer - Game.FixedDeltaTime);
                }
            }
        }

        public class Diving : PlayerState
        {
            private bool _diveDone = false;

            public override void Enter(PlayerStateInput i)
            {
                MySM._player.Dive();
                MySM._canDive = false;
                _diveDone = false;
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
                if (isGrounded)
                {
                    MySM.Transition<Grounded>();
                }
            }

            public override void FixedUpdate()
            {
                if (_diveDone)
                {
                    MySM._player.Fall();
                }
                else
                {
                    _diveDone = MySM._player.DiveDecelUpdate();
                }
            }

            public override bool EnterCrystal(Crystal c)
            {
                MySM.Transition<Airborne>();
                MySM._player.Jump();
                MySM._canDoubleJump = true;
                MySM._canDive = true;
                c.Break();
                return false;
            }
        }
    }

    public class PlayerStateInput : StateInput {
    }
}