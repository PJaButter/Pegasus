using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamerController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Dialogue dialogueAfterBattle;
    [SerializeField] private GameObject fov;
    [SerializeField] private GameObject exclaimation;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private string tamerName;

    private Character character;

    private bool battleLost = false;

    public Sprite BattleSprite { get { return battleSprite; } }
    public string Name { get { return tamerName; } }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFOVRotation(character.CharacterAnimator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    private void FixedUpdate()
    {
        character.HandleFixedUpdate();
    }

    public void SetFOVRotation(Direction direction)
    {
        float angle = 0.0f;
        switch (direction)
        {
            case Direction.Right:
                {
                    angle = 90.0f;
                    break;
                }
            case Direction.Up:
                {
                    angle = 180.0f;
                    break;
                }
            case Direction.Left:
                {
                    angle = 270.0f;
                    break;
                }
        }

        fov.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
    }

    public IEnumerator TriggerBattle(PlayerController player)
    {
        // Show Exclaimation
        exclaimation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclaimation.SetActive(false);

        // Walk towards the player
        Vector2 diff = player.transform.position - transform.position;
        Vector2 moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));
        character.MoveToPosition(moveVector, MovedToPlayer);
    }

    public void Interact(Transform initiator)
    {
        if (!battleLost)
        {
            character.LookTowards(initiator.position);

            StartCoroutine(DialogueManager.Get.ShowDialogue(dialogue, () => { GameManager.Get.StartTamerBattle(this); }));
        }
        else
        {
            StartCoroutine(DialogueManager.Get.ShowDialogue(dialogueAfterBattle));
        }
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    private void MovedToPlayer()
    {
        StartCoroutine(DialogueManager.Get.ShowDialogue(dialogue, () => { GameManager.Get.StartTamerBattle(this); }));
    }
}
