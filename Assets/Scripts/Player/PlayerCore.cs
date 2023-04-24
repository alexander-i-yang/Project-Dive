using UnityEngine;

using MyBox;

using Mechanics;
using UnityEngine.Serialization;

namespace Player
{
    //L: The purpose of this class is to ensure that all player components are initialized properly, and it helps keep all of the player properties in one place.
    [RequireComponent(typeof(PlayerActor))]
    [RequireComponent(typeof(PlayerSpawnManager))]
    [RequireComponent(typeof(PlayerStateMachine))]
    [RequireComponent(typeof(PlayerInputController))]
    [RequireComponent(typeof(PlayerScreenShakeActivator))]
    public class PlayerCore : Singleton<PlayerCore>
    {
        #region Player Properties
        [Foldout("Move", true)]
        [SerializeField] private int moveSpeed;
        public static int MoveSpeed => Instance.moveSpeed;

        [SerializeField] private int maxAcceleration;
        public static int MaxAcceleration => Instance.maxAcceleration;  

        [SerializeField] private int maxAirAcceleration;
        public static int MaxAirAcceleration => Instance.maxAirAcceleration;

        [SerializeField] private int maxDeceleration;
        public static int MaxDeceleration =>Instance. maxDeceleration;

        [Tooltip("Timer between the player crashing into a wall and them getting their speed back (called a cornerboost)")]
        [SerializeField] private float cornerboostTimer;
        public static float CornerboostTimer => Instance.cornerboostTimer;

        [Tooltip("Cornerboost speed multiplier")]
        [SerializeField] private float cornerboostMultiplier;
        public static float CornerboostMultiplier => Instance.cornerboostMultiplier;

        [Foldout("Jump", true)]
        [SerializeField] private int jumpHeight;
        public static int JumpHeight => Instance.jumpHeight;

        [SerializeField] private int doubleJumpHeight;
        public static int DoubleJumpHeight => Instance.doubleJumpHeight;

        [SerializeField] private float jumpCoyoteTime;
        public static float JumpCoyoteTime => Instance.jumpCoyoteTime;

        [SerializeField] private float jumpBufferTime;
        public static float JumpBufferTime => Instance.jumpBufferTime;

        [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier;
        public static float JumpCutMultiplier => Instance.jumpCutMultiplier;

        [Foldout("Dive", true)]
        [SerializeField] private int diveVelocity;
        public static int DiveVelocity => Instance.diveVelocity;

        [SerializeField] private int diveDeceleration;
        public static int DiveDeceleration
        {
            get => Instance.diveDeceleration;
            set => Instance.diveDeceleration = value;
        }

        [Foldout("Dogo", true)]
        [SerializeField] private float dogoJumpHeight;
        public static float DogoJumpHeight => Instance.dogoJumpHeight;

        [SerializeField] private float dogoJumpXV;
        public static float DogoJumpXV => Instance.dogoJumpXV;

        [SerializeField] private int dogoJumpAcceleration;
        public static int DogoJumpAcceleration => Instance.dogoJumpAcceleration;

        [Tooltip("Time where acceleration/decelartion is 0")]
        [SerializeField] private float dogoJumpTime;
        public static float DogoJumpTime => Instance.dogoJumpTime;
        
        [Tooltip("Amount of time you need to wait to press jump in order to ultra")]
        [SerializeField] private float ultraTimeDelay;
        public static float UltraTimeDelay => Instance.ultraTimeDelay;
        
        [Tooltip("Window of time you need to press jump in order to ultra")]
        [FormerlySerializedAs("dogoConserveXVTime")]
        [SerializeField] private float ultraTimeWindow;
        public static float UltraTimeWindow => Instance.ultraTimeWindow;
        
        [Tooltip("Speed multiplier on the boost you get from ultraing")]
        [FormerlySerializedAs("dogoConserveXVTime")]
        [SerializeField] private float ultraSpeedMult;
        public static float UltraSpeedMult => Instance.ultraSpeedMult;

        [Tooltip("Debug option to change sprite color to green when u can ultra")]
        [SerializeField] private bool ultraHelper;
        public static bool UltraHelper => Instance.ultraHelper;

        // [SerializeField] private float dogoConserveXVTime;
        // public static float DogoConserveXVTime => Instance.dogoConserveXVTime;

        [Tooltip("Time to let players input a direction change")]
        [SerializeField] private float dogoJumpGraceTime;
        public static float DogoJumpGraceTime => Instance.dogoJumpGraceTime;

        [Foldout("RoomTransitions", true)]
        [SerializeField, Range(0f, 1f)] private float roomTransitionVCutX = 0.5f;
        public static float RoomTransitionVCutX => Instance.roomTransitionVCutX;

        [SerializeField, Range(0f, 1f)] private float roomTransitionVCutY = 0.5f;
        public static float RoomTransistionVCutY => Instance.roomTransitionVCutY;
        
        [SerializeField] private float deathTime;
        public static float DeathTime => Instance.deathTime;

        #endregion

        public static PlayerStateMachine StateMachine { get; private set; }
        public static PlayerInputController Input { get; private set; }
        public static PlayerActor Actor { get; private set; }
        public static PlayerSpawnManager SpawnManager { get; private set; }
        public static PlayerScreenShakeActivator MyScreenShakeActivator { get; private set; }

        private void Awake()
        {
            InitializeSingleton(false); //L: Don't make player persistent, bc then there'll be multiple players OO
            StateMachine = gameObject.GetComponent<PlayerStateMachine>();
            Input = gameObject.GetComponent<PlayerInputController>();
            Actor = gameObject.GetComponent<PlayerActor>();
            SpawnManager = gameObject.GetComponent<PlayerSpawnManager>();
            MyScreenShakeActivator = gameObject.GetComponent<PlayerScreenShakeActivator>();

            //gameObject.AddComponent<PlayerCrystalResponse>();
            //gameObject.AddComponent<PlayerSpikeResponse>();
        }
    }
}