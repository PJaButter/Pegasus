using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    private Rigidbody2D rb;
    private CharacterAnimator characterAnimator;
    private Vector2? targetPosition;
    private Action OnMoveToPositionFinished;

    public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }
    public bool IsPaused { get; private set; }
    public Vector2 Movement { get; set; }
    public Vector2 DirectionFacing { get; set; }

    // Start is called before the first frame update
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        characterAnimator = this.GetComponent<CharacterAnimator>();
        DirectionFacing = new Vector2(0, -1);
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (IsPaused)
            return;

        if (Movement.y > 0)
        {
            DirectionFacing = new Vector2(0, 1);
            characterAnimator.Direction = Direction.Up;
            characterAnimator.Speed = 1.0f;
        }
        else if (Movement.y < 0)
        {
            DirectionFacing = new Vector2(0, -1);
            characterAnimator.Direction = Direction.Down;
            characterAnimator.Speed = 1.0f;
        }
        else if (Movement.x < 0)
        {
            DirectionFacing = new Vector2(-1, 0);
            characterAnimator.Direction = Direction.Left;
            characterAnimator.Speed = 1.0f;
        }
        else if (Movement.x > 0)
        {
            DirectionFacing = new Vector2(1, 0);
            characterAnimator.Direction = Direction.Right;
            characterAnimator.Speed = 1.0f;
        }
        else
        {
            characterAnimator.Speed = 0.0f;
        }
    }

    public void HandleFixedUpdate()
    {
        if (IsPaused)
            return;

        HandleMovement();
    }

    public void MoveToPosition(Vector2 moveAmount, Action OnMoveToPositionFinished = null)
    {
        this.OnMoveToPositionFinished = OnMoveToPositionFinished;
        Vector2 targetPosition = transform.position;
        targetPosition.x += moveAmount.x;
        targetPosition.y += moveAmount.y;
        this.targetPosition = targetPosition;
        Movement = moveAmount.normalized;
    }

    public void Pause()
    {
        IsPaused = true;

        StopMoving();
    }

    public void Resume()
    {
        IsPaused = false;
    }

    public bool IsMoving()
    {
        return Movement != Vector2.zero;
    }

    private void HandleMovement()
    {
        if (targetPosition.HasValue && ((targetPosition.Value - rb.position).sqrMagnitude <= Mathf.Epsilon))
        {
            targetPosition = null;
            Movement = Vector2.zero;
            OnMoveToPositionFinished?.Invoke();
            OnMoveToPositionFinished = null;
        }

        Vector2 newPosition;
        if (targetPosition.HasValue)
        {
            newPosition = Vector3.MoveTowards(rb.position, targetPosition.Value, movementSpeed * Time.fixedDeltaTime);
        }
        else
            newPosition = rb.position + Movement * movementSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void StopMoving()
    {
        Movement = Vector2.zero;
        characterAnimator.Speed = 0.0f;
    }
}
