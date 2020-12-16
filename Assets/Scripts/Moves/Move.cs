using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase MoveBase { get; }
    public int MaxUses { get; set; }

    public Move(MoveBase pMoveBase)
    {
        MoveBase = pMoveBase;
        MaxUses = pMoveBase.MaxUses;
    }
}
