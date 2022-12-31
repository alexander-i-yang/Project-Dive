using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

//Source: Slider by Daniel Carr
[System.Serializable]
public class Music
{
    public string name;

    public EventReference fmodEvent;

    [HideInInspector]
    public StudioEventEmitter emitter;
}