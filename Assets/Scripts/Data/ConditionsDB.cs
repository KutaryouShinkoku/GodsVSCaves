using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConditionsDB
{

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        { 
            ConditionID.none,
            new Condition ()
        },
        {
            ConditionID.psn ,
            new Condition()
            {
                ConditionName = $"{Localize.GetInstance().GetTextByKey("Poison")}",
                StartMessage = $"{Localize.GetInstance().GetTextByKey("has been poisoned")}",
                OnAfterTurn = (Hero hero) =>
                {
                    hero.UpdateHp(hero.MaxHP/8);
                    hero.StatusChanges.Enqueue(string .Format($"{Localize.GetInstance().GetTextByKey("{0} lose life due to poison")}",hero.Base.HeroName));
                }
            }
        }
    };
}

public enum ConditionID
{
    none, //Œﬁ
    psn, //÷–∂æ
    stn, //—£‘Œ
}