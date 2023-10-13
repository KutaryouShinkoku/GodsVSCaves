using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Hero/Create a new move")]


public class MoveBase : ScriptableObject 
{
    [SerializeField] string moveName;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int extraTime;
    [SerializeField] bool isMagic;
    [SerializeField] MoveActionType moveActionType;
    [SerializeField] MoveCatagory moveCatagory;
    [SerializeField] MoveEffects moveEffects;

    [Header("This setting is stat effects only")]
    [SerializeField] EffectTarget effectTarget;
    public string MoveName{
        get { return $"{Localize.GetInstance().GetTextByKey($"{moveName}")}"; }
    }

    public string Description{
        get { return $"{Localize.GetInstance().GetTextByKey($"{description}")}"; }
    }

    public int Power{
        get { return power; }
    }

    public int Accuracy{
        get { return accuracy; }
    }
    public int ExtraTime{
        get { return extraTime; }
    }

    public bool IsMagic{
        get { return isMagic; }
    }

    public MoveActionType MoveActionType{
        get { return moveActionType; }
    }

    public MoveCatagory MoveCatagory { 
        get { return moveCatagory; }
    }

    public MoveEffects MoveEffects{
        get { return moveEffects; }
    }

    public EffectTarget EffectTarget{
        get { return effectTarget; }
    }
}
[System.Serializable]
//技能特效
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] int heal;
    [SerializeField] LosePercentLife losePercentLife;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public ConditionID Status
    {
        get { return status; }
    }
    public int Heal
    {
        get { return heal; }
    }
    public LosePercentLife LosePercentLife
    {
        get { return losePercentLife; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

[System.Serializable]
public class LosePercentLife
{
    public int min;
    public int max;
    public EffectTarget target;
}



//技能动画类型
public enum MoveActionType
{
    Melee,Ranged,Heal,Special
}
//技能类型
public enum MoveCatagory
{
    Physics,Magic,Shield,Heal,Status,Special
}

public enum EffectTarget
{
    Self,//自己
    Enemy, //敌人
    All,//全部
}