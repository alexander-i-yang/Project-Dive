using Mechanics.MovingBlocks;
using UnityEngine;

namespace Mechanics {
    public class MovingBlockSM : StateMachine<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public MovingBlock M;

        protected override void Init() {
            M = GetComponent<MovingBlock>();
        }

        protected override void SetInitialState() {
            SetState<Idle>();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            CurrState.FixedUpdate();
        }

        public void HitWall() {
            CurrState.HitWall();
        }
    }

    public abstract class MovingBlockState : State<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public abstract void HitWall();
    }

    public class MovingBlockInput : StateInput { }
}