using Helpers.Animation;

using System.Collections.Generic;

namespace Player
{
    public enum PlayerAnimations
    {
        IDLE,
        RUNNING,
        JUMP_INIT,
        FREEFALL,
        LANDING,
        DIVING,
        DOGOING,
        SLEEPING,
    }

    public class PlayerAnimationStateManager : AnimationStateManager<PlayerAnimations>
    {
        public override Dictionary<PlayerAnimations, string> Animations => new()
        {
            { PlayerAnimations.IDLE, "Player_Idle"},
            { PlayerAnimations.SLEEPING, "Player_Sleeping"},
            { PlayerAnimations.RUNNING, "Player_Running" },
            { PlayerAnimations.JUMP_INIT, "Player_Jump_Init" },
            { PlayerAnimations.FREEFALL, "Player_Freefall" },
            { PlayerAnimations.LANDING, "Player_Landing" },
            { PlayerAnimations.DIVING, "Player_Diving" },
            { PlayerAnimations.DOGOING, "Player_Dogoing" },
        };
    }
}