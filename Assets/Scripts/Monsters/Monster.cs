using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public MonsterBase MonsterBase { get; set; }
    public int Level { get; set; }

    public int CurrentHealth { get; set; }
    public int CurrentEnergy { get; set; }

    public Move BasicMove { get; set; }
    public List<Move> Moves { get; set; }

    public Monster(MonsterBase pMonsterBase, int iLevel)
    {
        MonsterBase = pMonsterBase;
        Level = iLevel;
        CurrentHealth = MaxHealth;
        CurrentEnergy = MaxEnergy;

        // Generate Moves
        BasicMove = new Move(pMonsterBase.BasicMove);

        Moves = new List<Move>();
        foreach (LearnableMove move in pMonsterBase.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));

                if (Moves.Count >= 4)
                    break;
            }
        }
    }

    public int MaxHealth { get { return Mathf.FloorToInt((MonsterBase.MaxHealth * Level) / 100.0f) + 10; } }
    public int MaxEnergy { get { return Mathf.FloorToInt((MonsterBase.MaxEnergy * Level) / 100.0f) + 10; } }
    public int Attack { get { return Mathf.FloorToInt((MonsterBase.Attack * Level) / 100.0f) + 5; } }
    public int Defense { get { return Mathf.FloorToInt((MonsterBase.Defense * Level) / 100.0f) + 5; } }
    public int SpecialAttack { get { return Mathf.FloorToInt((MonsterBase.SpecialAttack * Level) / 100.0f) + 5; } }
    public int SpecialDefense { get { return Mathf.FloorToInt((MonsterBase.SpecialDefense * Level) / 100.0f) + 5; } }
    public int Speed { get { return Mathf.FloorToInt((MonsterBase.Speed * Level) / 100.0f) + 5; } }

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        float criticalModifer = 1.0f;
        if (Random.value * 100.0f < 6.25f)
        {
            criticalModifer = 2.0f;
        }

        float attributeModifer = AttributeChart.GetEffectiveness(move.MoveBase.Attribute, this.MonsterBase.MainAttribute) * 
                                 AttributeChart.GetEffectiveness(move.MoveBase.Attribute, this.MonsterBase.SecondaryAttribute);

        DamageDetails damageDetails = new DamageDetails()
        {
            AttributeModifier = attributeModifer,
            CriticalModifier = criticalModifer,
            Fainted = false
        };

        float modifiers = Random.Range(0.85f, 1.0f) * attributeModifer * criticalModifer;
        float a = (2 * attacker.Level + 10) / 250.0f;
        float d = a * move.MoveBase.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            // Monster died
            CurrentHealth = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count + 1);

        if (r == 0)
        {
            return BasicMove;
        }
        else
        {
            return Moves[r - 1];
        }
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float CriticalModifier { get; set; }
    public float AttributeModifier { get; set; }
}