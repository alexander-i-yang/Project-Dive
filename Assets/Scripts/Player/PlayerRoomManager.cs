using MyBox;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Helpers;
using World;

public class PlayerRoomManager : MonoBehaviour, IFilterLoggerTarget
{
    private Room _currentRoom;
    private Spawn _currentSpawnPoint;
    public Room CurrentRoom => _currentRoom;

    public Spawn CurrentSpawnPoint
    {
        get
        {
            if (_currentSpawnPoint == null)
            {
                _currentSpawnPoint = FindClosestSpawnPoint();
            }
            return _currentSpawnPoint;
        }
    }

    private void OnEnable()
    {
        Room.RoomTransitionEvent += OnRoomTransition;
    }

    private void OnDisable()
    {
        Room.RoomTransitionEvent -= OnRoomTransition;
    }

    private void OnRoomTransition(Room roomEntering)
    {
        _currentRoom = roomEntering;
        _currentSpawnPoint = FindClosestSpawnPoint();
    }

    private Spawn FindClosestSpawnPoint()
    {
        if (_currentRoom == null)
        {
            FilterLogger.LogWarning(this, "Player is not in a room. Choosing spawn of arbitrary room.");
            _currentRoom = FindObjectOfType<Room>();
        }

        float closestDist = float.MaxValue;
        Spawn closest = null;
        FilterLogger.Log(this, $"CurrentRoom: {CurrentRoom}");
        FilterLogger.Log(this, $"Spawns: {CurrentRoom.Spawns.Length}");
        foreach (Spawn spawn in CurrentRoom.Spawns)
        {
            FilterLogger.Log(this, $"{spawn}");
            float newDist = Vector2.Distance(transform.position, spawn.transform.position);
            if (newDist < closestDist)
            {
                closestDist = newDist;
                closest = spawn;
            }
        }

        if (closest == null)
        {
            FilterLogger.LogError(this, "No spawn point was found for the player. Every room must have at least one Spawn.");
        }
        return closest;
    }

    public LogLevel GetLogLevel()
    {
        return LogLevel.Info;
    }
}