using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrphanChild : MonoBehaviour
{
    [Tooltip("Orphan's new parent")]
    [SerializeField] private Transform newParent;

    [Tooltip("Should this transform take the parent the new parent's parent?")]
    [SerializeField] private Boolean parentOfParent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AbandonParent());
    }

    IEnumerator AbandonParent()
    {
        yield return new WaitForSeconds(0.2f);
        if (parentOfParent) {
            transform.parent = newParent.parent;
        } else
        {
            transform.parent = newParent;
        }
    }
}
