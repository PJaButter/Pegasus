using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private List<Waypoint> waypoints;

    private NPCState state;
    private float idleTimer = 0.0f;
    private int currentWaypoint;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (DialogueManager.Get.IsShowing)
            return;

        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > waypoints[currentWaypoint].TimeToWait)
            {
                idleTimer = 0.0f;
                if (waypoints.Count > 0)
                {
                    state = NPCState.Walking;
                    character.MovementSpeed = waypoints[currentWaypoint].MovementSpeed;
                    character.MoveToPosition(waypoints[currentWaypoint].Position, ReachedWaypoint);
                }
            }
        }

        character.HandleUpdate();
    }

    private void FixedUpdate()
    {
        character.HandleFixedUpdate();
    }

    public void Interact()
    {
        StartCoroutine(DialogueManager.Get.ShowDialogue(dialogue));
    }

    private void ReachedWaypoint()
    {
        state = NPCState.Idle;
        currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
    }
}

public enum NPCState { Idle, Walking }
