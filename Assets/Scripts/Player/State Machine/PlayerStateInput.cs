using System;
using System.Collections.Generic;

using Helpers;
using Mechanics;
using UnityEngine;

namespace Player
{
    public class PlayerStateInput : StateInput {
        //Movement
        public int moveDirection;
        public int facing;

        //Jump
        public GameTimer jumpBufferTimer;
        public bool jumpedFromGround;
        public bool canJumpCut;
        public bool canDoubleJump;

        //Dive
        public bool canDive;

        //Dogo
        // public GameTimer dogoXVBufferTimer;
        public HashSet<Spike> dogoDisabledSpikes;
        public double oldVelocity;
        public GameTimerWindowed ultraTimer;

        public Vector3 diePos;
        
        public void RefillAbilities()
        {
            canDive = true;
            canDoubleJump = true;
        }
    }
}