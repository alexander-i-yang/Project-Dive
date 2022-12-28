using Core;
using Helpers;
using System;
using System.Collections;

using Mechanics;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class DogoJumping : PlayerState
        {
            private float _dogoJumpTimer;

            public override void Enter(PlayerStateInput i)
            {
                bool conserveMomentum = i.DogoXVBufferTimer > 0;
                MySM.StartCoroutine(DogoJumpRoutine(conserveMomentum, i.OldVelocity));
                foreach (Spike spike in MySM.DogoDisableSpikes)
                {
                    spike.Recharge();
                }
                MySM.Refill();
            }

            private IEnumerator DogoJumpRoutine(bool conserveMomentum, double oldXV)
            {
                _dogoJumpTimer = Player.DogoJumpTime;
                Player.DogoJump(conserveMomentum, oldXV);
                int oldMoveDirection = Player.MoveDirection;
                yield return Helper.DelayAction(Player.DogoJumpGraceTime, () =>
                {
                    if (oldMoveDirection != Player.MoveDirection)
                    {
                        _dogoJumpTimer = Player.DogoJumpTime;
                        Player.DogoJump(conserveMomentum, oldXV);
                    }
                });
            }

            public override void JumpPressed()
            {
                if (MySM._canDoubleJump)
                {
                    Player.DoubleJump();
                    MySM._canDoubleJump = false;
                }
            }

            public override void JumpReleased()
            {
                base.JumpReleased();
                Player.TryJumpCut();
            }

            public override void DivePressed()
            {
                base.DivePressed();
                if (MySM._canDive)
                {
                    MySM.Transition<Diving>();
                }
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (isGrounded)
                {
                    MySM.Transition<Grounded>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                Player.UpdateMovementX(Player.DogoJumpAcceleration);
            }

            public override void FixedUpdate()
            {
                _dogoJumpTimer = Math.Max(0, _dogoJumpTimer - Game.Instance.FixedDeltaTime);
                Player.Fall();
            }
        }
    }
}