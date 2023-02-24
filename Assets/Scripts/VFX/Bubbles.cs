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
    private Collider2D _collider;

    // Start is called before the first frame update

    // Update is called once per frame
    private void Awake()
    {
        _sr = gameObject.transform.parent.Find("Player Sprite").GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();
        filter.layerMask = LayerMask.GetMask("Water");
        filter.useLayerMask = true;

        var contacts = new List<Collider2D>();
        int collisions = _collider.OverlapCollider(filter, contacts);
        inWater = collisions > 0;

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
    
    //private void OnTriggerEnter2D(Collider2D other){
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Water")){
    //        inWater = true;
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D other){
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Water")){
    //        inWater = false;
    //    }
    //}
}
