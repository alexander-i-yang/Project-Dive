using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BubbleSnap : MonoBehaviour
{
    private ParticleSystem p_system;
    private ParticleSystem.Particle[] p_particles;
    [SerializeField] private int precision;
    

    private void Awake()
    {
        intitializeIfNeeded();
    }

    private void FixedUpdate()
    {
        int numParticles = p_system.GetParticles(p_particles);

        for (int i = 0; i < numParticles; i++)
        {
            p_particles[i].position = new Vector3(TruncateValue(p_particles[i].position.x), p_particles[i].position.y, 1);
        }
        p_system.SetParticles(p_particles, numParticles);
    }

    void intitializeIfNeeded()
    {
        if (p_system == null)
            p_system = gameObject.GetComponent<ParticleSystem>();
        if (p_particles == null || p_particles.Length < p_system.main.maxParticles)
            p_particles = new ParticleSystem.Particle[p_system.main.maxParticles];
    }

    float TruncateValue(float val)
    {
        return (int)(val * precision) / precision;
    }
}
