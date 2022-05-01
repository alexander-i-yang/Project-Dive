using Mechanics.MovingBlocks;

namespace Mechanics {
    public class MovingBlockSM : StateMachine<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        protected override void SetInitialState() {
            SetCurState<Idle>();
        }
    }

    public abstract class MovingBlockState : State<MovingBlockSM, MovingBlockState, MovingBlockInput> {
        
    }

    public class MovingBlockInput : StateInput { }
}