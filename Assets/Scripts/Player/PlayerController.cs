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
    private PlayerStateMachine _mySM;
    protected void Start() {
        _mySM = GetComponent<PlayerStateMachine>();
        _mySM.P = this;
        base.Start();
    }

    void Update()
    {
        _mySM.JumpPressed(Input.GetKeyDown(KeyCode.Z));

        if (Input.GetKey(KeyCode.LeftArrow)) {
            velocityX = HSpeed*-1;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            velocityX = HSpeed*1;
        } else {
            velocityX = 0;
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            transform.position = new Vector3(0, 0);
            velocity = Vector2.zero;
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
        velocity = new Vector2(velocity.x, JumpV);
    }


    public override bool OnCollide(PhysObj p) {
        return true;
    }

    public void Land() {
        velocity = new Vector2(velocity.x, 0);
    }
}
