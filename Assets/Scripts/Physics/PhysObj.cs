using Core;

using System;
using System.Collections.Generic;
using Helpers;
using Mechanics;
using UnityEngine;

namespace Phys {
    [RequireComponent(typeof(Collider2D))]
    public abstract class PhysObj : MonoBehaviour {
        protected BoxCollider2D myCollider { get; private set; }
        public Vector2 velocity { get; protected set; }  = Vector2.zero;

        [NonSerialized] public Vector2 NextFrameOffset = Vector2.zero;
        [NonSerialized] private Vector2 MoveRemainder = Vector2.zero;

        public float velocityY {
            get { return velocity.y; }
            protected set { velocity = new Vector2(velocity.x, value); }
        }

        public float velocityX {
            get { return velocity.x; }
            protected set { velocity = new Vector2(value, velocity.y); }
        }

        protected virtual void Start() {
            myCollider = GetComponent<BoxCollider2D>();
            Game.Instance.ResetNextFrameOffset += ResetNextFrameOffset;
        }

        public virtual void FixedUpdate() {
            Move(velocity * Game.Instance.FixedDeltaTime);
        }

        private void ResetNextFrameOffset() {
            NextFrameOffset = Vector2.zero;
        }

        public bool CheckCollisionsAll(Func<PhysObj, Vector2, bool> onCollide)
        {
            //L: lmao
            if (CheckCollisions(Vector2.right, onCollide))
            {
                return true;
            }

            if (CheckCollisions(Vector2.down, onCollide))
            {
                return true;
            }

            if (CheckCollisions(Vector2.left, onCollide))
            {
                return true;
            }

            if (CheckCollisions(Vector2.up, onCollide))
            {
                return true;
            }

            return false;
        }

        public bool IsOverlapping(PhysObj p)
        {
            return CheckCollisions(Vector2.zero, (checkCol, dir) =>
            {
                return p == checkCol;
            });
        }

        /// <summary>
        /// Checks the interactable layer for any collisions. Will call onCollide if it hits anything.
        /// </summary>
        /// <param name="direction"><b>MUST</b> be a cardinal direction with a <b>magnitude of one.</b></param>
        /// <param name="onCollide">(<b>physObj</b> collided with, <b>Vector2</b> direction),
        /// returns true if should stop</param>
        /// <returns></returns>
        public bool CheckCollisions(Vector2 direction, Func<PhysObj, Vector2, bool> onCollide) {
            Vector2 colliderSize = myCollider.size;
            Vector2 sizeMult = colliderSize - Vector2.one;
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = LayerMask.GetMask("Interactable", "Ground");
            filter.useLayerMask = true;
            Physics2D.BoxCast(transform.position, sizeMult, 0, direction, filter, hits, 8f);

            List<Solid> solids = new();
            List<Actor> actors = new();

            foreach (var hit in hits) {
                if (hit.transform == transform)
                {
                    continue;
                }
                var s = hit.transform.GetComponent<Solid>();
                var a = hit.transform.GetComponent<Actor>();
                if (s != null)
                {
                    solids.Add(s);
                } else if (a != null)
                {
                    actors.Add(a);
                }
            }
            
            foreach (var s in solids)
            {
                bool proactiveCollision = ProactiveBoxCast(
                    s.transform, 
                    s.NextFrameOffset,
                    sizeMult,
                    1,
                    direction, 
                    filter
                );
                if (proactiveCollision)
                {
                    bool col = onCollide.Invoke(s, direction);
                    if (col)
                    {
                        return true;
                    }
                }
            }
            
            foreach (var a in actors)
            {
                bool proactiveCollision = ProactiveBoxCast(
                    a.transform, 
                    a.NextFrameOffset,
                    sizeMult,
                    1,
                    direction, 
                    filter
                );
                if (proactiveCollision)
                {
                    bool col = onCollide.Invoke(a, direction);
                    if (col)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public bool ProactiveBoxCast(Transform checkAgainst, Vector3 nextFrameOffset, Vector2 sizeMult, float dist, Vector2 direction, ContactFilter2D filter) {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            int numHits = Physics2D.BoxCast(
                transform.position - nextFrameOffset,
                size: sizeMult, 
                angle: 0, 
                direction: direction, 
                distance: dist,
                results: hits, 
                contactFilter: filter
            );
            foreach (var hit in hits) {
                if (hit.transform == checkAgainst) {
                    return true;
                }
            }
            return false;
        }
        
        private void OnDrawGizmosSelected() {
            Vector2 direction = velocity == Vector2.zero ? Vector2.up: velocity.normalized;
            var col = GetComponent<BoxCollider2D>();
            if (col == null) return;
            Vector2 colliderSize = col.size;
            Vector2 sizeMult = colliderSize - Vector2.one;
            // Vector2 sizeMult = colliderSize;
            BoxDrawer.DrawBoxCast2D(
                origin: (Vector2) transform.position,
                size: sizeMult,
                direction: direction,
                distance: 1,
                angle: 0,
                color: Color.blue
            );
        }

        public void Move(Vector2 vel) {
            vel += MoveRemainder;
            int moveX = (int) Math.Abs(vel.x);
            if (moveX != 0) {
                Vector2 xDir = new Vector2(vel.x / moveX, 0);
                MoveGeneral(xDir, moveX, OnCollide);
            }

            int moveY = (int) Math.Abs(vel.y);
            if (moveY != 0) {
                Vector2 yDir = new Vector2(0, vel.y / moveY);
                MoveGeneral(yDir, moveY, OnCollide);
            }

            Vector2 truncVel = new Vector2((int) vel.x, (int) vel.y);
            MoveRemainder = vel - truncVel;
        }
        public abstract bool MoveGeneral(Vector2 direction, int magnitude, Func<PhysObj, Vector2, bool> onCollide);
        
        public abstract bool Collidable();
        public virtual bool OnCollide(PhysObj p, Vector2 direction) {
            return Collidable();
        }

        public virtual bool PlayerCollide(PlayerActor p, Vector2 direction) {
            return OnCollide(p, direction);
        }

        public virtual bool IsGround(PhysObj whosAsking) {
            return Collidable();
        }

        public static Actor[] GetActors() {
            return FindObjectsOfType<Actor>();
        }

        public abstract bool Squish(PhysObj p, Vector2 d);
        
        public int ColliderBottomY()
        {
            return Convert.ToInt16(transform.position.y + myCollider.offset.y - myCollider.bounds.extents.y);
        }
        
        public int ColliderTopY()
        {
            return Convert.ToInt16(transform.position.y + myCollider.offset.y + myCollider.bounds.extents.y);
        }
    }
}