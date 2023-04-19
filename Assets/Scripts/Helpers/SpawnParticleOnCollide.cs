using UnityEngine;
using System.Collections.Generic;

public class SpawnParticleOnCollide : MonoBehaviour
{
    private ParticleSystem _part;
    private List<ParticleCollisionEvent> _collisionEvents;

    [SerializeField] private ParticleSystem childParticles;
    [SerializeField] private int numParticles;
    
    void Awake()
    {
        _part = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent>();
        // childParticles.gameObject.SetActive(false);
    }

    void OnParticleCollision(GameObject other)
    {
        _part.GetCollisionEvents(other, _collisionEvents);

        foreach (var colEvent in _collisionEvents)
        {
            Vector3 point = colEvent.intersection;
            childParticles.transform.position = point;
            childParticles.Emit(numParticles);
        }
    }
}