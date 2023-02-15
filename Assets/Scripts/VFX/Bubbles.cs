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
    private GameObject bubbles;
    private BoxCollider2D col;
    [SerializeField]private SpriteRenderer rend;
    // Start is called before the first frame update

    // Update is called once per frame
    private void Awake()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if(inWater) {
            if (rend.flipX)
            {
                gameObject.transform.localPosition = new Vector3(-4, 4, 0);
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(4, 4, 0);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == 4 && !inWater){
            inWater = true;
            bubbles = Instantiate(bubblePrefab, this.gameObject.transform, false);
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.layer == 4 && inWater){
            inWater = false;
            if (bubbles != null) {
                Destroy(bubbles);
            }
        }
    }
}
