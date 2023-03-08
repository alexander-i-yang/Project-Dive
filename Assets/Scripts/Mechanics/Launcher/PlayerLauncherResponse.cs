using System;
using MyBox;
using Phys;
using Player;
using UnityEngine;

namespace Mechanics
{
    public class PlayerLauncherResponse : LauncherResponse
    {
        public override void OnLauncherEnter(Launcher l)
        {
            bool isDiving = PlayerCore.Actor.IsDiving();
            if (isDiving)
            {
                l.StartLaunch(this);
            }
            else if (PlayerCore.Actor.velocityY <= 0)
            {
                l.Bounce(this);
            }
        }

        public override void AttemptLaunch(Launcher l, Vector2 v)
        {
            bool isOnLaunch = l.IsOverlapping(_myActor);
            if (isOnLaunch)
            {
                //PlayerCore.StateMachine.CurrInput.facing = Math.Sign(v.x);
                if(PlayerCore.Actor.IsDogoing())
                {
                    PlayerCore.StateMachine.Transition<PlayerStateMachine.DogoJumping>();
                    PlayerCore.Actor.DoubleJump((int)v.y, 0);
                }
                else
                {
                    int facing = Math.Sign(PlayerCore.Actor.Facing);
                    Vector2 chadVelocity = new Vector2(facing*v.x, v.y); 
                    _myActor.ApplyVelocity(3*chadVelocity);
                }
            }
        }

        public override void Bounce(int bounceHeight)
        {
            PlayerCore.Actor.DoubleJump(bounceHeight, 0);
            PlayerCore.StateMachine.CurrInput.RefillAbilities();
            // _myActor.ApplyVelocity(new Vector2(0, bounceHeight));
        }
    }
}