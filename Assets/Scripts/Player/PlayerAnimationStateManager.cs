using Helpers.Animation;
using System;
using System.Collections.Generic;

public enum PlayerAnimations
{
    IDLE,
    RUNNING,
    JUMPING,
    DIVING,
    DOGOING,
}

public class PlayerAnimationStateManager : AnimationStateManager<PlayerAnimations>
{
    public override Dictionary<PlayerAnimations, string> Animations => new Dictionary<PlayerAnimations, string>()
    {
        { PlayerAnimations.IDLE, "Player_Idle"},
        { PlayerAnimations.RUNNING, "Player_Running" },
        { PlayerAnimations.JUMPING, "Player_Jumping" },
        { PlayerAnimations.DIVING, "Player_Diving" },
        { PlayerAnimations.DOGOING, "Player_Dogoing" }
    };
}
