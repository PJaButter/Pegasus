using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters;

    private void Start()
    {
        foreach (Monster monster in monsters)
        {
            monster.Init();
        }
    }

    public Monster GetHealthyMonster()
    {
        return monsters.Where(x => x.CurrentHealth > 0).FirstOrDefault();
    }
}
