using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Move CurrentMove { get; set; }

    public Dictionary <Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HealthChanged { get; set; }
    public bool EnergyChanged { get; set; }
    public event System.Action OnStatusChanged;

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

                if (Moves.Count >= 5)
                    break;
            }
        }

        CalculateStats();

        CurrentHealth = MaxHealth;
        CurrentEnergy = MaxEnergy;

        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
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

        UpdateHealth(damage);
        return damageDetails;
    }

    public void UpdateHealth(int damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        HealthChanged = true;
    }

    public void UpdateEnergy(int amount)
    {
        CurrentEnergy = Mathf.Clamp(CurrentEnergy - amount, 0, MaxEnergy);
        EnergyChanged = true;
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
            return;

        Status = ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{MonsterBase.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null)
            return;

        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{MonsterBase.Name} {VolatileStatus.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
        OnStatusChanged?.Invoke();
    }

    public Move GetRandomMove()
    {
        List<Move> useableMoves = Moves.Where(x => x.MoveBase.EnergyCost <= CurrentEnergy).ToList();
        useableMoves.Add(BasicMove);

        int randomMoveIndex = Random.Range(0, useableMoves.Count);
        return useableMoves[randomMoveIndex];
    }

    public bool UseEnergy(int amount)
    {
        if (CurrentEnergy >= amount)
        {
            UpdateEnergy(amount);
            return true;
        }

        return false;
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
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

        MaxHealth = Mathf.FloorToInt((MonsterBase.MaxHealth * Level) / 100.0f) + 10 + Level;
        MaxEnergy = Mathf.FloorToInt((MonsterBase.MaxEnergy * Level) / 100.0f) + 10 + Level;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 }
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