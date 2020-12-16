using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private Vector2 directionPlayerFacing;
    private RaycastHit2D target;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        boxCollider = this.GetComponent<BoxCollider2D>();
        directionPlayerFacing = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (movement.y > 0)
        {
            directionPlayerFacing = new Vector2(0, 1);
            animator.SetInteger("Direction", 2);
            animator.SetFloat("Speed", 1.0f);
        }
        else if (movement.y < 0)
        {
            directionPlayerFacing = new Vector2(0, -1);
            animator.SetInteger("Direction", 0);
            animator.SetFloat("Speed", 1.0f);
        }
        else if (movement.x < 0)
        {
            directionPlayerFacing = new Vector2(-1, 0);
            animator.SetInteger("Direction", 1);
            animator.SetFloat("Speed", 1.0f);
        }
        else if (movement.x > 0)
        {
            directionPlayerFacing = new Vector2(1, 0);
            animator.SetInteger("Direction", 3);
            animator.SetFloat("Speed", 1.0f);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }


        Vector2 playerCenter = new Vector2(this.transform.position.x + boxCollider.offset.x, this.transform.position.y + boxCollider.offset.y);
        target = Physics2D.BoxCast(playerCenter, boxCollider.size * 0.8f, 0.0f, directionPlayerFacing, 0.15f, LayerMask.GetMask("Default"));

        if (Input.GetButtonDown("Interact"))
        {
            if (target.transform != null)
            {
                InteractableObject interactableObject = target.transform.gameObject.GetComponent<InteractableObject>();
                if (interactableObject != null)
                {
                    interactableObject.Interact();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Door")
        {
            Door door = collision.gameObject.GetComponent<Door>();
            door.UsedDoor(this.gameObject);
        }
    }
}