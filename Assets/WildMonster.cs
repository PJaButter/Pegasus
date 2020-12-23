using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildMonster : MonoBehaviour
{
    public event Action<WildMonster> OnDefeated;
    [SerializeField] private Monster monster;

    public Monster Monster { get { return monster; } set { monster = value; } }

    private void Start()
    {
        if (monster != null)
        {
            monster.Init();
        }
    }

    public void Defeated()
    {
        OnDefeated(this);
    }
}
