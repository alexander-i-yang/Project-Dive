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

        public void FixedUpdate() {
            if (CurState == null) {
                Debug.LogError("Curstate null");
                SetInitialState();
            }

            CurState.FixedUpdate();
        }

        public void HitWall() {
            CurState.HitWall();
        }
    }

    public abstract class MovingBlockState : State<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        public abstract void FixedUpdate();
        public abstract void HitWall();
    }

    public class MovingBlockInput : StateInput { }
}