using System;
using DefaultNamespace;
using UnityEngine;

namespace Phys {
    public abstract class Actor : PhysObj {
        [Header("Gravity")]
        public int VelocityDownImmediate;
        public int GravityDown;
        public int GravityUp;
        public int MaxFall;
        
        /// <summary>
        /// Moves this actor a specified number of pixels.
        /// </summary>
        /// <param name="direction"><b>MUST</b> be a cardinal direction with a <b>magnitude of one.</b></param>
        /// <param name="magnitude">Must be <b>non-negative</b> amount of pixels to move.</param>
        /// <param name="OnCollide">Collision function that determines how to behave when colliding with an object</param>
        /// <returns>True if it needs to stop on a collision, false otherwise</returns>
        public bool MoveGeneral(Vector2 direction, int magnitude, Func<PhysObj, bool> OnCollide) {
            if (magnitude < 0) throw new ArgumentException("Magnitude must be >0");

            int remainder = magnitude;
            // If the actor moves at least 1 pixel, Move one pixel at a time
            while (remainder > 0) {
                bool collision = CheckCollisions(direction, OnCollide);
                if (collision) {
                    return true;
                }
                transform.position += new Vector3((int)direction.x, (int)direction.y, 0);
                remainder -= 1;
            }

            return false;
        }

        public override void Move(Vector2 vel) {
            int moveX = (int) Math.Abs(vel.x);
            if (moveX != 0) {
                Vector2 xDir = new Vector2(vel.x/moveX, 0);
                MoveGeneral(xDir, moveX, OnCollide);
            }

            int moveY = (int) Math.Abs(vel.y);
            if (moveY != 0) {
                Vector2 yDir = new Vector2(0, vel.y/moveY);
                MoveGeneral(yDir, moveY, OnCollide);
            }
        }

        public abstract bool OnCollide(PhysObj p);


        public void Fall() {
            velocityY = Math.Max(MaxFall, velocityY + (velocityY > 0 ? GravityUp : GravityDown) * Game.FixedDeltaTime);
            //Stops the stalling problem
            if (velocityY < 50 && velocityY > VelocityDownImmediate) {
                velocityY = VelocityDownImmediate;
            }
        }

        public bool IsGrounded() {
            return CheckCollisions(Vector2.down, e => true);
        }
    }
}