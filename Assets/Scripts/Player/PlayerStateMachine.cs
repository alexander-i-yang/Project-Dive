using System;

namespace Player {
    public class PlayerStateMachine : StateMachine<PlayerStateMachine, PlayerState, PlayerStateInput> {
        [NonSerialized] public double CoyoteTime = 0;
        [NonSerialized] public PlayerController P;
        [NonSerialized] public bool JumpedFromGround;
        
        protected override void SetInitialState() {
            SetCurState<Grounded>();
        }

        public void JumpPressed(bool z) {
            CurState.SetZPressed(z);
        }

        public void SetGrounded(bool isGrounded) {
            CurState.SetGrounded(isGrounded);
        }

        private void FixedUpdate() {
            CurState.FixedUpdate();
        }
    }

    public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput> {
        public abstract void SetZPressed(bool z);

        public abstract void SetGrounded(bool isGrounded);
        public virtual void FixedUpdate() { }
    }

    public class PlayerStateInput : StateInput {
    }
}