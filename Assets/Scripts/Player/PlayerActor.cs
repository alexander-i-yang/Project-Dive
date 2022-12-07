using System;
using Helpers;
using Mechanics;
using Phys;
using Player;

using MyBox;

using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Audio;

[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerActor : Actor {

    [Foldout("Move", true)]
    [SerializeField] private int MoveSpeed;
    [SerializeField] private int maxAcceleration;
    [SerializeField] private int maxAirAcceleration;
    [SerializeField] private int maxDeceleration;
    
    [Foldout("Jump", true)]
    [SerializeField] private int JumpV;
    [SerializeField] private int DoubleJumpV;
    [SerializeField] public double JumpCoyoteTime;
    [SerializeField] public double JumpBufferTime;

    [Foldout("Dive", true)]
    [SerializeField] private int DiveVelocity;
    [SerializeField] private int DiveDeceleration;

    private PlayerInputController _input;
    private PlayerStateMachine _stateMachine;

    private int _moveDirection;

    private void Awake()
    {
        _input = GetComponent<PlayerInputController>();
        _stateMachine = GetComponent<PlayerStateMachine>();
    }

    void Update()
    {
        _stateMachine.JumpPressed(_input.JumpStarted());
        _stateMachine.DivePressed(_input.DiveStarted());

        _moveDirection = _input.GetMovementInput();

        // GetComponent<SpriteRenderer>().color = CheckCollisions(Vector2.down, e => true) ? Color.red : Color.blue;
    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        int effectiveAcceleration;
        if (IsGrounded())
        {
            effectiveAcceleration = _moveDirection == 0 ? maxDeceleration : maxAcceleration;
        }
        else
        {
            SetGrounded(false);
            effectiveAcceleration = maxAirAcceleration;
        }

        int targetVelocityX = _moveDirection * MoveSpeed;
        int maxSpeedChange = (int) (effectiveAcceleration * Time.deltaTime);
        velocityX = Mathf.MoveTowards(velocityX, targetVelocityX, maxSpeedChange);
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
        velocityY += DiveDeceleration;
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
