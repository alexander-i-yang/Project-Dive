using System.Collections.Generic;
using Mechanics;

namespace Player
{
    public class PlayerStateInput : StateInput {
        public List<Spike> DogoDisabledSpikes;

        public double OldVelocity;
        public double DogoXVBufferTimer;
    }
}