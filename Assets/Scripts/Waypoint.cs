using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    [SerializeField] private Vector2 position;
    [SerializeField] private float timeToWait;
    [SerializeField] private float movementSpeed = 5.0f;

    public Vector2 Position { get { return position; } }
    public float TimeToWait { get { return timeToWait; } }
    public float MovementSpeed { get { return movementSpeed; } }
}
