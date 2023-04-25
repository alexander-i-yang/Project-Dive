using System;
using System.Collections;
using Cinemachine;
using Core;
using Helpers;
using Phys;
using Player;
using World;

using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using VFX;

[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerActor : Actor, IFilterLoggerTarget {
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private PlayerStateMachine _stateMachine;
    [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private BoxCollider2D _collider;
    [SerializeField] private SpriteRenderer _sprite;

    private bool _hitWallCoroutineRunning;
    private float _hitWallPrevSpeed;

    public int Facing => _sprite.flipX ? -1 : 1;    //-1 is facing left, 1 is facing right

    [Foldout("Movement Events", true)]
    public PlayerEvent OnJumpFromGround;
    public PlayerEvent OnDoubleJump;
    public UnityEvent OnDiveStart;
    public PlayerEvent OnDogo;
    public PlayerEvent OnLand;
    
    private Func<Vector2, Vector2> _deathRecoilFunc;

    private float _divePosY;

    private void OnEnable()
    {
        Room.RoomTransitionEvent += OnRoomTransition;
        EndCutsceneManager.EndCutsceneEvent += OnEndCutscene;
    }

    private void OnDisable()
    {
        Room.RoomTransitionEvent -= OnRoomTransition;
        EndCutsceneManager.EndCutsceneEvent -= OnEndCutscene;
    }

    #region Movement
    public void UpdateMovementX(int moveDirection, int acceleration) {
        int targetVelocityX = moveDirection * PlayerCore.MoveSpeed;
        int maxSpeedChange = (int) (acceleration * Game.Instance.FixedDeltaTime);
        velocityX = Mathf.MoveTowards(velocityX, targetVelocityX, maxSpeedChange);
    }

    public void Land()
    {
        OnLand?.Invoke(transform.position + Vector3.down * 5.5f);
        velocityY = 0;
        _beegVelocityInd = 0;
    }
    #endregion

    #region Jumping

    /// <summary>
    /// Function that bounces the player.
    /// </summary>
    /// <param name="jumpHeight"></param>
    public void Bounce(int jumpHeight)
    {
        velocityY = GetJumpSpeedFromHeight(jumpHeight);
    }

    [Foldout("Beeg Velocity Data")]
    [SerializeField] private int[] beegVelocities = {200, 250, 350, 1200};
    [SerializeField] private int[] beegDiveHeights = {200, 250, 350, 1200};
    private int _beegVelocityInd = 0;
    public int BeegVInd => _beegVelocityInd;
    public bool IsBeegBouncing => _beegVelocityInd != 0;

    private int CalcBeegVInd(float divePosY)
    {
        int ret = 0;
        for (int i = 0; i < beegDiveHeights.Length; ++i)
        {
            int startHeight = -3000;
            if (i >= 1) startHeight = beegDiveHeights[i - 1];
            int endHeight = beegDiveHeights[i];

            if (divePosY >= startHeight && divePosY <= endHeight)
            {
                ret = i;
            }
        }
        return ret;
    }

    public void BeegBounce(int jumpHeight)
    {
        if (!EndCutsceneManager.IsEndCutscene)
        {
            _beegVelocityInd = CalcBeegVInd(_divePosY);
            velocityY = beegVelocities[_beegVelocityInd];
        }
    }

    public bool ShouldEndCutscene()
    {
        return _beegVelocityInd + 1 == beegVelocities.Length;
    }

    public void SmolBounce(int jumpHeight)
    {
        _beegVelocityInd = 0;
        velocityY = -0.25f * velocityY + GetJumpSpeedFromHeight(jumpHeight);
    }

    private void OnEndCutscene()
    {
        StartCoroutine(Helper.DelayAction(0.25f, () =>
        {
            transform.position = new Vector3(720, 2656, 0);
            velocityY = 0;
        }));
    }

    public void DoubleJump(int jumpHeight, int moveDirection = 0)
    {
        Bounce(jumpHeight);

        // If the player is trying to go in the opposite direction of their x velocity, instantly switch direction.
        if (moveDirection != 0 && moveDirection != Math.Sign(velocityX))
        {
            velocityX = 0;
        }

        OnDoubleJump?.Invoke(transform.position);
    }

    public void JumpFromGround(int jumpHeight)
    {
        Bounce(jumpHeight);

        OnJumpFromGround?.Invoke(transform.position);
    }

    public void JumpCut()
    {
        if (velocityY > 0f)
        {
            velocityY *= PlayerCore.JumpCutMultiplier;
        }
    }
    #endregion

    #region Dive
    public void Dive()
    {
        /*if (EndCutsceneManager.IsBeegBouncing)
        {
            velocityY = Mathf.Min(PlayerCore.DiveVelocity, velocityY);
        }
        else
        {
            velocityY = PlayerCore.DiveVelocity;
        }*/
        velocityY = PlayerCore.DiveVelocity;

        _divePosY = transform.position.y;
        OnDiveStart?.Invoke();
    }

    public void UpdateWhileDiving()
    {
        /*if (EndCutsceneManager.IsBeegBouncing)
        {
            velocityY += GravityDown * Game.Instance.FixedDeltaTime;
        }*/
        
        float oldYV = velocityY;
        if (FallVelocityExceedsMax())
        {
            velocityY += PlayerCore.DiveDeceleration;
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
        OnDogo?.Invoke(transform.position);
        float v = velocityX;
        velocityX = 0;
        return v;
    }

    public void DogoJump(int moveDirection, bool conserveMomentum, double oldXV) {
        if (!(moveDirection == 1 || moveDirection == -1)) {
            throw new ArgumentException(
            $"Cannot dogo jump in direction({moveDirection})"
            );
        }
        velocityX = moveDirection * PlayerCore.DogoJumpXV;
        if (conserveMomentum)
        {
            float addSpeed = PlayerCore.DogoJumpXV * PlayerCore.UltraSpeedMult;
            if (moveDirection == 1)
            {
                velocityX = (float)Math.Max(oldXV + addSpeed, PlayerCore.DogoJumpXV);
            }
            else if (moveDirection == -1)
            {
                velocityX = (float)Math.Min(oldXV - addSpeed, -PlayerCore.DogoJumpXV);
            }
        }

        velocityY = GetJumpSpeedFromHeight(PlayerCore.DogoJumpHeight);
    }
    
    public bool IsDogoJumping()
    {
        return _stateMachine.IsOnState<PlayerStateMachine.DogoJumping>();
    }
    
    public bool IsDogoing()
    {
        return _stateMachine.IsOnState<PlayerStateMachine.Dogoing>();
    }

    public void BallBounce(Vector2 direction)
    {
        if (direction.x != 0)
        {
            velocityX = Math.Sign(direction.x) * -150;
        }
    }
    #endregion

    #region  Death

    public bool IsDrilling() {
        return _stateMachine.UsingDrill;
    }

    public void Die(Func<Vector2, Vector2> recoilFunc = null)
    {
        if (recoilFunc == null) recoilFunc = v => v;
        _deathRecoilFunc = recoilFunc;
        _stateMachine.OnDeath();
        // Game.Instance.ScreenShakeManagerInstance.Screenshake(
        //     PlayerCore.SpawnManager.CurrentRoom.GetComponentInChildren<CinemachineVirtualCamera>(),
        //     10,
        //     1
        //     );
        PlayerCore.MyScreenShakeActivator.ScreenShakeBurst(
            PlayerCore.MyScreenShakeActivator.DeathData
        );
    }

    public void DeathRecoil()
    {
        velocity = _deathRecoilFunc(velocity);
    }

    public void DeadStop()
    {
        velocity = Vector2.zero;
    }

    #endregion

    #region Actor Overrides
    public override bool Collidable() {
        return true;
    }

    public override bool OnCollide(PhysObj p, Vector2 direction) {
        if (p != this)
        {
            FilterLogger.Log(this, $"Player collided with object {p} from direction {direction}");
        }
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

    public void BonkHead() {
        velocityY = Math.Min(10, velocityY);
    }
    
    public void FloorDisappear()
    {
        if (IsDogoing())
        {
            velocity = Vector2.zero;
            _stateMachine.Transition<PlayerStateMachine.Airborne>();    
        }
    }

    private void HitWall(int direction) {
        if (!_hitWallCoroutineRunning) {
            _hitWallPrevSpeed = velocityX;
            velocityX = 0;
            _hitWallCoroutineRunning = true;
            StartCoroutine(HitWallLogic(direction));
        }
    }

    //Todo: change to fixedUpdate GameTimer
    private IEnumerator HitWallLogic(int direction) {
        for (float t = 0; t < PlayerCore.CornerboostTimer; t += Game.Instance.FixedDeltaTime) {
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
                velocityX = _hitWallPrevSpeed * PlayerCore.CornerboostMultiplier;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        _hitWallCoroutineRunning = false;
    }

    private void OnRoomTransition(Room roomEntering)
    {
        // if (EndCutsceneManager.IsBeegBouncing) return;
        
        velocityX *= PlayerCore.RoomTransitionVCutX;
        velocityY *= PlayerCore.RoomTransistionVCutY;
    }

    private float GetJumpSpeedFromHeight(float jumpHeight)
    {
        return Mathf.Sqrt(-2f * GravityUp * jumpHeight);
    }
    
    public void BeegBounceStart()
    {
        // GravityDown = 100;
        // PlayerCore.DiveDeceleration = 0;
        PlayerCore.DiveDeceleration = 0;
        MaxFall = -10000000;
        // foreach (Room room in RoomList.Rooms)
        // {
        //     room.VCam.gameObject.SetActive(false);
        // }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.Label(transform.position, $"Velocity: <{velocityX}, {velocityY}>");
        foreach (var h in beegDiveHeights)
        {
            Handles.DrawLine(new Vector3(600, h), new Vector3(700, h));
        }
    }
    #endif

    public LogLevel GetLogLevel()
    {
        return LogLevel.Error;
    }
    
    public override void Fall() {
        /*if (EndCutsceneManager.IsBeegBouncing)
        {
            velocityY += EffectiveGravity() * Game.Instance.FixedDeltaTime;
        }
        else
        {
            
        }*/
        base.Fall();
    }
}