using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Hero/Create a new move")]


public class MoveBase : ScriptableObject 
{
    [SerializeField] string moveName;
    
    [TextArea]
    [SerializeField] string description;
    [SerializeField] bool isMagic;
    [SerializeField] bool isNonLethal;
    [SerializeField] bool isDrain;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int extraTime;

    [SerializeField] MoveActionType moveActionType;
    [SerializeField] MoveCatagory moveCatagory;
    [SerializeField] MoveEffects moveEffects;

    [Header("Targets")]
    [SerializeField] EffectTarget statChangeTarget;
    [SerializeField] EffectTarget statusTarget;

    [Header("SoundEffects")]
    [SerializeField] AudioClip performSE;
    [SerializeField] AudioClip hitSE;
    [SerializeField] AudioClip effectSE;
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
    public bool IsNonLethal{
        get { return isNonLethal; }
    }
    public bool IsDrain
    {
        get { return isDrain; }
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

    public AudioClip PerformSE
    {
        get { return performSE; }
    }
    public AudioClip HitSE
    {
        get { return hitSE; }
    }
    public AudioClip EffectSE
    {
        get { return effectSE; }
    }

    public EffectTarget StatChangeTarget{
        get { return statChangeTarget; }
    }
    public EffectTarget StatusTarget
    {
        get { return statusTarget; }
    }
}
[System.Serializable]
//技能特效
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] AdaptiveDecrease adaptiveDecrease;
    [SerializeField] ConditionID status;
    [SerializeField] bool reborn;
    [SerializeField] int heal;
    [SerializeField] int delay;
    [SerializeField] LosePercentLife losePercentLife;


    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public AdaptiveDecrease AdaptiveDecrease
    {
        get { return adaptiveDecrease; }
    }
    public ConditionID Status
    {
        get { return status; }
    }
    public bool Reborn
    {
        get { return reborn; }
    }
    public int Heal
    {
        get { return heal; }
    }
    public int Delay
    {
        get { return delay; }
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
[System.Serializable]
public class AdaptiveDecrease
{
    public AdaptiveType adaptiveType;
    public int boost;
}
public enum AdaptiveType
{
    Attack,Defence
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