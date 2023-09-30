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
    [SerializeField] bool isMagic;
    [SerializeField] MoveActionType moveActionType;
    [SerializeField] MoveCatagory moveCatagory;
    [SerializeField] MoveEffects moveEffects;
    [SerializeField] MoveTarget moveTarget;
    public string MoveName{
        get { return moveName; }
    }

    public string Description{
        get { return description; }
    }

    public int Power{
        get { return power; }
    }

    public int Accuracy{
        get { return accuracy; }
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

    public MoveTarget MoveTarget{
        get { return moveTarget; }
    }
}
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

//技能动画类型
public enum MoveActionType
{
    Melee,Ranged,Heal,Special
}
//技能类型
public enum MoveCatagory
{
    Physics,Magic,Heal,Status,Special
}

public enum MoveTarget
{
    Self,Enemy
}