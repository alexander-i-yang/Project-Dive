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
        { PlayerAnimations.IDLE, "player_idle"},
        { PlayerAnimations.RUNNING, "player_running" },
        { PlayerAnimations.JUMPING, "player_jumping" },
        { PlayerAnimations.DIVING, "player_diving" },
        { PlayerAnimations.DOGOING, "player_dogoing" }
    };
}
