using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create new Monster")]
public class MonsterBase : ScriptableObject
{

    [SerializeField] private int id;
    [SerializeField] private string name;
    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite worldSprite;

    [SerializeField] private Attribute mainAttribute;
    [SerializeField] private Attribute secondaryAttribute;

    // Base Stats
    [SerializeField] private int maxHealth;
    [SerializeField] private int maxEnergy;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int speed;

    [SerializeField] private MoveBase basicMove;
    [SerializeField] private List<LearnableMove> learnableMoves;

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Sprite FrontSprite { get { return frontSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public Sprite WorldSprite { get { return worldSprite; } }
    public Attribute MainAttribute { get { return mainAttribute; } }
    public Attribute SecondaryAttribute { get { return secondaryAttribute; } }
    public int MaxHealth { get { return maxHealth; } }
    public int MaxEnergy { get { return maxEnergy; } }
    public int Attack { get { return attack; } }
    public int Defense { get { return defense; } }
    public int Speed { get { return speed; } }
    public MoveBase BasicMove { get { return basicMove; } }
    public List<LearnableMove> LearnableMoves { get { return learnableMoves; } }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int level;

    public MoveBase MoveBase { get { return moveBase; } }
    public int Level { get { return level; } }
}

public enum Attribute
{
    None,
    Neutral,
    Fire,
    Water,
    Air,
    Earth,
    Light,
    Dark
}

public class AttributeChart
{
    static private float[][] chart =
    {
        //                          NEUTRAL FIRE  WATER AIR   EARTH LIGHT DARK
        /* NEUTRAL */   new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        /* FIRE */      new float[] { 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f },
        /* WATER */     new float[] { 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f },
        /* AIR */       new float[] { 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f },
        /* EARTH */     new float[] { 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f },
        /* LIGHT */     new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f },
        /* DARK */      new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f }
    };

    public static float GetEffectiveness(Attribute attackAttribute, Attribute defenseAttribute)
    {
        if (attackAttribute == Attribute.None || defenseAttribute == Attribute.None)
            return 1;

        return chart[(int)attackAttribute - 1][(int)defenseAttribute - 1];
    }
}