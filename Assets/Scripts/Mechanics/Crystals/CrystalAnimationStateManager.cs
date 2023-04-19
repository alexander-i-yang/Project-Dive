using System.Collections.Generic;
using Helpers.Animation;

namespace Mechanics
{
    public enum CrystalAnimations
    {
        IDLE,
        BOUNCE,
        INFLATE
    }

    public class CrystalAnimationStateManager : AnimationStateManager<CrystalAnimations>
    {
        public override Dictionary<CrystalAnimations, string> Animations => new Dictionary<CrystalAnimations, string>()
        {
            { CrystalAnimations.IDLE, "Crystal_Idle"},
            { CrystalAnimations.BOUNCE, "Crystal_Bounce"},
            { CrystalAnimations.INFLATE, "Crystal_Inflate" }
        };
    }
}