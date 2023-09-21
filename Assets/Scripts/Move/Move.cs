using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }

    public Move(MoveBase hBase)
    {
        Base = hBase;
    }
}
