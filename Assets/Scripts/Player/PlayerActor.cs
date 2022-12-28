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
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerActor : Actor {
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private PlayerInputController _input;
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private PlayerStateMachine _stateMachine;
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private BoxCollider2D _collider;
    [SerializeField, AutoProperty(AutoPropertyMode.Children)] private PlayerRoomManager _roomManager;
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private SpriteRenderer _mySR;
    
    [Foldout("Move", true)]
    [SerializeField] private int MoveSpeed;
    [SerializeField] private int maxAcceleration;
    [SerializeField] private int maxAirAcceleration;
    [SerializeField] private int maxDeceleration;
    [FormerlySerializedAs("HitWallTimer")]
    [Tooltip("Timer between the player crashing into a wall and them getting their speed back (called a cornerboost)")]
    [SerializeField] private float CornerboostTimer;
    [FormerlySerializedAs("HitWallMultiplier")]
    [Tooltip("Cornerboost speed multiplier")]
    [SerializeField] private float CornerboostMultiplier;

    [Foldout("Jump", true)]
    [SerializeField] private int JumpHeight;
    [SerializeField] private int CrystalJumpHeight;
    [SerializeField] private int DoubleJumpHeight;
    [SerializeField] public float JumpCoyoteTime;
    [SerializeField] public float JumpBufferTime;
    [SerializeField, Range(0f, 1f)] public float JumpCutMultiplier;

    [Foldout("Dive", true)]
    [SerializeField] private int DiveVelocity;
    [SerializeField] private int DiveDeceleration;
    
    [Foldout("Dogo", true)]
    [SerializeField] private float DogoYJumpHeight;
    [SerializeField] private float DogoXJumpV;
    [SerializeField] private int dogoJumpAcceleration;
    [Tooltip("Time where acceleration/decelartion is 0")]
    [SerializeField] private float dogoJumpTime;
    [SerializeField] public double DogoConserveXVTime;
    [Tooltip("Time to let players input a direction change")]
    [SerializeField] public float DogoJumpGraceTime;

    [Foldout("RoomTransitions", true)]
    [SerializeField, Range(0f, 1f)] private float roomTransitionVCutX = 0.5f;
    [SerializeField, Range(0f, 1f)] private float roomTransitionVCutY = 0.5f;

# region Serialized Fields Exposed through Properties
    public int MaxAcceleration => maxAcceleration;
    public int MaxDeceleration => maxDeceleration;
    public int MaxAirAcceleration => maxAirAcceleration;
    public int DogoJumpAcceleration => dogoJumpAcceleration;
    public float DogoJumpTime => dogoJumpTime;
#endregion

    private int _moveDirection;
    public int MoveDirection => _moveDirection;
    private bool _lastJumpBeingHeld;

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
        UpdateInputs();

        // GetComponent<SpriteRenderer>().color = CheckCollisions(Vector2.down, e => true) ? Color.red : Color.blue;
    }

    private void UpdateInputs()
    {
        if (_input.JumpStarted())
        {
            _stateMachine.CurrState.JumpPressed();
        }

        if (_input.JumpFinished())
        {
            _stateMachine.CurrState.JumpReleased();
            _lastJumpBeingHeld = false;
        }

        if (_input.DiveStarted())
        {
            _stateMachine.CurrState.DivePressed();
        }

        _moveDirection = _input.GetMovementInput();
        if (_moveDirection != 0) {
            _mySR.flipX = _moveDirection == -1;
        }
    }

    private void OnRoomTransition(Room roomEntering)
    {
        velocityX *= roomTransitionVCutX;
        velocityY *= roomTransitionVCutY;
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        bool grounded = IsGrounded();
        _stateMachine.CurrState.SetGrounded(grounded);
        _stateMachine.CurrState.MoveX(_moveDirection);
    }

    public void UpdateMovementX(int acceleration) {
        int targetVelocityX = _moveDirection * MoveSpeed;
        int maxSpeedChange = (int) (acceleration * Time.deltaTime);
        velocityX = Mathf.MoveTowards(velocityX, targetVelocityX, maxSpeedChange);
    }

    public double Dogo() {
        double v = velocityX;
        velocityX = 0;
        return v;
    }
    
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

    public void Jump() {
        velocityY = GetJumpSpeedFromHeight(JumpHeight);
        _stateMachine.CurrState.SetGrounded(false);
    }

    public void JumpFromInput() {
        _lastJumpBeingHeld = true;
        Jump();
    }
    
    public void CrystalJump() {
        velocityY = GetJumpSpeedFromHeight(CrystalJumpHeight);
        _stateMachine.CurrState.SetGrounded(false);
        _lastJumpBeingHeld = false;
    }

    public void TryJumpCut()
    {
        if (_lastJumpBeingHeld && velocityY > 0f) {
            velocityY *= JumpCutMultiplier;
        }

        _lastJumpBeingHeld = false;
    }

    public void DoubleJump() {
        velocityY = GetJumpSpeedFromHeight(DoubleJumpHeight);
        _lastJumpBeingHeld = true;
        // If the player is trying to go in the opposite direction of their x velocity, instantly switch direction.
        if (_moveDirection != 0 && _moveDirection != Math.Sign(velocityX)) {
            velocityX = 0;
        }
    }

    public void DogoJump(bool conserveMomentum, double oldXV) {
        _lastJumpBeingHeld = true;
        if (_moveDirection != 0) {
            velocityX = _moveDirection * DogoXJumpV;
            if (conserveMomentum) {
                if (_moveDirection == 1) {
                    velocityX = (float)Math.Max(oldXV+DogoXJumpV, DogoXJumpV);
                } else if (_moveDirection == -1) {
                    velocityX = (float)Math.Min(oldXV-DogoXJumpV, -DogoXJumpV);
                }
            }
        }

        velocityY = GetJumpSpeedFromHeight(DogoYJumpHeight);
    }

    public void Land() {
        velocityY = 0;
    }

    public void Dive() {
        velocityY = DiveVelocity;
    }

    public void DiveDecelUpdate() {
        velocityY += DiveDeceleration;
    }

    public void Die() {
        if (_roomManager.CurrentSpawnPoint != null) {
            transform.position = _roomManager.CurrentSpawnPoint.transform.position;
            velocity = Vector2.zero;
        } else {
            Debug.LogError("Room Spawn Point Not Found..");
            transform.position = new Vector2(24, -34);
        }
    }

    public bool EnterCrystal(Crystal c) {
        return _stateMachine.CurrState.EnterCrystal(c);
    }
    
    public bool EnterSpike(Spike s) {
        return _stateMachine.CurrState.EnterSpike(s);
    }

    public void BonkHead() {
        velocityY = Math.Min(10, velocityY);
    }

    public void HitWall(int direction) {
        if (!_hitWallCoroutineRunning) {
            _hitWallPrevSpeed = velocityX;
            velocityX = 0;
            _hitWallCoroutineRunning = true;
            StartCoroutine(HitWallLogic(direction));
        }
    }

    public IEnumerator HitWallLogic(int direction) {
        for (float t = 0; t < CornerboostTimer; t += Game.Instance.FixedDeltaTime) {
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
                velocityX = _hitWallPrevSpeed * CornerboostMultiplier;
                break;
            }
            yield return null;
        }
        _hitWallCoroutineRunning = false;
    }

    public override bool Squish(PhysObj p, Vector2 d) {
        if (OnCollide(p, d)) {
            Debug.Log("Squish " + p);
            Die();
        }
        return false;
    }

    public bool IsDiving() {
        return _stateMachine.IsOnState<PlayerStateMachine.Diving>();
    }

    private float GetJumpSpeedFromHeight(float jumpHeight)
    {
        return Mathf.Sqrt(-2f * GravityUp * jumpHeight);
    }

    private void OnDrawGizmosSelected() {
        Handles.Label(new Vector3(0, 56, 0) , $"Velocity: <{velocityX}, {velocityY}>");
    }
}