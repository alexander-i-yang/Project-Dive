using System;
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
    private PlayerStateMachine _mySM;
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

        // GetComponent<SpriteRenderer>().color = CheckCollisions(Vector2.down, e => true) ? Color.red : Color.blue;
    }

    void FixedUpdate() {
        _mySM.SetGrounded(IsGrounded());
        base.FixedUpdate();
    }


    public void Jump() {
        velocity = new Vector2(velocity.x, JumpV);
    }
    
    public void DoubleJump() {
        print("Double Jump");
        velocity = new Vector2(velocity.x, DoubleJumpV);
    }


    public override bool OnCollide(PhysObj p) {
        return true;
    }

    public void Land() {
        velocityY = 0;
    }

    public void Dive() {
        print("Dive");
        velocityY = DiveVelocity;
    }
}
