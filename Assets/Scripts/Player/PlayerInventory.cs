
using System;
using System.Collections.Generic;

using UnityEngine;

using Collectibles;
using Helpers;

public class PlayerInventory : MonoBehaviour, ICollector, IFilterLoggerTarget
{
    private Dictionary<string, int> itemQuantities = new Dictionary<string, int>();  //id, qty

    public LogLevel GetLogLevel()
    {
        return LogLevel.Info;
    }

    public void OnCollect(Collectible collectible)
    {
        string id = collectible.ID;
        if (!itemQuantities.ContainsKey(id))
        {
            itemQuantities.Add(id, 0);
        }
        itemQuantities[id]++;

        collectible.OnCollected(this);
        FilterLogger.Log(this, $"Player Has {itemQuantities[id]} {id}'s");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Collectible collectible = other.GetComponent<Collectible>();
        if (collectible != null)
        {
            OnCollect(collectible);
        }
    }
}
