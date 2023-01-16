using System;
using System.Collections;
using Core;
using Helpers;
using Phys;
using UnityEngine;

namespace Mechanics
{
    public class Ball : Actor
    {
        protected override void Start()
        {
            base.Start();
            velocityX = 50;
            velocityY = -50;
            Debug.Log(velocity);
        }

        public override bool Collidable()
        {
            return true;
        }

        public override bool OnCollide(PhysObj p, Vector2 direction)
        {
            if (p == this) return false;
            TimedBreakable b = p as TimedBreakable;
            bool c = base.OnCollide(p, direction);
            if (c)
            {
                Debug.Log("Collide");
                SwitchDirection(direction);
            }
            if (b != null)
            {
                b.Break();
            } else if (c && direction.y < 0 && (p as Wall) != null)
            {
                GameObject l = GameObject.Find("Lava");
                l.transform.position += new Vector3(0, 8, 0);
            }
            return c;
        }

        public override bool Squish(PhysObj p, Vector2 d)
        {
            Destroy(gameObject);
            return false;
        }

        void SwitchDirection(Vector2 direction)
        {
            if (direction.y != 0)
            {
                velocityY = Math.Sign(direction.y) * -50f;
                velocityX = Math.Sign(velocityX) * 25f;
            }
            else if (direction.x != 0)
            {
                velocityX = Math.Sign(direction.x) * -50f;
            }
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            SwitchDirection(direction);
            if (direction.x != 0 && Math.Abs(p.velocityX) > 75)
            {
                p.BallBounce(direction);
                velocityY = Math.Max(50, velocityY);
                velocityX = Math.Sign(direction.x) * -75f;
                StartCoroutine(FreezeTime());
            }
            return true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            int targetVelocityX = Math.Sign(velocity.x) * 50;
            int maxSpeedChange = (int) (50 * Game.Instance.FixedDeltaTime);
            velocityX = Mathf.MoveTowards(velocityX, targetVelocityX, maxSpeedChange);
        }

        IEnumerator FreezeTime()
        {
            Game.Instance.TimeScale = 0;
            yield return new WaitForSeconds(0.2f);
            Game.Instance.TimeScale = 1;
        }
    }
}