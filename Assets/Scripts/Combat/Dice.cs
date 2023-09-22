using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{

    public int CurrentValue;
    public int DiceRoll()
    {
        CurrentValue = Random.Range(0, 6);
        return CurrentValue;
    }
}
