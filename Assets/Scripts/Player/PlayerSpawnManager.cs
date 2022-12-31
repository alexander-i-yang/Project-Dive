using System.Collections.Generic;

using UnityEngine;

using Helpers;
using World;

public class PlayerSpawnManager : MonoBehaviour, IFilterLoggerTarget
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

    public void Respawn()
    {
        if (CurrentSpawnPoint != null)
        {
            transform.position = CurrentSpawnPoint.transform.position;
        }
    }

    private Spawn FindClosestSpawnPoint()
    {
        Spawn closest;
        if (_currentRoom == null)
        {
            FilterLogger.LogWarning(this, "Player is not in a room. Choosing arbitrary spawn.");
            Spawn[] allSpawns = FindObjectsOfType<Spawn>();
            closest = FindClosestSpawnPoint(allSpawns);
        } else
        {
            closest = FindClosestSpawnPoint(CurrentRoom.Spawns);
        }

        if (closest == null)
        {
            FilterLogger.LogError(this, "No spawn point was found for the player. There must be at least one spawn point in the scene.");
        }
        return closest;
    }

    private Spawn FindClosestSpawnPoint(IEnumerable<Spawn> spawns)
    {
        float closestDist = float.MaxValue;
        Spawn closest = null;
        FilterLogger.Log(this, $"CurrentRoom: {CurrentRoom}");
        FilterLogger.Log(this, $"Spawns: {CurrentRoom.Spawns.Length}");
        foreach (Spawn spawn in spawns)
        {
            FilterLogger.Log(this, $"{spawn}");
            float newDist = Vector2.Distance(transform.position, spawn.transform.position);
            if (newDist < closestDist)
            {
                closestDist = newDist;
                closest = spawn;
            }
        }

        return closest;
    }

    public LogLevel GetLogLevel()
    {
        return LogLevel.Warning;
    }
}