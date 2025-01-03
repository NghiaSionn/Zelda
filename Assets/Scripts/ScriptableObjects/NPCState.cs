using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NPCState : ScriptableObject
{
    public Vector2 position;
    public int currentWaypointIndex;
    public bool isActive;
    public bool isGoingHome;
}
