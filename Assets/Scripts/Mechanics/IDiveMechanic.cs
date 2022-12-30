using UnityEngine;

namespace Mechanics {
    /// <summary>
    /// An interface for mechanics that behave differently when you dive into them.
    /// </summary>
    public interface IDiveMechanic {
        public void OnDiveEnter(PlayerActor p);
    }
}