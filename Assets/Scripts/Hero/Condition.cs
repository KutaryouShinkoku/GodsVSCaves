using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition
{
    public string ConditionName { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public ConditionID ConditionID { get; set; }

    public Action<Hero> OnAfterTurn { get; set; }
}
