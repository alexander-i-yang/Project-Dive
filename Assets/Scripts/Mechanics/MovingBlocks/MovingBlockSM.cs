using System;
using Mechanics.MovingBlocks;

namespace Mechanics {
    public class MovingBlockSM : StateMachine<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public MovingBlock M;

        public override void Init() {
            M = GetComponent<MovingBlock>();
        }

        protected override void SetInitialState() {
            SetCurState<Idle>();
        }

        public void FixedUpdate() {
            CurState.FixedUpdate();
        }

        public void Zoom() {
            CurState.Zoom();
        }
    }

    public abstract class MovingBlockState : State<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public abstract void FixedUpdate();
        public abstract void Zoom();
    }

    public class MovingBlockInput : StateInput { }
}