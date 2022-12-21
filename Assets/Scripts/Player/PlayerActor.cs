using System;
using System.Collections;
using System.Collections.Generic;

using Helpers;
using Mechanics;
using Phys;
using Player;
using World;

using MyBox;

using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Audio;

[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerActor : Actor {
    [SerializeField, AutoProperty] private PlayerInputController _input;
    [SerializeField, AutoProperty] private PlayerStateMachine _stateMachine;
    [SerializeField, AutoProperty] private BoxCollider2D _collider;

    [Foldout("Move", true)]
    [SerializeField] private int MoveSpeed;
    [SerializeField] private int maxAcceleration;
    [SerializeField] private int maxAirAcceleration;
    [SerializeField] private int maxDeceleration;

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
    [SerializeField] private float DogoXJumpV;
    [SerializeField] private float DogoYJumpHeight;
    [SerializeField] public double DogoConserveXV;
    [SerializeField] public double DogoJumpGrace;

    [Foldout("Misc", true)]
    [SerializeField, Range(0f, 1f)] private float roomTransitionVCutX = 0.5f;
    [SerializeField, Range(0f, 1f)] private float roomTransitionVCutY = 0.5f;
    private int _facing = 1;

    private Room _currentRoom;
    private Spawn _currentSpawnPoint;
    private int _moveDirection;
    private bool _lastJumpBeingHeld;

    public Room CurrentRoom
    {
        get
        {
            if (_currentRoom == null)
            {
                _currentRoom = FindCurrentRoom();
            }

            return _currentRoom;
        }
    }

    public Spawn CurrentSpawnPoint
    {
        get
        {
            if (_currentSpawnPoint == null)
            {
                _currentSpawnPoint = FindClosestSpawnPoint();
            }
            return _currentSpawnPoint;
        }
    }

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
            _stateMachine.CurState.JumpPressed();
        }

        if (_input.JumpFinished())
        {
            _stateMachine.CurState.JumpReleased();
            _lastJumpBeingHeld = false;
        }

        if (_input.DiveStarted())
        {
            _stateMachine.CurState.DivePressed();
        }

        _moveDirection = _input.GetMovementInput();
        if (_moveDirection != 0) _facing = _moveDirection;
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        bool grounded = IsGrounded();
        _stateMachine.CurState.SetGrounded(grounded);
        _stateMachine.CurState.MoveX(grounded);
    }

    public void MoveX(bool grounded) {
        int effectiveAcceleration;
        if (grounded) {
            effectiveAcceleration = _moveDirection == 0 ? maxDeceleration : maxAcceleration;
        } else {
            effectiveAcceleration = maxAirAcceleration;
        }

        int targetVelocityX = _moveDirection * MoveSpeed;
        int maxSpeedChange = (int) (effectiveAcceleration * Time.deltaTime);
        velocityX = Mathf.MoveTowards(velocityX, targetVelocityX, maxSpeedChange);
    }

    public double Dogo() {
        double v = velocityX;
        velocityX = 0;
        return v;
    }

    public override bool OnCollide(PhysObj p, Vector2 direction)
    {
        bool col = p.PlayerCollide(this, direction);
        if (direction.y < 0 && p.IsGround(this))
        {
            //SetGrounded(true);
        }
        else if (direction.y > 0 && col)
        {
            BonkHead();
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
        _stateMachine.CurState.SetGrounded(false);
    }

    public void JumpFromInput() {
        _lastJumpBeingHeld = true;
        Jump();
    }
    
    public void CrystalJump() {
        velocityY = GetJumpSpeedFromHeight(CrystalJumpHeight);
        _stateMachine.CurState.SetGrounded(false);
        _lastJumpBeingHeld = false;
    }

    public void TryJumpCut()
    {
        if (_lastJumpBeingHeld && velocityY > 0f)
        {
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

    public void DogoJump(bool conserveMomentum, double oldXv) {
        StartCoroutine(_dogoJumpContinuous(conserveMomentum, oldXv));
    }

    private void _dogoJumpLogic(bool conserveMomentum, double oldXv) {
        if (_moveDirection != 0) {
            velocityX = _moveDirection * DogoXJumpV;
            if (conserveMomentum) {
                if (_moveDirection == 1) {
                    velocityX = (float)Math.Max(oldXv+DogoXJumpV, DogoXJumpV);
                } else if (_moveDirection == -1) {
                    velocityX = (float)Math.Min(oldXv-DogoXJumpV, -DogoXJumpV);
                }
            }
        }
        /*
        if (_moveDirection == 0) {
            velocityY = GetJumpSpeedFromHeight(DogoYJumpHeight);
            _lastJumpBeingHeld = false;
        }*/
    }
    
    private IEnumerator _dogoJumpContinuous(bool conserveMomentum, double oldXv) {
        int oldFacing = _moveDirection;
        _dogoJumpLogic(conserveMomentum, oldXv);
        yield return new WaitForSeconds((float)(DogoJumpGrace / Game.Instance.TimeScale));
        if (oldFacing != _moveDirection) {
            _dogoJumpLogic(conserveMomentum, oldXv);
        }
        /*if (conserveMomentum) {
            if (_moveDirection == 1) {
                velocityX = (float)Math.Max(oldXv+DogoXJumpV, DogoXJumpV);
            } else if (_moveDirection == -1) {
                velocityX = (float)Math.Min(oldXv-DogoXJumpV, -DogoXJumpV);
            }
        } else {
            velocityX = _moveDirection * DogoXJumpV;
            if (_moveDirection == 0) {
                velocityY = GetJumpSpeedFromHeight(DogoYJumpHeight);
                _lastJumpBeingHeld = false;
            }
        }
        return null*/
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
        if (CurrentRoom != null) {
            transform.position = CurrentSpawnPoint.transform.position;
            velocity = Vector2.zero;
        } else {
            Debug.LogError("Update room prefab");
            transform.position = new Vector2(24, -34);
        }
    }

    public bool EnterCrystal(Crystal c) {
        return _stateMachine.CurState.EnterCrystal(c);
    }
    
    public bool EnterSpike(Spike s) {
        return _stateMachine.CurState.EnterSpike(s);
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
        return _stateMachine.IsOnState<PlayerStateMachine.Diving>();
    }

    private void OnRoomTransition(Room roomEntering)
    {
        _currentRoom = roomEntering;
        _currentSpawnPoint = FindClosestSpawnPoint();
        velocityX *= roomTransitionVCutX;
        velocityY *= roomTransitionVCutY;
    }

    private Room FindCurrentRoom()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        List<Collider2D> contacts = new List<Collider2D>();
        _collider.GetContacts(filter, contacts);
        foreach (Collider2D contact in contacts)
        {
            Room room = contact.GetComponent<Room>();
            if (room != null) {
                return room;
            }
        }

        return null;
    }

    private Spawn FindClosestSpawnPoint()
    {
        float closestDist = float.MaxValue;
        Spawn closest = null;
        Debug.Log($"CurrentRoom: {CurrentRoom}");
        Debug.Log($"Spawns: {CurrentRoom.Spawns}");
        foreach (Spawn spawn in CurrentRoom.Spawns)
        {
            float newDist = Vector2.Distance(transform.position, spawn.transform.position);
            if (newDist < closestDist)
            {
                closestDist = newDist;
                closest = spawn;
            }
        }

        if (closest == null)
        {
            Debug.LogError("No spawn point was found for the player. Every room must have at least one Spawn.");
        }
        return closest;
    }

    private float GetJumpSpeedFromHeight(float jumpHeight)
    {
        return Mathf.Sqrt(-2f * GravityUp * jumpHeight);
    }

    private void OnDrawGizmosSelected() {
        Handles.Label(new Vector3(0, 56, 0) , "" + velocityY);
    }
}