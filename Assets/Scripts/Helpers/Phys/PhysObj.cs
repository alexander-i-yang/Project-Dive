using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Phys {
    [RequireComponent(typeof(Collider2D))]
    public abstract class PhysObj : MonoBehaviour {
        protected BoxCollider2D myCollider { get; private set; }
        protected Vector2 velocity = Vector2.zero;

        [NonSerialized] public Vector2 NextFrameOffset = Vector2.zero;
        [NonSerialized] private Vector2 MoveRemainder = Vector2.zero;

        protected float velocityY {
            get { return velocity.y; }
            set { velocity = new Vector2(velocity.x, value); }
        }

        protected float velocityX {
            get { return velocity.x; }
            set { velocity = new Vector2(value, velocity.y); }
        }

        protected void Start() {
            myCollider = GetComponent<BoxCollider2D>();
            Game.ResetNextFrameOffset += ResetNextFrameOffset;
        }

        public virtual void FixedUpdate() {
            Move(velocity * Game.FixedDeltaTime);
        }

        private void ResetNextFrameOffset() {
            NextFrameOffset = Vector2.zero;
        }

        /// <summary>
        /// Checks the interactable layer for any collisions. Will call onCollide if it hits anything.
        /// </summary>
        /// <param name="direction"><b>MUST</b> be a cardinal direction with a <b>magnitude of one.</b></param>
        /// <param name="onCollide"></param>
        /// <returns></returns>
        public bool CheckCollisions(Vector2 direction, Func<PhysObj, Vector2, bool> onCollide) {
            if (myCollider == null) { return true; }

            Vector2 colliderSize = myCollider.size;
            Vector2 sizeMult = colliderSize - Vector2.one;
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = LayerMask.GetMask("Interactable");
            filter.useLayerMask = true;
            Physics2D.BoxCast(transform.position, sizeMult, 0, direction, filter, hits, 8f);

            foreach (var hit in hits) {
                var p = hit.transform.GetComponent<PhysObj>();

                bool proactiveCollision = ProactiveBoxCast(
                    p.transform, 
                    p.NextFrameOffset,
                    sizeMult,
                    1,
                    direction, 
                    filter
                );
                if (proactiveCollision) {
                    if (onCollide.Invoke(p, direction)){
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
            Vector2 direction = velocity == Vector2.zero ? Vector2.up : velocity.normalized;
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

        public abstract bool OnCollide(PhysObj p, Vector2 direction);

        public abstract bool PlayerCollide(PlayerActor p, Vector2 direction);

        public virtual bool IsGround(PhysObj whosAsking) {
            return true;
        }

        public static Actor[] GetActors() {
            return FindObjectsOfType<Actor>();
        }

        public abstract bool Squish(PhysObj p, Vector2 d);
    }
}