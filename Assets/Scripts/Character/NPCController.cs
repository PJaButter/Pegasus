using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private List<Waypoint> waypoints;

    private NPCState previousState;
    private NPCState currentState;
    private float idleTimer = 0.0f;
    private int currentWaypoint;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (currentState == NPCState.Idle)
        {
            if (waypoints.Count > 0)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > waypoints[currentWaypoint].TimeToWait)
                {
                    idleTimer = 0.0f;
                    SetState(NPCState.Walking);
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

    public void Interact(Transform initiator)
    {
        SetState(NPCState.Dialogue);
        character.LookTowards(initiator.position);
        character.PauseMovement();
        StartCoroutine(DialogueManager.Get.ShowDialogue(dialogue, FinishedDialogue));
    }

    private void ReachedWaypoint()
    {
        SetState(NPCState.Idle);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
    }

    private void FinishedDialogue()
    {
        character.ResumeMovement();
        SetState(previousState);
        idleTimer = 0.0f;
    }

    private void SetState(NPCState newState)
    {
        previousState = currentState;
        currentState = newState;
    }
}

public enum NPCState { Idle, Walking, Dialogue }
