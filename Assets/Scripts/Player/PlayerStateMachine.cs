using System;
using Mechanics;
using UnityEngine;

namespace Player {
    public class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerState, PlayerStateInput> {
        [NonSerialized] public double jumpCoyoteTimer = 0;
        [NonSerialized] public double jumpBufferTimer = 0;
        [NonSerialized] public PlayerActor pActor;
        [NonSerialized] public bool JumpedFromGround;

        [NonSerialized] public bool DoubleJumpLeft;
        [NonSerialized] public bool DiveLeft;
        
        protected override void SetInitialState() {
            SetCurState<Grounded>();
        }

        public override void Init() {
            pActor = GetComponent<PlayerActor>();
        }

        public void JumpPressed(bool pressed) {
            CurState.SetJumpPressed(pressed);
            if (pressed) jumpBufferTimer = pActor.JumpBufferTime;
        }
        
        public void DivePressed(bool pressed) {
            CurState.SetDivePressed(pressed);
        }

        public void SetGrounded(bool isGrounded) {
            CurState.SetGrounded(isGrounded);
        }

        public override void FixedUpdate() {
            if (jumpBufferTimer > 0) {
                jumpBufferTimer = Math.Max(0, jumpBufferTimer - Game.FixedDeltaTime);
            }
            base.FixedUpdate();
        }

        public bool EnterCrystal(Crystal c) {
            return CurState.EnterCrystal(c);
        }
    }

    public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput> {
        public abstract void SetJumpPressed(bool pressed);
        public abstract void SetDivePressed(bool pressed);

        public abstract void SetGrounded(bool isGrounded);
        public abstract bool EnterCrystal(Crystal c);
    }

    public class PlayerStateInput : StateInput {
    }
}