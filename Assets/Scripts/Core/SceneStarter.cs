using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneStarter : MonoBehaviour
{
    [SerializeField] private UnityEvent OnSceneStart;
    
    // Start is called before the first frame update
    void Start()
    {
        OnSceneStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
