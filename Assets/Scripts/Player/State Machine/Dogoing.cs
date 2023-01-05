﻿using Helpers;
using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Dogoing : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                PlayerAnim.ChangeState(PlayerAnimations.DOGOING);
                i.oldVelocity = PlayerActions.Dogo();
                i.ultraTimer = GameTimerWindowed.StartNewWindowedTimer(
                    PlayerCore.UltraTimeDelay, 
                    PlayerCore.UltraTimeWindow
                );
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM.Transition<DogoJumping>();
            }

            public override void FixedUpdate()
            {
                GameTimer.FixedUpdate(Input.ultraTimer);

                base.FixedUpdate();
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
            }

            public override void Update()
            {
                base.Update();
                if (PlayerCore.UltraHelper)
                {
                    if (GameTimerWindowed.GetTimerState(Input.ultraTimer) == TimerStateWindowed.InWindow)
                    {
                        MySM._spriteR.color = Color.green;
                    }
                    else
                    {
                        MySM._spriteR.color = Color.white;
                    }
                }
            }
            
            public override void Exit(PlayerStateInput playerStateInput)
            {
                base.Exit(playerStateInput);
                if (PlayerCore.UltraHelper)
                {
                    MySM._spriteR.color = Color.white;
                }
            }
        }
    }
}