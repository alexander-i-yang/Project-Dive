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
            private GameTimer _dogoJumpTimer;

            public override void Enter(PlayerStateInput i)
            {
                bool conserveMomentum = GameTimer.GetTimerState(i.dogoXVBufferTimer) == TimerState.Running;
                MySM.StartCoroutine(DogoJumpRoutine(conserveMomentum, i.oldVelocity));
                foreach (Spike spike in Input.dogoDisabledSpikes)
                {
                    spike.Recharge();
                }
                RefreshAbilities();
            }

            private IEnumerator DogoJumpRoutine(bool conserveMomentum, double oldXV)
            {
                Input.canJumpCut = true;
                _dogoJumpTimer = GameTimer.StartNewTimer(PlayerInfo.DogoJumpTime);
                PlayerAction.DogoJump(Input.moveDirection, conserveMomentum, oldXV);

                int oldMoveDirection = Input.moveDirection;
                yield return Helper.DelayAction(PlayerInfo.DogoJumpGraceTime, () =>
                {
                    if (oldMoveDirection != Input.moveDirection)
                    {
                        _dogoJumpTimer = GameTimer.StartNewTimer(PlayerInfo.DogoJumpTime);
                        PlayerAction.DogoJump(Input.moveDirection, conserveMomentum, oldXV);
                    }
                });
            }

            public override void JumpPressed()
            {
                if (Input.canDoubleJump)
                {
                    DoubleJump();
                }
            }

            public override void JumpReleased()
            {
                base.JumpReleased();
                TryJumpCut();
            }

            public override void DivePressed()
            {
                base.DivePressed();
                if (Input.canDive)
                {
                    MySM.Transition<Diving>();
                }
            }

            public override void SetGrounded(bool isGrounded, bool isMovingUp)
            {
                base.SetGrounded(isGrounded, isMovingUp);
                if (isGrounded)
                {
                    MySM.Transition<Grounded>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
                PlayerAction.UpdateMovementX(moveDirection, PlayerInfo.DogoJumpAcceleration);
            }

            public override void FixedUpdate()
            {
                GameTimer.FixedUpdate(_dogoJumpTimer);
                PlayerAction.Fall();
                if (GameTimer.GetTimerState(_dogoJumpTimer) == TimerState.Finished)
                {
                    MySM.Transition<Airborne>();
                }
            }
        }
    }
}