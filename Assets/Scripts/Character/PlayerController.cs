using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    public event Action<WildMonster> OnEncountered;

    private BoxCollider2D boxCollider;
    private RaycastHit2D target;
    private Character character;

    // Start is called before the first frame update
    void Start()
    {
        character = this.GetComponent<Character>();
        boxCollider = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (character.IsPaused)
            return;

        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        character.Movement = movement.normalized;

        character.HandleUpdate();

        Vector2 playerCenter = new Vector2(this.transform.position.x + boxCollider.offset.x, this.transform.position.y + boxCollider.offset.y);
        target = Physics2D.BoxCast(playerCenter, boxCollider.size * 0.8f, 0.0f, character.DirectionFacing, 0.15f, LayerMask.GetMask("Interactable"));

        if (Input.GetButtonDown("Interact"))
        {
            if (target.transform != null)
            {
                Interactable interactable = target.transform.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact(transform);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (character.IsPaused)
            return;

        character.HandleFixedUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Door")
        {
            Door door = collision.gameObject.GetComponent<Door>();
            door.UsedDoor(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            WildMonster wildMonster = collision.gameObject.GetComponent<WildMonster>();
            OnEncountered(wildMonster);
        }
    }

    public void Pause()
    {
        character.Pause();
    }

    public void Resume()
    {
        character.Resume();
    }
}