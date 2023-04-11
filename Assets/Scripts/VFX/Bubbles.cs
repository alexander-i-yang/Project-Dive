using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Player;
using UnityEngine;

public class Bubbles : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    private GameObject _bubbleInstance;

    private PlayerWaterInteraction _waterCheck;

    private void Awake()
    {
        _waterCheck = GetComponent<PlayerWaterInteraction>();
    }

    void Update()
    {
        if(_waterCheck.InWater) {
            if (_bubbleInstance == null)
            {
                _bubbleInstance = Instantiate(bubblePrefab, this.gameObject.transform, false);
            }

            if (PlayerCore.Actor.Facing == -1)
            {
                transform.localPosition = new Vector3(-4, 4, 0);
            }
            else
            {
                transform.localPosition = new Vector3(4, 4, 0);
            }
        } else
        {
            if (_bubbleInstance != null)
            {
                Destroy(_bubbleInstance);
                _bubbleInstance = null;
            }
        }
    }
}
