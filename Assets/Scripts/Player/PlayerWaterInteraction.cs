using Mechanics;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class PlayerWaterInteraction : MonoBehaviour
{
    public UnityEvent<GameObject> WaterEnter;
    public UnityEvent<GameObject> WaterStay;
    public UnityEvent<GameObject> WaterExit;

    private Collider2D _collider;

    public bool InWater => _inWater;
    private bool _inWater;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        bool prevInWater = _inWater;
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();
        filter.layerMask = LayerMask.GetMask("Water");
        filter.useLayerMask = true;

        var contacts = new List<Collider2D>();
        int collisions = _collider.OverlapCollider(filter, contacts);
        _inWater = collisions > 0;

        if (prevInWater != _inWater)
        {
            if (_inWater)
            {
                WaterEnter?.Invoke(this.gameObject);
            }
            else
            {
                WaterExit?.Invoke(this.gameObject);
            }
        }

        if (_inWater)
        {
            WaterStay?.Invoke(this.gameObject);
        }
    }
}
