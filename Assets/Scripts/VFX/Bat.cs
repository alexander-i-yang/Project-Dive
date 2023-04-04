using System;
using Core;
using Helpers;
using MyBox;
using UnityEngine;

namespace VFX
{
    public class Bat : MonoBehaviour
    {
        [SerializeField, MinMaxRange(0, 500)]
        private RangedInt speedRange = new (100, 150);
        [SerializeField] private float lifeSpan = 10f;
        
        private float _speed;

        private void Awake()
        {
            _speed = speedRange.GetRandom();
            StartCoroutine(
            Helper.DelayAction(lifeSpan, () => Destroy(gameObject))
            );
        }

        private void Update()
        {
            transform.position += new Vector3(_speed*Game.Instance.DeltaTime, 0);
        }
    }
}