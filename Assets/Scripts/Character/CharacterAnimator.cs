using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> walkUpSprites;
    [SerializeField] private List<Sprite> walkDownSprites;
    [SerializeField] private List<Sprite> walkLeftSprites;
    [SerializeField] private List<Sprite> walkRightSprites;

    [SerializeField] private List<Sprite> idleUpSprites;
    [SerializeField] private List<Sprite> idleDownSprites;
    [SerializeField] private List<Sprite> idleLeftSprites;
    [SerializeField] private List<Sprite> idleRightSprites;

    // Parameters
    public Direction Direction { get; set; }
    public float Speed { get; set; }

    // States
    private SpriteAnimator walkUpAnim;
    private SpriteAnimator walkDownAnim;
    private SpriteAnimator walkLeftAnim;
    private SpriteAnimator walkRightAnim;

    private SpriteAnimator idleUpAnim;
    private SpriteAnimator idleDownAnim;
    private SpriteAnimator idleLeftAnim;
    private SpriteAnimator idleRightAnim;

    private SpriteAnimator currentAnimator;

    // References
    private SpriteRenderer spriteRenderer;

    private bool isPaused;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkUpAnim = new SpriteAnimator(spriteRenderer, walkUpSprites);
        walkDownAnim = new SpriteAnimator(spriteRenderer, walkDownSprites);
        walkLeftAnim = new SpriteAnimator(spriteRenderer, walkLeftSprites);
        walkRightAnim = new SpriteAnimator(spriteRenderer, walkRightSprites);

        idleUpAnim = new SpriteAnimator(spriteRenderer, idleUpSprites);
        idleDownAnim = new SpriteAnimator(spriteRenderer, idleDownSprites);
        idleLeftAnim = new SpriteAnimator(spriteRenderer, idleLeftSprites);
        idleRightAnim = new SpriteAnimator(spriteRenderer, idleRightSprites);

        Direction = Direction.Down;
        SetCurrentAnimator(idleDownAnim);
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    private void Update()
    {
        switch(Direction)
        {
            case Direction.Up:
            {
                if (Speed > 0)
                    SetCurrentAnimator(walkUpAnim);
                else
                    SetCurrentAnimator(idleUpAnim);
                break;
            }
            case Direction.Down:
            {
                if (Speed > 0)
                    SetCurrentAnimator(walkDownAnim);
                else
                    SetCurrentAnimator(idleDownAnim);
                break;
            }
            case Direction.Left:
            {
                if (Speed > 0)
                    SetCurrentAnimator(walkLeftAnim);
                else
                    SetCurrentAnimator(idleLeftAnim);
                break;
            }
            case Direction.Right:
            {
                if (Speed > 0)
                    SetCurrentAnimator(walkRightAnim);
                else
                    SetCurrentAnimator(idleRightAnim);
                break;
            }
        }

        if (isPaused)
            return;

        currentAnimator.HandleUpdate();
    }

    private void SetCurrentAnimator(SpriteAnimator spriteAnimator)
    {
        if (currentAnimator != spriteAnimator)
        {
            currentAnimator = spriteAnimator;
            currentAnimator.Start();
        }
    }
}
