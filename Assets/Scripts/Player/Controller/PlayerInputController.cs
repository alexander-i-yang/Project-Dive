using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : InputController
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

    public override int GetMovementInput()
    {
        int rightInput = inputActions.MoveRight.IsPressed() ? 1 : 0;
        int leftInput = inputActions.MoveLeft.IsPressed() ? 1 : 0;

        return rightInput - leftInput;
    }

    public override bool MovementStarted()
    {
        bool bothDirsDifferent = inputActions.MoveRight.IsPressed() ^ inputActions.MoveLeft.IsPressed();
        return MovementChanged() && bothDirsDifferent;
    }

    public override bool MovementFinished()
    {
        //If left and right are held at the same time, the player will not move.
        bool bothDirsSame = inputActions.MoveRight.IsPressed() == inputActions.MoveLeft.IsPressed();
        return MovementChanged() && bothDirsSame;
    }

    public override bool GetJumpInput()
    {
        return inputActions.Jump.IsPressed();
    }

    public override bool JumpStarted()
    {
        return inputActions.Jump.WasPressedThisFrame();
    }

    public override bool JumpFinished()
    {
        return inputActions.Jump.WasReleasedThisFrame();
    }

    public override bool GetDiveInput()
    {
        return inputActions.Dive.IsPressed();
    }

    public override bool DiveStarted()
    {
        return inputActions.Dive.WasPressedThisFrame();
    }

    public override bool DiveFinished()
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
