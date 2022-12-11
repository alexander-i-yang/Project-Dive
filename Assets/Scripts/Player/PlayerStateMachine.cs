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

        private void JumpFromInput()
        {
            _player.JumpFromInput();
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
            public virtual void MoveX(bool grounded) {MySM._player.MoveX(grounded);}
        }

        public class Grounded : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                MySM._jumpedFromGround = false;
                MySM.refill();

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
                if (MySM._jumpCoyoteTimer > 0)
                {
                    MySM._jumpCoyoteTimer = Math.Max(0, MySM._jumpCoyoteTimer - Game.FixedDeltaTime);
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
                if (isGrounded)
                {
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
                MySM._player.Jump();
                MySM.refill();
                c.Break();
                return false;
            }
        }

        public class Dogoing : PlayerState {
            public override void Enter(PlayerStateInput i) {
                MySM._player.StopX();
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                print("Dogo jump");
                MySM._player.DogoJump();
                MySM.Transition<Airborne>();
            }

            public override void MoveX(bool grounded) {
            }
        }

        private void refill() {
            _canDoubleJump = true;
            _canDive = true;
        }
    }

    public class PlayerStateInput : StateInput {
    }
}