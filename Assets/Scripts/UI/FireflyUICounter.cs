using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Collectibles;
using Helpers;

public class FireflyUICounter : MonoBehaviour, IFilterLoggerTarget
{
    private PlayerInventory _inventory;
    private TextMeshProUGUI _tmText;
    private void Start()
    {
        _inventory = PlayerCore.Actor.GetComponent<PlayerInventory>();
        _inventory.OnCollectibleAdded += HandleCollectibleAdded;
        _tmText = GetComponent<TextMeshProUGUI>();

        UpdateCounter();
    }

    private void OnEnable()
    {
        if (_inventory != null)
        {
            _inventory.OnCollectibleAdded += HandleCollectibleAdded;
        }
    }

    private void OnDisable()
    {
        _inventory.OnCollectibleAdded -= HandleCollectibleAdded;
    }

    private void HandleCollectibleAdded(string id, int quantity)
    {
        FilterLogger.Log(this, $"Firefly UI Received message of collectibles: {quantity}");
        if (id.Equals(Firefly.s_ID))
        {
            FilterLogger.Log(this, $"Firefly Updated Collectibles Text to {quantity}");
            UpdateCounter();
        }
    }

    private void UpdateCounter()
    {

        _tmText.text = _inventory.NumCollectibles("Firefly").ToString();
    }

    public LogLevel GetLogLevel()
    {
        return LogLevel.Info;
    }
}
