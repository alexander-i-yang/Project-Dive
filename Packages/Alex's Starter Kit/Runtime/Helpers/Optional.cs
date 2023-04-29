using System;
using UnityEngine;

[Serializable]
/// Requires Unity 2020.1+
public struct Optional<T>
{
    [SerializeField] private bool enabled;
    [SerializeField] private T value;

    public bool Enabled => enabled;
    public T Value => value;

    public Optional(T initialValue)
    {
        enabled = true;
        value = initialValue;
    }

    public Optional(bool enabled)
    {
        this.enabled = enabled;
        this.value = default(T);
    }
}
