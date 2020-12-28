using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (KeyValuePair<ConditionID, Condition> kvp in Conditions)
        {
            ConditionID conditionID = kvp.Key;
            Condition condition = kvp.Value;

            condition.ID = conditionID;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.Poison, new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned.",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHealth(monster.MaxHealth / 8);
                    monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} was hurt from poison.");
                }
            }
        },
        {
            ConditionID.Burn, new Condition()
            {
                Name = "Burn",
                StartMessage = "has caught on fire.",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHealth(monster.MaxHealth / 16);
                    monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} was hurt from the fire.");
                }
            }
        },
        {
            ConditionID.Paralyze, new Condition()
            {
                Name = "Paralyze",
                StartMessage = "has been paralyzed.",
                OnBeforeMove = (Monster monster) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name}'s paralyzed and can't move.");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.Freeze, new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen.",
                OnBeforeMove = (Monster monster) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} is no longer frozen.");
                        monster.CureStatus();
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.Sleep, new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep.",
                OnStart = (Monster monster) =>
                {
                    // Sleep for 1-3 turns
                    monster.StatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.StatusTime <= 0)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} has woken up.");
                        return true;
                    }

                    --monster.StatusTime;
                    monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} is sleeping.");
                    return false;
                }
            }
        },
        // Volatile Status Conditions
        {
            ConditionID.Confusion, new Condition()
            {
                Name = "Confusion",
                StartMessage = "has gotten confused.",
                OnStart = (Monster monster) =>
                {
                    // Confused for 1-4 turns
                    monster.StatusTime = Random.Range(1, 5);
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} has broken free of confusion.");
                        return true;
                    }

                    --monster.VolatileStatusTime;
                    monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} is confused.");

                    // 50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return true;

                    // Hurt by confusion
                    monster.UpdateHealth(monster.MaxHealth / 8);
                    monster.StatusChanges.Enqueue($"{monster.MonsterBase.Name} hurt itself in it's confusion.");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    None,
    Poison,
    Sleep,
    Paralyze,
    Freeze,
    Confusion,

    Burn, // Fire
    Drown, // Water
    Stun, // Earth
    KnockBack, // Air
    Blind, // Light
    Unconscious // Dark
}