using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController : MonoBehaviour, IInputController
    {
        private PlayerControls controls;
        private PlayerControls.GameplayActions inputActions;

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();
                inputActions = controls.Gameplay;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public int GetMovementInput()
        {

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

        public bool GetJumpInput()
        {
            return inputActions.Jump.IsPressed();
        }

        public bool JumpStarted()
        {
            return inputActions.Jump.WasPressedThisFrame();
        }

        public bool JumpFinished()
        {
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

        private bool MovementChanged()
        {
            bool dirPressed = inputActions.MoveRight.WasPressedThisFrame() || inputActions.MoveLeft.WasPressedThisFrame();
            bool dirReleased = inputActions.MoveRight.WasReleasedThisFrame() || inputActions.MoveLeft.WasReleasedThisFrame();

            return dirPressed || dirReleased;
        }
    }
}