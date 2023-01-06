using UnityEngine;

namespace Mechanics
{
    [CreateAssetMenu(fileName = "SpikeSprites", menuName = "ScriptableObjects/SpikeSprites", order = 1)]
    public class SpikeSpritesScriptableObject : ScriptableObject
    {
        [Tooltip("Direction tips are pointing: right, up, left, down")]
        [SerializeField] public Sprite[] Sprites;
    }
}