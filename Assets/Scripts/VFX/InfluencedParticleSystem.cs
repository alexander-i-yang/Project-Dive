using System;
using UnityEngine;

public class InfluencedParticleSystem : MonoBehaviour
{
    private ParticleSystem _psystem;
    [SerializeField] private int particleCount;
    [SerializeField] private float influenceWeight;

    private void Awake()
    {
        _psystem = GetComponent<ParticleSystem>();
    }
    
    private void ApplyParticleVelocity(Vector3 influencedV)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_psystem.particleCount+1];
        _psystem.GetParticles(particles);
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].velocity += influencedV * influenceWeight;
            // particles[i].velocity = new Vector3(1000, 1000);
        }
        _psystem.SetParticles(particles, particles.Length);
    }

    public void Emit(Vector3 actorV)
    {
        _psystem.Emit(particleCount);
        ApplyParticleVelocity(actorV);
    }
}