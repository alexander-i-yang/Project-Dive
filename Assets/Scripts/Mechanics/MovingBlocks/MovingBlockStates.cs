namespace Mechanics.MovingBlocks {
    public class Idle : MovingBlockState {
        public override void Enter(MovingBlockInput i) {
            MySM.M.Stop();
        }

        public override void FixedUpdate() {
            
        }
        
        public override void Zoom() {
            MySM.Transition<Zooming>();
        }
    }
    
    public class Zooming : MovingBlockState {
        public override void Enter(MovingBlockInput i) {
            MySM.M.Zoom();
        }

        public override void FixedUpdate() {
            if (MySM.M.PositionedAtEnd()) {
                MySM.Transition<Returning>();
            }
        }

        public override void Zoom() {
            MySM.Transition<Zooming>();
        }
    }
    
    public class Returning : MovingBlockState {
        public override void FixedUpdate() {
            MySM.M.Decel();
            if (MySM.M.PositionedAtStart()) {
                MySM.Transition<Idle>();
            }
        }
        
        public override void Zoom() {
            MySM.Transition<Zooming>();
        }
    }
}