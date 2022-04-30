using System;

namespace Player {
    public class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerState, PlayerStateInput> {
        [NonSerialized] public double CoyoteTime = 0;
        [NonSerialized] public PlayerController P;
        [NonSerialized] public bool JumpedFromGround;

        [NonSerialized] public bool DoubleJumpLeft;
        [NonSerialized] public bool DiveLeft;
        
        protected override void SetInitialState() {
            SetCurState<Grounded>();
        }

        public void JumpPressed(bool pressed) {
            CurState.SetJumpPressed(pressed);
        }
        
        public void DivePressed(bool pressed) {
            CurState.SetDivePressed(pressed);
        }

        public void SetGrounded(bool isGrounded) {
            CurState.SetGrounded(isGrounded);
        }

        private void FixedUpdate() {
            CurState.FixedUpdate();
        }
    }

    public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput> {
        public abstract void SetJumpPressed(bool pressed);
        public abstract void SetDivePressed(bool pressed);

        public abstract void SetGrounded(bool isGrounded);
        public virtual void FixedUpdate() { }
    }

    public class PlayerStateInput : StateInput {
    }
}