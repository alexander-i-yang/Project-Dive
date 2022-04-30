using System;
using DefaultNamespace;
using Helpers;
using UnityEditor;
using UnityEngine;

namespace Phys {
    [RequireComponent(typeof(BoxCollider2D))]
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
        /// Checks the interactable layer for any collisions.
        /// </summary>
        /// <param name="direction"><b>MUST</b> be a cardinal direction with a <b>magnitude of one.</b></param>
        /// <param name="OnCollide"></param>
        /// <returns></returns>
        public bool CheckCollisions(Vector2 direction, Func<PhysObj, bool> OnCollide) {
            Vector2 colliderSize = myCollider.size;
            Vector2 sizeMult = colliderSize*0.97f;
            RaycastHit2D hit = Physics2D.BoxCast(
                origin: (Vector2) transform.position,
                size: new Vector2(1, 1) * sizeMult,
                direction: direction,
                distance: 0.3f,
                angle: 0,
                layerMask: LayerMask.GetMask("Interactable")
            );
            return hit.collider;
        }

        private void OnDrawGizmosSelected() {
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
        }

        public abstract void Move(Vector2 velocity);
    }
}