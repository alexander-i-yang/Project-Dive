using Core;
using Helpers;
using System;
using System.Collections;

using Mechanics;
using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class DogoJumping : PlayerState
        {
            private GameTimer _dogoJumpTimer;

            public override void Enter(PlayerStateInput i)
            {
                bool conserveMomentum = GameTimerWindowed.GetTimerState(i.ultraTimer) == TimerStateWindowed.InWindow;
                MySM.StartCoroutine(DogoJumpRoutine(conserveMomentum, i.oldVelocity));
                foreach (Spike spike in Input.dogoDisabledSpikes)
                {
                    spike.Recharge();
                }
                RefreshAbilities();
            }

            private int GetDogoJumpDirection() {
                int facing = Actor.Facing;
                int moveDir = Input.moveDirection;
                if (moveDir == 0) moveDir = facing;
                return moveDir;
            }

            private IEnumerator DogoJumpRoutine(bool conserveMomentum, double oldXV)
            {
                Input.canJumpCut = true;
                _dogoJumpTimer = GameTimer.StartNewTimer(PlayerCore.DogoJumpTime);
                int jumpDir = GetDogoJumpDirection();
                Actor.DogoJump(jumpDir, conserveMomentum, oldXV);
                int oldJumpDir = jumpDir;
                
                yield return Helper.DelayAction(PlayerCore.DogoJumpGraceTime, () => {
                    jumpDir = GetDogoJumpDirection();
                    if (jumpDir != oldJumpDir)
                    {
                        _dogoJumpTimer = GameTimer.StartNewTimer(PlayerCore.DogoJumpTime);
                        Actor.DogoJump(jumpDir, conserveMomentum, oldXV);
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
                Actor.UpdateMovementX(moveDirection, PlayerCore.DogoJumpAcceleration);
            }

            public override void FixedUpdate()
            {
                GameTimer.FixedUpdate(_dogoJumpTimer);
                Actor.Fall();
                if (GameTimer.GetTimerState(_dogoJumpTimer) == TimerState.Finished)
                {
                    MySM.Transition<Airborne>();
                }
            }
        }
    }
}