using System;
using UnityEngine;
using Phys;

[Obsolete]
public class BreakableParticle : Actor
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        bool grounded = IsGrounded();
        print(grounded);
        if (!grounded)
        {
            print(velocityY);
            Fall();
        }
    }

    public override bool Collidable()
    {
        return false;
    }

    public override bool IsGround(PhysObj whosAsking)
    {
        return false;
    }

    public override bool Squish(PhysObj p, Vector2 d)
    {
        return false;
    }

    public override bool OnCollide(PhysObj p, Vector2 direction)
    {
        return p.IsGround(this);
    }
}