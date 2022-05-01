using System;
using System.Collections.Generic;
using DefaultNamespace;
using Helpers;
using UnityEditor;
using UnityEngine;

namespace Phys {
    [RequireComponent(typeof(Collider2D))]
    public abstract class PhysObj : MonoBehaviour {
        protected BoxCollider2D myCollider { get; private set; }
        protected Vector2 velocity = Vector2.zero;

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
        }

        public void FixedUpdate() {
            Move(velocity*Game.FixedDeltaTime);
        }

        /// <summary>
        /// Checks the interactable layer for any collisions. Will call OnCollide if it hits anything.
        /// </summary>
        /// <param name="direction"><b>MUST</b> be a cardinal direction with a <b>magnitude of one.</b></param>
        /// <param name="onCollide"></param>
        /// <returns></returns>
        public bool CheckCollisions(Vector2 direction, Func<PhysObj, Vector2, bool> onCollide) {
            Vector2 colliderSize = myCollider.size;
            Vector2 sizeMult = colliderSize*0.97f;
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = LayerMask.GetMask("Interactable");
            filter.useLayerMask = true;
            int numHits = Physics2D.BoxCast(transform.position, sizeMult, 0, direction, filter, hits, 0.3f);
            if (numHits != 0) {
                foreach (var hit in hits) {
                    var p = hit.transform.GetComponent<PhysObj>();
                    if (onCollide.Invoke(p, direction)){
                        return true;
                    }
                }

                return false;
            } else {
                return false;
            }
        }

        /*private void OnDrawGizmosSelected() {
            Vector2 direction = Vector2.left;
            Vector2 colliderSize = GetComponent<BoxCollider2D>().size;
            Vector2 sizeMult = colliderSize*0.97f;
            BoxDrawer.DrawBoxCast2D(
                origin: (Vector2) transform.position,
                size: new Vector2(1, 1) * sizeMult,
                direction: direction,
                distance: 0.3f,
                angle: 0,
                color: Color.blue
            );
        }*/

        public abstract void Move(Vector2 velocity);
        public abstract bool OnCollide(PhysObj p, Vector2 direction);
        public abstract bool PlayerCollide(PlayerController p, Vector2 direction);
        
        public virtual bool IsGround(PhysObj whosAsking) {
            return true;
        }
    }
}