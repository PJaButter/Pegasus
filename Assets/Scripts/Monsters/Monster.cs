using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] private MonsterBase monsterBase;
    [SerializeField] private int level;
    public MonsterBase MonsterBase { get { return monsterBase; } set { monsterBase = value; } }
    public int Level { get { return level; } set { level = value; } }

    public int CurrentHealth { get; set; }
    public int CurrentEnergy { get; set; }

    public Move BasicMove { get; set; }
    public List<Move> Moves { get; set; }

    public Dictionary <Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    public int MaxHealth { get; private set; }
    public int MaxEnergy { get; private set; }
    public int Attack { get { return GetStat(Stat.Attack); } }
    public int Defense { get { return GetStat(Stat.Defense); } }
    public int Speed { get { return GetStat(Stat.Speed); } }

    public void Init()
    {
        // Generate Moves
        BasicMove = new Move(MonsterBase.BasicMove);

        Moves = new List<Move>();
        foreach (LearnableMove move in MonsterBase.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));

                if (Moves.Count >= 4)
                    break;
            }
        }

        CalculateStats();

        CurrentHealth = MaxHealth;
        CurrentEnergy = MaxEnergy;

        ResetStatBoost();

    }

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

    public bool UseEnergy(int amount)
    {
        if (CurrentEnergy >= amount)
        {
            CurrentEnergy -= amount;
            return true;
        }

        return false;
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }

    public void ApplyStatBoosts(List<StatBoost> statBoosts)
    {
        foreach (StatBoost statBoost in statBoosts)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;

            // If you want to change the -6 or 6, you must also change "boostValues" in the GetStat method
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{MonsterBase.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{MonsterBase.Name}'s {stat} fell!");
        }
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((MonsterBase.Attack * Level) / 100.0f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((MonsterBase.Defense * Level) / 100.0f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((MonsterBase.Speed * Level) / 100.0f) + 5);

        MaxHealth = Mathf.FloorToInt((MonsterBase.MaxHealth * Level) / 100.0f) + 10;
        MaxEnergy = Mathf.FloorToInt((MonsterBase.MaxEnergy * Level) / 100.0f) + 10;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 }
        };
    }

    private int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // TODO: Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float CriticalModifier { get; set; }
    public float AttributeModifier { get; set; }
}