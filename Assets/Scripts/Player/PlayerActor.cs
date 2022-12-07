using System;
using Helpers;
using Mechanics;
using Phys;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Audio;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerActor : Actor {
    [Header("Controls")]
    public int HSpeed;
    public int JumpV;
    public int DoubleJumpV;
    public double jumpCoyoteTime;
    public double jumpBufferTime;
    public int DiveVelocity;
    public int DiveDecel;

    private PlayerInputController _input;
    private PlayerStateMachine _stateMachine;

    private void Awake()
    {
        _input = GetComponent<PlayerInputController>();
        _stateMachine = GetComponent<PlayerStateMachine>();
    }

    void Update()
    {
        _stateMachine.JumpPressed(_input.JumpStarted());
        _stateMachine.DivePressed(_input.DiveStarted());

        velocityX = HSpeed * _input.GetMovementInput();

        // GetComponent<SpriteRenderer>().color = CheckCollisions(Vector2.down, e => true) ? Color.red : Color.blue;
    }

    public override void FixedUpdate() {
        if (!IsGrounded()) {
            SetGrounded(false);
        }
        base.FixedUpdate();
    }

    public void SetGrounded(bool b) {
        _stateMachine.SetGrounded(b);
    }

    public void Jump() {
        velocityY = JumpV;
        _stateMachine.SetGrounded(false);
    }
    
    public void DoubleJump() {
        velocityY = DoubleJumpV;
    }
    
    public override bool OnCollide(PhysObj p, Vector2 direction) {
        bool col = p.PlayerCollide(this, direction);
        if (direction.y < 0 && p.IsGround(this)) {
            SetGrounded(true);
        } else if (direction.y > 0 && col) {
            BonkHead();
        }

        return col;
    }

    public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
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
        transform.position = new Vector2(-109, 23);
        velocity = Vector2.zero;
    }

    public bool EnterCrystal(Crystal c) {
        return _stateMachine.EnterCrystal(c);
    }

    public void BonkHead() {
        velocityY = Math.Min(10, velocityY);
    }

    public override bool Squish(PhysObj p, Vector2 d) {
        if (OnCollide(p, d)) {
            Debug.Log("Squish " + p);
            Die();
        }
        return false;
    }

    public bool IsDiving() {
        return _stateMachine.IsOnState<Diving>();
    }

    private void OnDrawGizmosSelected() {
        Handles.Label(new Vector3(0, 56, 0) , "" + velocity.y);
    }
}
