using MyBox;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using World;

public class PlayerRoomManager : MonoBehaviour
{
    [SerializeField, AutoProperty] private BoxCollider2D _collider;

    private Room _currentRoom;
    private Spawn _currentSpawnPoint;
    public Room CurrentRoom
    {
        get
        {
            if (_currentRoom == null)
            {
                _currentRoom = FindRoomsTouching()[0];
                if (_currentRoom == null)
                {
                    Debug.LogError("Player is out of bounds!");
                }
            }

            return _currentRoom;
        }
    }

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

    public List<Room> FindRoomsTouching()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        List<Collider2D> contacts = new List<Collider2D>();
        _collider.GetContacts(filter, contacts);

        List<Room> roomsTouching = new List<Room>();
        foreach (Collider2D contact in contacts)
        {
            Room room = contact.GetComponent<Room>();
            if (room != null)
            {
                roomsTouching.Add(room);
            }
        }

        return roomsTouching;
    }

    private Spawn FindClosestSpawnPoint()
    {
        float closestDist = float.MaxValue;
        Spawn closest = null;
        Debug.Log($"CurrentRoom: {CurrentRoom}");
        Debug.Log($"Spawns: {CurrentRoom.Spawns}");
        foreach (Spawn spawn in CurrentRoom.Spawns)
        {
            float newDist = Vector2.Distance(transform.position, spawn.transform.position);
            if (newDist < closestDist)
            {
                closestDist = newDist;
                closest = spawn;
            }
        }

        if (closest == null)
        {
            Debug.LogError("No spawn point was found for the player. Every room must have at least one Spawn.");
        }
        return closest;
    }
}