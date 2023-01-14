using Phys;

namespace Mechanics
{
    public class BreakableDecoration : Breakable
    {
        protected override string ParticlePath() => "PS_BreakableSign";
        public override bool Collidable()
        {
            return false;
        }

        public override bool IsGround(PhysObj whosAsking)
        {
            return false;
        }
    }
}