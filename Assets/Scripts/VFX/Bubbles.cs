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

    // Start is called before the first frame update

    // Update is called once per frame
    private void Awake()
    {
        _sr = gameObject.transform.parent.Find("Player Sprite").GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(inWater) {
            if (_bubbleInstance == null)
            {
                _bubbleInstance = Instantiate(bubblePrefab, this.gameObject.transform, false);
            }

            if (_sr.flipX)
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
    
    private void OnTriggerEnter2D(Collider2D other){
        //Debug.Log("IN WATER");
        if(other.gameObject.layer == LayerMask.NameToLayer("Water")){
            inWater = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        //Debug.Log("OUT OF WATER");
        if(other.gameObject.layer == LayerMask.NameToLayer("Water")){
            inWater = false;
        }
    }
}
