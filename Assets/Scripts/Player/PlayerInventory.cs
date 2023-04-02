
using System;
using System.Collections.Generic;

using UnityEngine;

using Collectibles;
using Helpers;
using System.Runtime.CompilerServices;

public class PlayerInventory : Collector, IFilterLoggerTarget
{
    private Dictionary<string, int> itemQuantities = new Dictionary<string, int>();  //id, qty

    public delegate void HandleCollectibleAdd(string id, int number);
    public event HandleCollectibleAdd OnCollectibleAdded;

    public LogLevel GetLogLevel()
    {
        return LogLevel.Error;
    }

    public override void OnCollectFinished(Collectible collectible)
    {
        AddItem(collectible.ID);
    }

    public void AddItem(string id)
    {
        if (!itemQuantities.ContainsKey(id))
        {
            itemQuantities.Add(id, 0);
        }
        itemQuantities[id]++;

        OnCollectibleAdded?.Invoke(id, itemQuantities[id]);
        FilterLogger.Log(this, $"Player Has {itemQuantities[id]} {id}'s");
    }

    public int NumCollectibles(string id)
    {
        if (!itemQuantities.ContainsKey(id))
        {
            return 0;
        }

        return itemQuantities[id];
    }
}
