
using System;
using System.Collections.Generic;

using UnityEngine;

using Collectibles;
using Helpers;

public class PlayerInventory : Collector, IFilterLoggerTarget
{
    private Dictionary<string, int> itemQuantities = new Dictionary<string, int>();  //id, qty

    public LogLevel GetLogLevel()
    {
        return LogLevel.Info;
    }

    public override void OnCollectFinished(Collectible collectible)
    {
        string id = collectible.ID;
        if (!itemQuantities.ContainsKey(id))
        {
            itemQuantities.Add(id, 0);
        }
        itemQuantities[id]++;

        FilterLogger.Log(this, $"Player Has {itemQuantities[id]} {id}'s");
    }
}
