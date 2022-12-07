using System;
using Mechanics;
using UnityEngine;

namespace Player {
    public class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerState, PlayerStateInput> {
        [NonSerialized] public PlayerActor pActor;

        [NonSerialized] public float jumpCoyoteTimer = 0f;
        [NonSerialized] public float jumpBufferTimer = 0f;
        [NonSerialized] public bool JumpedFromGround;
        [NonSerialized] public bool JumpBeingHeld;


        [NonSerialized] public bool DoubleJumpLeft;
        [NonSerialized] public bool DiveLeft;
        
        protected override void SetInitialState() {
            SetCurState<Grounded>();
        }

        public override void Init() {
            pActor = GetComponent<PlayerActor>();
        }

        public void JumpPressed() {
            CurState.JumpPressed();
            jumpBufferTimer = pActor.JumpBufferTime;
        }

        public void JumpReleased()
        {
            CurState.JumpReleased();
        }
        
        public void DivePressed() {
            CurState.DivePressed();
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

        public void JumpCut()
        {
            if (JumpBeingHeld)
            {
                JumpBeingHeld = false;
                pActor.JumpCut();
            }
        }

        public bool EnterCrystal(Crystal c)
        {
            CurState.EnterCrystal(c);
            return false;
        }
    }

    public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput> {
        public virtual void JumpPressed() { }
        public virtual void JumpReleased() { }
        public virtual void DivePressed() { }
        public virtual void SetGrounded(bool isGrounded) { }
        public virtual void EnterCrystal(Crystal c) { }
    }

    public class PlayerStateInput : StateInput {
    }
}