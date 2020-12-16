using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] MonsterBase monsterBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    private Image monsterImage;
    private Animator animator;

    public Monster Monster { get; set; }

    private void Awake()
    {
        monsterImage = GetComponent<Image>();
        animator = GetComponent<Animator>();

        animator.SetBool("IsPlayer", isPlayerUnit);
    }

    public void Setup()
    {
        Monster = new Monster(monsterBase, level);
        if (isPlayerUnit)
        {
            monsterImage.sprite = Monster.MonsterBase.BackSprite;
        }
        else
        {
            monsterImage.sprite = Monster.MonsterBase.FrontSprite;
        }

        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        animator.SetTrigger("EnterBattle");
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayHitAnimation()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}
