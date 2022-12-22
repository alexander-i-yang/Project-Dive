using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(10, 10, 1));
    }
}
