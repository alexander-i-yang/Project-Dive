using DefaultNamespace;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Player {
    public class Grounded : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.JumpedFromGround = false;
            MySM.P.Land();
        }

        public override void SetZPressed(bool z) {
            if (z) {
                MySM.P.Jump();
                MySM.JumpedFromGround = true;
            }
        }

        public override void SetGrounded(bool isGrounded) {
            if (!isGrounded) MySM.Transition<Airborne>();
        }
    }

    public class Airborne : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.CoyoteTime = MySM.P.CoyoteTime;
        }

        public override void Update(PlayerStateInput i) {
            if (MySM.CoyoteTime > 0) {
                MySM.CoyoteTime--;
            }
        }

        public override void SetZPressed(bool z) {
            if (z) {
                //Just left the ground
                if (MySM.CoyoteTime > 0 && !MySM.JumpedFromGround) {
                    MySM.P.Jump();
                } else {
                    //Otherwise, double jump
                    MySM.Transition<DoubleJumping>();
                }
            }
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            MySM.P.Fall();
        }
    }

    public class DoubleJumping : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.P.DoubleJump();
        }

        public override void SetZPressed(bool z) {
            
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            MySM.P.Fall();
        }
    }
}