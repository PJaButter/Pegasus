using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] string name;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private Attribute attribute;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int energyCost;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private MoveTarget target;

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Attribute Attribute { get { return attribute; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int EnergyCost { get { return energyCost; } }
    public MoveCategory Category { get { return category; } }
    public MoveEffects Effects { get { return effects; } }
    public MoveTarget Target { get { return target; } }
}

public enum MoveCategory
{
    Nonstatus,
    Status
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> statBoosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> StatBoosts { get { return statBoosts; } }
    public ConditionID Status { get { return status; } }
    public ConditionID VolatileStatus { get { return volatileStatus; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveTarget
{
    Foe, Self
}
