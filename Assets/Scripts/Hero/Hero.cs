using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Hero
{
    [SerializeField] HeroBase _base;
    [SerializeField] int level;
    public HeroBase Base { get { return _base; } }
    public int Level { get { return level; } }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    public Dice dice = new Dice(); //备用，用来给角色绑骰子

    //实装英雄血量和技能到unit的函数
    public void Init()
    {
        Moves = new List<Move>();
        foreach(var move in Base.MovesOfDice)
        {
            //实装技能
            if(move.DiceNum <= 6) { Moves.Add(new Move(move.Base)); }

            if (Moves.Count >= 6) break;
        }

        //实装属性
        CalculateStats();

        HP = MaxHP;
        //重置属性加成
        ResetStatBoost();
    }
    //-----------------------------属性部分-----------------------------

    //获取初始属性
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defence, Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5);
        Stats.Add(Stat.Magic, Mathf.FloorToInt((Base.Magic * Level) / 100f) + 5);
        Stats.Add(Stat.MagicDef, Mathf.FloorToInt((Base.MagicDef * Level) / 100f) + 5);
        Stats.Add(Stat.Luck, Mathf.FloorToInt(Base.Luck));

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10;
    }

    //每场战斗结束以后属性增长初始化为0
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat .Attack,0 },
            {Stat .Defence,0 },
            {Stat .Magic,0 },
            {Stat .MagicDef,0 },
            {Stat .Luck,0 },
        };
    }

    //获取改动后的属性
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //TODO:statBoost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f, 5.5f, 6f, 6.5f, 7f, 7.5f, 8f };

        if (boost >= 0) { statVal = Mathf.FloorToInt(statVal * boostValues[boost]); }
        else { statVal = Mathf.FloorToInt(statVal / boostValues[boost]); }

        return statVal;
    }

    //属性可读
    public Sprite Sprite{
        get { return Base.Sprite; }
    }

    public int MaxHP{ get; private set; }

    public int Attack{
        get { return GetStat (Stat.Attack); }
    }

    public int Defence{
        get { return GetStat(Stat.Defence); }
    }

    public int Magic{
        get { return GetStat(Stat.Magic); }
    }

    public int MagicDef{
        get { return GetStat(Stat.MagicDef); }
    }

    public int Speed{
        get { return Mathf.FloorToInt(Base.Speed); }
    }

    public int Evasion{
        get { return GetStat(Stat.Attack); }
    }

    public int Luck{
        get { return GetStat(Stat.Luck); }
    }

    //-----------------------------数值check部分-----------------------------
    //CritCheck 暴击检查
    public bool CritCheck(int luck)
    {
        if (Random.value * 100f <= 4f+(luck/2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Damage 伤害计算
    public int CalculateDamage(Move move, Hero attacker, int currentValue)
    {
        float attack = (move.Base.MoveCatagory == MoveCatagory.Magic) ? attacker.Magic : attacker.Attack;
        float defence = (move.Base.MoveCatagory == MoveCatagory.Physics) ? attacker.MagicDef : attacker.Defence ;

        //Debug.Log("骰子点数：" + currentValue);
        float modifiers = ((currentValue / 10f) + 0.7f);
        //Debug.Log("伤害调整值：" + modifiers);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)Attack / Defence) + 2;
        if (move.Base.Power == 0)
        {
            d = 0;
        }
        int damage = Mathf.FloorToInt(d * modifiers);
        //Debug.Log("伤害：" + damage);
        return damage;
    }

    //Damage 伤害处理
    public bool TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        return false;
    }

    //属性变化
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp ( StatBoosts[stat] + boost,-12,12);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.HeroName}'s {stat} get {boost*50}% up!");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.HeroName}'s {stat} get {boost*50}% up!");
            }

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]} ");
        }
    }

}
