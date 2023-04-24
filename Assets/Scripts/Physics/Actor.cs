using Core;

using System;

using MyBox;

using UnityEngine;

namespace Phys {
    public abstract class Actor : PhysObj {
        
        [SerializeField, Foldout("Gravity")] protected int GravityDown;
        [SerializeField, Foldout("Gravity")] protected int GravityUp;
        [SerializeField, Foldout("Gravity")] protected int MaxFall;
        public bool IsMovingUp => velocityY >= 0;

        /// <summary>
        /// Moves this actor a specified number of pixels.
        /// </summary>
        /// <param name="direction"><b>MUST</b> be a cardinal direction with a <b>magnitude of one.</b></param>
        /// <param name="magnitude">Must be <b>non-negative</b> amount of pixels to move.</param>
        /// <param name="onCollide">Collision function that determines how to behave when colliding with an object</param>
        /// <returns>True if it needs to stop on a collision, false otherwise</returns>
        public override bool MoveGeneral(Vector2 direction, int magnitude, Func<PhysObj, Vector2, bool> onCollide) {
            if (magnitude < 0) throw new ArgumentException("Magnitude must be >0");

            int remainder = magnitude;
            // If the actor moves at least 1 pixel, Move one pixel at a time
            while (remainder > 0) {
                bool collision = CheckCollisions(direction, onCollide);
                if (collision) {
                    return true;
                }
                transform.position += new Vector3((int)direction.x, (int)direction.y, 0);
                NextFrameOffset += new Vector2((int)direction.x, (int)direction.y);;
                remainder--;
            }
            
            return false;
        }

        public virtual void Fall() {
            velocityY = Math.Max(MaxFall, velocityY + EffectiveGravity() * Game.Instance.FixedDeltaTime);
        }

        public bool FallVelocityExceedsMax()
        {
            return velocityY < MaxFall;
        }

        protected int EffectiveGravity()
        {
            return (velocityY > 0 ? GravityUp : GravityDown);
        }

        public bool IsRiding(Solid s) {
            return CheckCollisions(Vector2.down, (p, d) => {
                return p == s;
            });
        }
        
        public bool IsGrounded() {
            return CheckCollisions(Vector2.down, (p, d) => {
                return p.IsGround(this);
            });
        }

        public void FlipGravity()
        {
            GravityDown *= -1;
            GravityUp *= -1;
        }

        public void ApplyVelocity(Vector2 v)
        {
            velocity += v;
        }
    }
}