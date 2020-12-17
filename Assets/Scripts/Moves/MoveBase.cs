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

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Attribute Attribute { get { return attribute; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int EnergyCost { get { return energyCost; } }
}
