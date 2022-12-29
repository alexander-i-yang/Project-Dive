using System;
using System.Collections;
using System.Collections.Generic;

using Core;
using Helpers;
using Mechanics;
using Phys;
using Player;
using World;

using MyBox;

using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerActor : Actor, IPlayerActionHandler, IPlayerInfoProvider {
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private PlayerStateMachine _stateMachine;
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private BoxCollider2D _collider;
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private SpriteRenderer _mySR;
    
    [Foldout("Move", true)]
    [SerializeField] private int moveSpeed;
    [SerializeField] private int maxAcceleration;
    [SerializeField] private int maxAirAcceleration;
    [SerializeField] private int maxDeceleration;
    [Tooltip("Timer between the player crashing into a wall and them getting their speed back (called a cornerboost)")]
    [SerializeField] private float cornerboostTimer;
    [Tooltip("Cornerboost speed multiplier")]
    [SerializeField] private float cornerboostMultiplier;

    [Foldout("Jump", true)]
    [SerializeField] private int jumpHeight;
    [SerializeField] private int crystalJumpHeight;
    [SerializeField] private int doubleJumpHeight;
    [SerializeField] private float jumpCoyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField, Range(0f, 1f)] public float JumpCutMultiplier;

    [Foldout("Dive", true)]
    [SerializeField] private int diveVelocity;
    [SerializeField] private int diveDeceleration;
    
    [Foldout("Dogo", true)]
    [SerializeField] private float dogoJumpHeight;
    [SerializeField] private float dogoJumpXV;
    [SerializeField] private int dogoJumpAcceleration;
    [Tooltip("Time where acceleration/decelartion is 0")]
    [SerializeField] private float dogoJumpTime;
    [SerializeField] private float dogoConserveXVTime;
    [Tooltip("Time to let players input a direction change")]
    [SerializeField] private float dogoJumpGraceTime;

    [Foldout("RoomTransitions", true)]
    [SerializeField, Range(0f, 1f)] private float roomTransitionVCutX = 0.5f;
    [SerializeField, Range(0f, 1f)] private float roomTransitionVCutY = 0.5f;

    #region IPlayerInfoProvider Properties
    public int MaxAcceleration => maxAcceleration;
    public int MaxAirAcceleration => maxAirAcceleration;
    public int MaxDeceleration => maxDeceleration;
    public bool Grounded => IsGrounded();
    public float JumpBufferTime => jumpBufferTime;
    public float JumpCoyoteTime => jumpCoyoteTime;
    public float DogoConserveXVTime => dogoConserveXVTime;
    public float DogoJumpTime => dogoJumpTime;
    public float DogoJumpGraceTime => dogoJumpGraceTime;
    public int DogoJumpAcceleration => dogoJumpAcceleration;
    #endregion

    private bool _hitWallCoroutineRunning;
    private float _hitWallPrevSpeed;

    private void OnEnable()
    {
        Room.RoomTransitionEvent += OnRoomTransition;
    }

    private void OnDisable()
    {
        Room.RoomTransitionEvent -= OnRoomTransition;
    }

    private void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #region Movement
    public void UpdateMovementX(int moveDirection, int acceleration) {
        int targetVelocityX = moveDirection * moveSpeed;
        int maxSpeedChange = (int) (acceleration * Time.deltaTime);
        velocityX = Mathf.MoveTowards(velocityX, targetVelocityX, maxSpeedChange);
    }
    public void Land()
    {
        velocityY = 0;
    }
    #endregion

    #region Jumping
    public void Jump()
    {
        velocityY = GetJumpSpeedFromHeight(jumpHeight);
    }

    public void CrystalJump()
    {
        velocityY = GetJumpSpeedFromHeight(crystalJumpHeight);
    }

    public void DoubleJump(int moveDirection)
    {
        velocityY = GetJumpSpeedFromHeight(doubleJumpHeight);
        // If the player is trying to go in the opposite direction of their x velocity, instantly switch direction.
        if (moveDirection != 0 && moveDirection != Math.Sign(velocityX))
        {
            velocityX = 0;
        }
    }

    public void JumpCut()
    {
        if (velocityY > 0f)
        {
            velocityY *= JumpCutMultiplier;
        }
    }
    #endregion

    #region Dive
    public void Dive()
    {
        velocityY = diveVelocity;
    }

    public void UpdateWhileDiving()
    {
        if (FallVelocityExceedsMax())
        {
            velocityY += diveDeceleration;
        }
        else
        {
            Fall();
        }
    }

    public bool IsDiving()
    {
        return _stateMachine.IsOnState<PlayerStateMachine.Diving>();
    }
    #endregion

    #region Dogo
    public float Dogo() {
        float v = velocityX;
        velocityX = 0;
        return v;
    }

    public void DogoJump(int moveDirection, bool conserveMomentum, double oldXV)
    {
        if (moveDirection != 0)
        {
            velocityX = moveDirection * dogoJumpXV;
            if (conserveMomentum)
            {
                if (moveDirection == 1)
                {
                    velocityX = (float)Math.Max(oldXV + dogoJumpXV, dogoJumpXV);
                }
                else if (moveDirection == -1)
                {
                    velocityX = (float)Math.Min(oldXV - dogoJumpXV, -dogoJumpXV);
                }
            }
        }

        velocityY = GetJumpSpeedFromHeight(dogoJumpHeight);
    }
    #endregion

    public void Die()
    {
        _stateMachine.OnDeath();
        velocity = Vector2.zero;
    }

    #region Actor Overrides
    public override bool Collidable() {
        return true;
    }

    public override bool OnCollide(PhysObj p, Vector2 direction) {
        bool col = p.PlayerCollide(this, direction);
        if (col) {
            if (direction.y > 0) {
                BonkHead();
            }

            if (direction.x != 0) {
                HitWall((int)direction.x);
            }
        }

        return col;
    }

    public override bool PlayerCollide(PlayerActor p, Vector2 direction)
    {
        return false;
        
    }

    public override bool IsGround(PhysObj whosAsking)
    {
        return false;
    }

    public override bool Squish(PhysObj p, Vector2 d)
    {
        if (OnCollide(p, d))
        {
            Debug.Log("Squish " + p);
            Die();
        }
        return false;
    }
    #endregion

    public bool EnterCrystal(Crystal c) {
        return _stateMachine.EnterCrystal(c);
    }
    
    public bool EnterSpike(Spike s) {
        return _stateMachine.EnterSpike(s);
    }

    public void BonkHead() {
        velocityY = Math.Min(10, velocityY);
    }

    private void HitWall(int direction) {
        if (!_hitWallCoroutineRunning) {
            _hitWallPrevSpeed = velocityX;
            velocityX = 0;
            _hitWallCoroutineRunning = true;
            StartCoroutine(HitWallLogic(direction));
        }
    }

    private IEnumerator HitWallLogic(int direction) {
        for (float t = 0; t < cornerboostTimer; t += Game.Instance.FixedDeltaTime) {
            bool movingWithDir = Math.Sign(velocityX) == Math.Sign(direction) || velocityX == 0;
            if (!movingWithDir) {
                break;
            }
            bool stillNextToWall = CheckCollisions(
                Vector2.right * direction, 
                (physObj, d) => {
                return physObj != this && physObj.Collidable();
            });
            if (!stillNextToWall) {
                velocityX = _hitWallPrevSpeed * cornerboostMultiplier;
                break;
            }
            yield return null;
        }
        _hitWallCoroutineRunning = false;
    }

    private void OnRoomTransition(Room roomEntering)
    {
        velocityX *= roomTransitionVCutX;
        velocityY *= roomTransitionVCutY;
    }

    private float GetJumpSpeedFromHeight(float jumpHeight)
    {
        return Mathf.Sqrt(-2f * GravityUp * jumpHeight);
    }

    private void OnDrawGizmosSelected() {
        Handles.Label(new Vector3(0, 56, 0) , $"Velocity: <{velocityX}, {velocityY}>");
    }
}