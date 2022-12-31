using UnityEngine;

namespace Mechanics {
    /// <summary>
    /// An interface for mechanics that behave differently when you dive into them.
    /// </summary>
    public interface IDiveMechanic {
        /// <summary>
        /// Function to handle behavior when the player dives into an object.
        /// </summary>
        /// <param name="p">Player Action Handler</param>
        /// <returns>true if the player should transition to airborne, false if the player should keep diving</returns>
        public bool OnDiveEnter(IPlayerActionHandler p);
    }
}