using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPlayerInfoProvider
{
    //Movement
    public int MaxAcceleration { get; }
    public int MaxAirAcceleration { get; }
    public int MaxDeceleration { get; }

    //Jumping
    public bool Grounded { get; }
    public bool IsMovingUp { get; }
    public float JumpBufferTime { get; }
    public float JumpCoyoteTime { get; }

    //Dogo
    public float DogoConserveXVTime { get; }
    public float DogoJumpTime { get; }
    public float DogoJumpGraceTime { get; }

    public int DogoJumpAcceleration { get; }


}
