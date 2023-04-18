using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Collectibles;
using Helpers;

public class FireflyUICounter : MonoBehaviour, IFilterLoggerTarget
{
    // private PlayerInventory _inventory;
    private TextMeshProUGUI _tmText;
    private void Start()
    {
        _tmText = GetComponent<TextMeshProUGUI>();
        UpdateCounter(0);
    }

    private void OnEnable()
    {
        Firefly.OnCollectAnimFinish += HandleCollectibleAdded;
    }

    private void OnDisable()
    {
        Firefly.OnCollectAnimFinish -= HandleCollectibleAdded;
    }

    private void HandleCollectibleAdded(int quantity)
    {
        FilterLogger.Log(this, $"Firefly UI Received message of collectibles: {quantity}");
        UpdateCounter(quantity);
        FilterLogger.Log(this, $"Firefly Updated Collectibles Text to {quantity}");
    }

    private void UpdateCounter(int quantity)
    {

        _tmText.text = quantity.ToString();
    }

    public LogLevel GetLogLevel()
    {
        return LogLevel.Error;
    }
}
