using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorState
    {
        Open,
        Closed,
        Locked
    }

    public enum DoorType
    {
        Horizontal,
        Vertical
    }

    [Header("Door Settings")]
    public DoorState doorState;
    public DoorType doorType;
    public Vector2Int direction;
    public List<Room> connectedRooms = new List<Room>();

    public void Initialize(Room room, Vector2Int direction)
    {
        this.direction = direction;
        doorType = (direction.x != 0) ? DoorType.Vertical : DoorType.Horizontal;

        AddConnected(room);
        UpdateDoorState();
    }

    public void AddConnected(Room room)
    {
        if (!connectedRooms.Contains(room))
        {
            connectedRooms.Add(room);
            room.OnRoomStateChanged += UpdateDoorState;
        }
    }

    public void UpdateDoorState()
    {
        bool anyRoomStart = connectedRooms.Any(r => r.type == RoomType.Start);
        bool allRoomsCleared = connectedRooms.All(r => r.isCleared || !r.hasEnemies);

        if (anyRoomStart)
        {
            SetState(DoorState.Closed);
        }
        else if (allRoomsCleared)
        {
            SetState(DoorState.Open);
        }
    }

    public void SetState(DoorState newState)
    {
        doorState = newState;

        switch (newState)
        {
            case DoorState.Open:
                break;
            case DoorState.Closed:
                break;
            case DoorState.Locked:
                break;
        }
    }

    private void OnDestroy()
    {
        foreach (var room in connectedRooms)
        {
            if (room != null)
            {
                room.OnRoomStateChanged -= UpdateDoorState;
            }
        }
    }
}