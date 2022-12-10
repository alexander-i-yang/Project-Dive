using Mechanics.MovingBlocks;
using UnityEngine;

namespace Mechanics {
    public class MovingBlockSM : StateMachine<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public MovingBlock M;

        public override void Init() {
            M = GetComponent<MovingBlock>();
        }

        protected override void SetInitialState() {
            SetCurState<Idle>();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
            CurState.FixedUpdate();
        }

        public void HitWall() {
            CurState.HitWall();
        }
    }

    public abstract class MovingBlockState : State<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public abstract void HitWall();
    }

    public class MovingBlockInput : StateInput { }
}