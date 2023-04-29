using ASK.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController : MonoBehaviour, IInputController
    {
        private PlayerControls controls;
        private PlayerControls.GameplayActions inputActions;

        private System.Action PauseAction;

        public bool HasPutInput = false;

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();
                inputActions = controls.Gameplay;
            }

            inputActions.Enable();

            inputActions.Pause.performed += OnPause;
        }

        private void OnDisable()
        {
            inputActions.Pause.performed -= OnPause;
            inputActions.Disable();
        }

        public bool AnyKeyPressed()
        {
            return MovementStarted() || DiveStarted() || JumpStarted();
        }

        public int GetMovementInput()
        {
            if (Game.Instance.FakeControlsArrows != -2)
            {
                return Game.Instance.FakeControlsArrows;
            }
            
            
            int rightInput = inputActions.MoveRight.IsPressed() ? 1 : 0;
            int leftInput = inputActions.MoveLeft.IsPressed() ? 1 : 0;
            return rightInput - leftInput;
        }

        public bool MovementStarted()
        {
            bool bothDirsDifferent = inputActions.MoveRight.IsPressed() ^ inputActions.MoveLeft.IsPressed();
            return MovementChanged() && bothDirsDifferent;
        }

        public bool MovementFinished()
        {
            //If left and right are held at the same time, the player will not move.
            bool bothDirsSame = inputActions.MoveRight.IsPressed() == inputActions.MoveLeft.IsPressed();
            return MovementChanged() && bothDirsSame;
        }
        
        public bool RetryStarted()
        {
            return inputActions.Restart.WasPerformedThisFrame();
        }
        
        public bool GetJumpInput()
        {
            // if (!Game.Instance.FakeControlsZ.Disabled) return Game.Instance.FakeControlsZ.Value;
            return inputActions.Jump.IsPressed();
        }

        public bool JumpStarted()
        {
            // if (!Game.Instance.FakeControlsZ.Disabled) return Game.Instance.FakeControlsZ.WasPressedThisFrame();
            return inputActions.Jump.WasPressedThisFrame();
        }

        public bool JumpFinished()
        {
            // if (!Game.Instance.FakeControlsZ.Disabled) return Game.Instance.FakeControlsZ.WasReleasedThisFrame();
            return inputActions.Jump.WasReleasedThisFrame();
        }

        public bool GetDiveInput()
        {
            return inputActions.Dive.IsPressed();
        }

        public bool DiveStarted()
        {
            return inputActions.Dive.WasPressedThisFrame();
        }

        public bool DiveFinished()
        {
            return inputActions.Dive.WasReleasedThisFrame();
        }

        public void AddToPauseAction(System.Action action)
        {
            PauseAction += action;
        }

        public void RemoveFromPauseAction(System.Action action)
        {
            PauseAction -= action;
        }

        private void OnPause(InputAction.CallbackContext ctx)
        {
            PauseAction?.Invoke();
        }

        public bool PausePressed()
        {
            return inputActions.Pause.WasPressedThisFrame();
        }

        private bool MovementChanged()
        {
            bool dirPressed = inputActions.MoveRight.WasPressedThisFrame() || inputActions.MoveLeft.WasPressedThisFrame();
            bool dirReleased = inputActions.MoveRight.WasReleasedThisFrame() || inputActions.MoveLeft.WasReleasedThisFrame();

            return dirPressed || dirReleased;
        }
    }
}