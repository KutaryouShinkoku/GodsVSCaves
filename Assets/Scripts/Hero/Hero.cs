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
    public Condition Status { get; set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Queue<string> CharacterBoost { get; private set; } = new Queue<string>();

    public bool HpChanged { get; set; }

    public Dice dice = new Dice(); //备用，用来给角色绑骰子

    //实装英雄血量和技能到unit的函数
    public void Init()
    {
        Moves = new List<Move>();
        foreach(var move in Base.MovesOfDice)
        {
            //实装技能
            if(Moves .Count <= 6) { Moves.Add(new Move(move.Base)); }

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
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
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

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f, 5.5f, 6f, 6.5f, 7f, 7.5f, 8f };

        if (boost >= 0) { statVal = Mathf.FloorToInt(statVal * boostValues[boost]); }
        else if (boost < 0){ statVal = Mathf.FloorToInt(statVal / boostValues[-boost]); }

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
        get { return GetStat(Stat.Speed); }
    }

    public int Evasion{
        get { return GetStat(Stat.Attack); }
    }

    public int Luck{
        get { return GetStat(Stat.Luck); }
    }
    //-----------------------------特殊骰子部分-----------------------------
    public void CharacterRoll(Character character) //性格roll
    {
        switch (character)
        {
            case Character.Ordinary:
                break;
            case Character.Brave:
                CharacterBoost.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("The more {0}'s dice fights, the braver it becomes!")}", Base.HeroName));
                break;
            case Character.Timid:
                CharacterBoost.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0}'s dice is scared because its HP is too low!")}", Base.HeroName));
                break;
            case Character.Experienced:
                CharacterBoost.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("Boosts double {0}'s dice's confidence!")}", Base.HeroName));
                break;
        }
        
    }
    public void LuckyRoll() //超幸运
    {
        CharacterBoost.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("Lucky Boost!!!")}"));
    }

    //-----------------------------特殊状态部分-----------------------------
    public void SetStatus(ConditionID conditionID)
    {
        Status = ConditionsDB.Conditions[conditionID];
        StatusChanges.Enqueue(string.Format("{0}{1}",Base.HeroName,Status.StartMessage));
    }

    public void ResetStatus() //游戏开始重置异常状态
    { 
        Status = ConditionsDB.Conditions[ConditionID.none];
    }

    //回合末处理以特殊状态为主的事件
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this); //处理特殊状态的回合末效应
    }

    //-----------------------------数值check部分-----------------------------
    //CritCheck 暴击检查
    public bool CritCheck(int sourceLuck, int targetLuck)
    {
        if (Random.value * 100f <= Mathf .Abs( 4f+((targetLuck-sourceLuck)/2)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Damage 伤害计算
    public int CalculateDamage(Move move, Hero attacker, Hero defender, int currentValue,bool isCrit)
    {
        float attack = CalculateAttakerStat(move, attacker);
        float defence = CalculateDefenderStat(move, defender,isCrit);


        float modifiers = ((currentValue / 10f) + 0.7f+Random .Range(-0.15f,0.15f));
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defence) + 2;
        if (move.Base.Power == 0)
        {
            d = 0;
        }
        int damage = Mathf.FloorToInt(d * modifiers);
        return damage;
    }
    //确定进攻和防御者用什么属性
    public float CalculateAttakerStat(Move move, Hero attacker)
    {
        if(move.Base .MoveCatagory == MoveCatagory.Physics) { return attacker.Attack; }
        else if (move.Base.MoveCatagory == MoveCatagory.Magic ) { return attacker.Magic ; }
        else if (move.Base.MoveCatagory == MoveCatagory.Shield) { return attacker.Defence ; }
        else return attacker.Attack;
    }
    public float CalculateDefenderStat(Move move,Hero defender,bool isCrit)
    {
        if (isCrit)
        {
            if (move.Base.MoveCatagory == MoveCatagory.Magic) { return Mathf.Min( defender.MagicDef,defender.Base.MagicDef); }
            else return Mathf.Min(defender.Defence,defender.Base.Defence);
        }
        else
        {
            if (move.Base.MoveCatagory == MoveCatagory.Magic) { return defender.MagicDef; }
            else return defender.Defence;
        }
    }

    //Damage 伤害处理
    //掉血血量更新，单独写出来用于处理非攻击导致的掉血
    public void UpdateHp(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HpChanged = true;
    }

    //治疗血量更新
    public void UpdateHpHeal(int heal,int diceValue)
    {
        HP = (int)Mathf.Min(HP + (1f /heal) * MaxHP* diceValue, MaxHP);
        if (heal == 1)
        {
            StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0}'s HP is restored to full")}", Base.HeroName));
        }
        else { StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0}'s HP is restored")}", Base.HeroName)); }
        HpChanged = true;
    }

    //百分比掉血
    public void PercentDamage(LosePercentLife losePer)
    {
        int percent = Random.Range(losePer.min, losePer.max);
        UpdateHp(percent * HP/100);
        StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0} lose {1}% HP")}", Base.HeroName,percent));
    }

    //疲劳伤害
    public void ExhaustedDamage(int turnCount)
    {
        UpdateHp((turnCount - 20) * _base.MaxHP / 4);
        StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0} feels exhausted, and lose some HP")}", Base.HeroName));
    }


    //-----------------------------属性增减部分-----------------------------
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
                StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0}'s {1} increase {2}%!")}", Base.HeroName, Localize.GetInstance().GetTextByKey($"{stat}"), boost * 50));
            }
            else
            {
                StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0}'s {1} decrease {2}%")}", Base.HeroName, Localize.GetInstance().GetTextByKey($"{stat}"), -boost * 50));
            }

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]} ");
        }
    }
}
