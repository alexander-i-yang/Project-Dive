
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

    [SerializeField] private int debugFirefliesCollected = 0;
    
    public LogLevel GetLogLevel()
    {
        return LogLevel.Error;
    }

    public override void OnCollectFinished(Collectible collectible)
    {
        AddItem(collectible.ID);
    }

    public void AddItem(string id, int numtoAdd = 1)
    {
        if (!itemQuantities.ContainsKey(id))
        {
            itemQuantities.Add(id, 0);
        }
        itemQuantities[id] += numtoAdd;

        OnCollectibleAdded?.Invoke(id, itemQuantities[id] + debugFirefliesCollected);
        FilterLogger.Log(this, $"Player Has {itemQuantities[id] + debugFirefliesCollected} {id}'s");
    }

    public int NumCollectibles(string id)
    {
        if (!itemQuantities.ContainsKey(id))
        {
            return 0;
        }
        
        if (id == Firefly.s_ID) return itemQuantities[id] + debugFirefliesCollected;

        return itemQuantities[id];
    }
}
