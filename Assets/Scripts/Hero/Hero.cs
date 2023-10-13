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

    public Dice dice = new Dice(); //���ã���������ɫ������

    //ʵװӢ��Ѫ���ͼ��ܵ�unit�ĺ���
    public void Init()
    {
        Moves = new List<Move>();
        foreach(var move in Base.MovesOfDice)
        {
            //ʵװ����
            if(Moves .Count <= 6) { Moves.Add(new Move(move.Base)); }

            if (Moves.Count >= 6) break;
        }

        //ʵװ����
        CalculateStats();

        HP = MaxHP;
        //�������Լӳ�
        ResetStatBoost();
    }
    //-----------------------------���Բ���-----------------------------

    //��ȡ��ʼ����
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

    //ÿ��ս�������Ժ�����������ʼ��Ϊ0
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

    //��ȡ�Ķ��������
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f, 5.5f, 6f, 6.5f, 7f, 7.5f, 8f };

        if (boost >= 0) { statVal = Mathf.FloorToInt(statVal * boostValues[boost]); }
        else if (boost < 0){ statVal = Mathf.FloorToInt(statVal / boostValues[-boost]); }

        return statVal;
    }

    //���Կɶ�
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
    //-----------------------------�������Ӳ���-----------------------------
    public void CharacterRoll(Character character) //�Ը�roll
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
    public void LuckyRoll() //������
    {
        CharacterBoost.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("Lucky Boost!!!")}"));
    }

    //-----------------------------����״̬����-----------------------------
    public void SetStatus(ConditionID conditionID)
    {
        Status = ConditionsDB.Conditions[conditionID];
        StatusChanges.Enqueue(string.Format("{0}{1}",Base.HeroName,Status.StartMessage));
    }

    public void ResetStatus() //��Ϸ��ʼ�����쳣״̬
    { 
        Status = ConditionsDB.Conditions[ConditionID.none];
    }

    //�غ�ĩ����������״̬Ϊ�����¼�
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this); //��������״̬�Ļغ�ĩЧӦ
    }

    //-----------------------------��ֵcheck����-----------------------------
    //CritCheck �������
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

    //Damage �˺�����
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
    //ȷ�������ͷ�������ʲô����
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

    //Damage �˺�����
    //��ѪѪ�����£�����д�������ڴ���ǹ������µĵ�Ѫ
    public void UpdateHp(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HpChanged = true;
    }

    //����Ѫ������
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

    //�ٷֱȵ�Ѫ
    public void PercentDamage(LosePercentLife losePer)
    {
        int percent = Random.Range(losePer.min, losePer.max);
        UpdateHp(percent * HP/100);
        StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0} lose {1}% HP")}", Base.HeroName,percent));
    }

    //ƣ���˺�
    public void ExhaustedDamage(int turnCount)
    {
        UpdateHp((turnCount - 20) * _base.MaxHP / 4);
        StatusChanges.Enqueue(string.Format($"{Localize.GetInstance().GetTextByKey("{0} feels exhausted, and lose some HP")}", Base.HeroName));
    }


    //-----------------------------������������-----------------------------
    //���Ա仯
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
