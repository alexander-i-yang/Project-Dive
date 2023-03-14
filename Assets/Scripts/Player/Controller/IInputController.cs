public interface IInputController
{
    //Get -> Returns if the button is pressed
    //_Started -> Returns true the first frame pressed
    //_Finished -> Returns true the first frame released

    public abstract int GetMovementInput();
    public abstract bool MovementStarted();
    public abstract bool MovementFinished();
    public abstract bool RetryStarted();
    public abstract bool GetJumpInput();
    public abstract bool JumpStarted();
    public abstract bool JumpFinished();
    public abstract bool GetDiveInput();
    public abstract bool DiveStarted();
    public abstract bool DiveFinished();
    public abstract bool PausePressed();
}
