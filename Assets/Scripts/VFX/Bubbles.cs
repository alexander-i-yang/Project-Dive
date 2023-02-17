using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Player;
using UnityEngine;

public class Bubbles : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private bool inWater;

    private SpriteRenderer _sr;
    private GameObject _bubbleInstance;
    private BoxCollider2D _collider;

    // Start is called before the first frame update

    // Update is called once per frame
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _sr = gameObject.transform.parent.Find("Player Sprite").GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(inWater) {
            if (_sr.flipX)
            {
                transform.localPosition = new Vector3(-4, 4, 0);
            }
            else
            {
                transform.localPosition = new Vector3(4, 4, 0);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other){
        //Debug.Log("IN WATER");
        if(other.gameObject.layer == LayerMask.NameToLayer("Water") && !inWater){
            inWater = true;
            _bubbleInstance = Instantiate(bubblePrefab, this.gameObject.transform, false);
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        //Debug.Log("OUT OF WATER");
        if(other.gameObject.layer == LayerMask.NameToLayer("Water") && inWater){
            inWater = false;
            if (_bubbleInstance != null) {
                Destroy(_bubbleInstance);
            }
        }
    }
}
