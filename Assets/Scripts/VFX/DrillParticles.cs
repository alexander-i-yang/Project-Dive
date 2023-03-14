using Player;
using UnityEngine;

public class DrillParticles : MonoBehaviour
{
    private GameObject psObject;
    private GameObject psObjectInstance;

    public PlayerStateMachine SM => PlayerCore.StateMachine;

    private void Awake()
    {
        psObject = (GameObject) Resources.Load("PS_Drilling");
    }

    private void Update()
    {
        if (SM.DrillingIntoGround)
        {
            if (psObjectInstance == null)
            {
                psObjectInstance = Instantiate(psObject, this.transform);
            }
            UpdateDogoParticleFacing();
        } else
        {
            if (psObjectInstance != null)
            {
                Destroy(psObjectInstance);
                psObjectInstance = null;
            }
        }
    }

    public void UpdateDogoParticleFacing()
    {
        int facing = PlayerCore.Actor.Facing;
        if (facing != 0 && psObjectInstance != null)
        {
            Vector3 scale = psObjectInstance.transform.localScale;
            psObjectInstance.transform.localScale = new Vector3(facing, scale.y, scale.z);
        }
    }
}
