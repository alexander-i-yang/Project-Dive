public interface IPlayerActionHandler
{
    //Movement
    public void UpdateMovementX(int moveDirection, int moveAcceleration);
    public void Land();
    public void Fall();

    //Jumping
    public void Jump();
    public void DoubleJump(int moveDirection);
    public void JumpCut();
    public void MechanicBounce(int bounceHeight);

    //Diving
    public void Dive();
    public void UpdateWhileDiving();

    //Dogoing
    public float Dogo();    //returns velocity before dogo.
    public void DogoJump(int moveDirection, bool conserveMomentum, double oldXV);
    public void Die();
}

