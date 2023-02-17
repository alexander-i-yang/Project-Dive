using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Dust : MonoBehaviour
{
    private ParticleSystem dust;
    private ParticleSystem.EmissionModule emiss;
    private float emissAmt;
    // Start is called before the first frame update
    void Start()
    {
        dust = this.gameObject.GetComponent<ParticleSystem>();
        emiss = dust.emission;
        emissAmt = emiss.rateOverDistance.constant;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCore.Actor.velocityY < 0)
        {
            emiss.rateOverDistanceMultiplier = 0;
        }
        else
        {
            emiss.rateOverDistanceMultiplier = emissAmt;
        }
    }
    
}
