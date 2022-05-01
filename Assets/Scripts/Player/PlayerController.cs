using System;
using Helpers;
using Mechanics;
using Phys;
using Player;
using UnityEngine;
using UnityEngine.Experimental.Audio;

public class PlayerController : Actor {
    [Header("Controls")]
    public int HSpeed;
    public int JumpV;
    public int DoubleJumpV;
    public int CoyoteTime;
    public int DiveVelocity;
    public int DiveDecel;
    private PlayerStateMachine _mySM;

    public bool ShouldBreak;
    public bool MoveRight;
    protected void Start() {
        _mySM = GetComponent<PlayerStateMachine>();
        _mySM.P = this;
        base.Start();
    }

    void Update()
    {
        _mySM.JumpPressed(Input.GetKeyDown(KeyCode.Z));
        _mySM.DivePressed(Input.GetKeyDown(KeyCode.X));

        if (Input.GetKey(KeyCode.LeftArrow)) {
            velocityX = HSpeed*-1;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            velocityX = HSpeed*1;
        } else {
            velocityX = 0;
        }

        if (MoveRight) velocityX = HSpeed;

        // GetComponent<SpriteRenderer>().color = CheckCollisions(Vector2.down, e => true) ? Color.red : Color.blue;
    }

    void FixedUpdate() {
        _mySM.SetGrounded(IsGrounded());
        base.FixedUpdate();
        if (ShouldBreak) {
            MoveRight = true;
            Debug.Break();
        }
    }


    public void Jump() {
        velocity = new Vector2(velocity.x, JumpV);
    }
    
    public void DoubleJump() {
        velocity = new Vector2(velocity.x, DoubleJumpV);
    }
    
    public override bool OnCollide(PhysObj p, Vector2 direction) {
        return p.PlayerCollide(this, direction);
    }

    public override bool PlayerCollide(PlayerController p, Vector2 direction) {
        return false;
    }

    public override bool IsGround(PhysObj whosAsking) {
        return false;
    }

    public void Land() {
        velocityY = 0;
    }

    public void Dive() {
        velocityY = DiveVelocity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if done diving</returns>
    public bool DiveDecelUpdate() {
        velocityY += DiveDecel;
        return velocityY > MaxFall;
    }

    public void Die() {
        transform.position = new Vector2(0, 60);
        velocity = Vector2.zero;
    }

    public bool EnterCrystal(Crystal c) {
        return _mySM.EnterCrystal(c);
    }

    public void BonkHead() {
        velocityY = Math.Min(100, velocityY);
    }

    public override bool Squish(PhysObj p, Vector2 d) {
        if (OnCollide(p, d)) {
            Die();
        }
        return false;
    }
}
