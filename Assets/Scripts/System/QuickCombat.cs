using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickCombat : MonoBehaviour
{
    public CombatState state;
    public int testAmount;
    public Hero p1Hero;
    public Hero p2Hero;
    bool isLastPlayer;
    int turnCount;
    public int currentValue;

    public int p1Win;
    public int p2Win;

    public void StartTestCombat1000()
    {
        p1Win = 0;
        p2Win = 0;
        for (int i = 0; i < testAmount; i++)
        {
            SetUpQuickCombat();
        }
        Debug.Log($"{p1Hero.Base.HeroName}ʤ��{p1Win}��{p2Hero.Base.HeroName}ʤ��{p2Win}��ʤ�ʱ�{(float)p1Win / (p1Win + p2Win)}:{(float)p2Win / (p1Win + p2Win)}");
    }

    public void SetUpQuickCombat()
    {
        turnCount = 0;
        p1Hero.HP = p1Hero.MaxHP;
        p2Hero.HP = p2Hero.MaxHP;
        p1Hero.ResetStatus();
        p2Hero.ResetStatus();
        p1Hero.ResetStatBoost();
        p2Hero.ResetStatBoost();
        isLastPlayer = false;
        NewTurn();
    }

    public void NewTurn()
    {
        turnCount += 1;
        if (SpeedCheck() == 1)
        {
            state = CombatState.P1TURN;
            Player1Turn();

        }
        else
        {
            state = CombatState.P2TURN;
            Player2Turn();
        }
    }
    public void Player1Turn()
    {
        //Debug.Log("1");
        //������
        int currentValue = DiceRoll(p1Hero.Base.Luck, p1Hero);
        //ѡ����
        var move = p1Hero.Moves[currentValue];
        //�ż���
        RunMove(p1Hero, p2Hero, move,currentValue);
    }
    public void Player2Turn()
    {
        //Debug.Log("2");
        //������
        int currentValue = DiceRoll(p2Hero.Base.Luck, p2Hero);
        //ѡ����
        var move = p2Hero.Moves[currentValue];
        //�ż���
        RunMove(p2Hero, p1Hero, move, currentValue);
    }
    //������
    public void RunMove(Hero source, Hero target, Move move,int currentValue)
    {
        //���˺�
        if (move.Base.MoveEffects.Delay == 0) { HandleBaseAttack(source, target, move, move.Base.IsMagic); }
        else
        {
            target.DelayDamage = target.CalculateDamage(move, source, target, currentValue, false);
            target.IsDelayDamageMagic = move.Base.IsMagic;
        }

        //�����ӳ��˺�
        if (source.DelayDamage != 0)
        {
            TakeDelayDamage(target, source);
        }
        //��������Ч
        HandleMoveEffects(move, source, target, currentValue);
        //�غϽ����׶δ������Ч��
        source.OnAfterTurn();
        //������Ч��
        HandleReborn(source); 
        HandleReborn(target); 
        //ƣ�ͼ춨
        CheckExhausted(source, turnCount);

        //Debug.Log($"˫��Ѫ����{p1Hero.HP},{p2Hero.HP}");

        //�ж�����
        CheckFainted(source, target);
    }
    //�����Ļ�������
    public void HandleBaseAttack(Hero source,Hero target, Move move,bool isMagic)
    {
        if (move.Base.ExtraTime > 0)
        {
            for (int i = move.Base.ExtraTime; i > 0; i--)
            {
                HandleDamage(source, target, move,currentValue, move.Base.IsMagic);
            }
        }
        HandleDamage(source, target, move, currentValue, move.Base.IsMagic);
    }
    //���˺�
    public void HandleDamage(Hero source, Hero target, Move move,int currentValue, bool isMagic)
    {
        if (move.Base.Power != 0)
        {
            //�����ж�
            bool isCrit = target.CritCheck(source.Base.Luck, target.Base.Luck);

            //�����˺�
            int damage = target.CalculateDamage(move, source, target, currentValue, isCrit);

            //�����˺�����
            if (damage != 0 && isCrit)
            {
                damage = (int)(damage * 1.5f);
            }
            //������Ч������
            if (damage >= target.HP && move.Base.IsNonLethal == true)
            {
                damage = target.HP - 1;
            }
            //����
            target.UpdateHp(damage);
        }
    }
    //�ӳ��˺�
    public void TakeDelayDamage(Hero source, Hero target)
    {
        //�����ж�
        bool isCrit = target.CritCheck(source.Base.Luck, target.Base.Luck);

        if (isCrit)
        {
            target.DelayDamage = (int)(target.DelayDamage * 1.5f);
        }

        target.UpdateHp(target.DelayDamage);
        target.DelayDamage = 0;
    }

    //������Ч��
    public void HandleMoveEffects(Move move, Hero source, Hero target, int diceValue)
    {
        //HandleDelayDamage(move, source, target); //�����ӳ��˺�������ս������Ҫ��
        HandleBoosts(move, source, target); //������������
        HandleAdaptiveBoosts(move, source, target); //����������������
        HandleStatus(move, source, target); //��������״̬
        HandleHeal(move, source, target, diceValue); //��������
        HandlePercentDamage(move, source, target); //����ǰ�����ٷֱȵ�Ѫ
        HandleBuffReborn(move, source); //������Buff
    }

    //ƣ�ͼ���
    public void CheckExhausted(Hero hero, int turnCount)
    {
        if (turnCount > 20)
        {
            hero.UpdateHp((turnCount - 20) * hero.Base.MaxHP / 4);
        }
    }

    //���غ�
    void PassTurn(Hero hero)
    {
        if (isLastPlayer) { isLastPlayer = false; NewTurn();  }
        else
        {
            if (hero == p1Hero)
            {
                //Debug.Log("��2��");
                isLastPlayer = true;
                state = CombatState.P2TURN;
                Player2Turn();
            }
            else
            {
                //Debug.Log("��1��");
                isLastPlayer = true;
                state = CombatState.P1TURN;
                Player1Turn();
            }
        }
    }
    //����
    void HandleReborn(Hero hero)
    {
        if (hero.IsReborn)
        {
            hero.HpChanged = true;
            hero.HP = hero.MaxHP / 2;
            hero.IsReborn = false;
        }
    }

    //�жϽ�ɫ�Ƿ�����
    public void CheckFainted(Hero source, Hero target)
    {
        if (target.HP <= 0)
        {
            if (source == p1Hero)
            {
                p1Win++;
            }
            else { p2Win++; }
            //Debug.Log($"P1WIN Turns:{turnCount}");
        }
        else if (source.HP <= 0)
        {
            if (source == p1Hero)
            {
                p2Win++;
            }
            else { p1Win++; }
            //Debug.Log($"P2WIN Turns:{turnCount}");
        }
        else
        {
            PassTurn(source);
        }
    }
    //--------------------------------------------------����Ч��
    public void HandleBoosts(Move move, Hero source, Hero target) //��������
    {
        var effect = move.Base.MoveEffects;
        if (effect.Boosts.Count != 0)
        {

            if (move.Base.StatChangeTarget == EffectTarget.Self)
            {
                source.ApplyBoosts(effect.Boosts);
            }
            else if (move.Base.StatChangeTarget == EffectTarget.Enemy)
            {
                target.ApplyBoosts(effect.Boosts);
            }
            else if (move.Base.StatChangeTarget == EffectTarget.All)
            {
                source.ApplyBoosts(effect.Boosts);
                target.ApplyBoosts(effect.Boosts);
            }
        }
    }
    public void HandleAdaptiveBoosts(Move move, Hero source,Hero target) //������������
    {
        var effect = move.Base.MoveEffects;
        if (effect.AdaptiveDecrease.boost != 0)
        {

            if (effect.AdaptiveDecrease.adaptiveType == AdaptiveType.Attack)
            {
                List<StatBoost> statboosts = new List<StatBoost>();
                StatBoost statBoost = new StatBoost(); ;
                if (target.Base.Attack >= target.Base.Magic) { statBoost.stat = Stat.Attack; }
                else { statBoost.stat = Stat.Magic; }
                statBoost.boost = effect.AdaptiveDecrease.boost;
                statboosts.Add(statBoost);
                target.ApplyBoosts(statboosts);
            }
            else if (effect.AdaptiveDecrease.adaptiveType == AdaptiveType.Defence)
            {
                List<StatBoost> statboosts = new List<StatBoost>();
                StatBoost statBoost = new StatBoost(); ;
                if (target.Base.Defence >= target.Base.MagicDef) { statBoost.stat = Stat.Defence; }
                else { statBoost.stat = Stat.MagicDef; }
                statBoost.boost = effect.AdaptiveDecrease.boost;
                statboosts.Add(statBoost);
                target.ApplyBoosts(statboosts);
            }
        }
    }
    public void HandleStatus(Move move, Hero source,Hero target)//����״̬
    {
        var effect = move.Base.MoveEffects;
        if (effect.Status != ConditionID.none)
        {
            target.SetStatus(effect.Status);
        }
    }

    public void HandleHeal(Move move, Hero source, Hero target, int diceValue) //����
    {
        var effect = move.Base.MoveEffects;
        if (effect.Heal != 0)
        {
            source.UpdateHpHeal(effect.Heal, diceValue);
        }
    }

    public void HandlePercentDamage(Move move, Hero source, Hero target) //��ǰ�����ٷֱȿ�Ѫ
    {
        var effect = move.Base.MoveEffects;
        var per = effect.LosePercentLife;
        if (per.max != 0)
        {
            if (per.target == EffectTarget.Self)
            {
                source.PercentDamage(per);
            }
            else if (per.target == EffectTarget.Enemy)
            {
                target.PercentDamage(per);
            }
        }
    }
    void HandleBuffReborn(Move move, Hero source) //����
    {
        var effect = move.Base.MoveEffects;
        if (effect.Reborn)
        {
            source.IsReborn = true;
        }
    }


    //--------------------------------------------------���Ӳ���
    //ҡһ��D6
    public int DiceRoll(int luck,Hero hero)
    {
        //�������¼�
        if(SixLuckyCheck(luck))
        {
            currentValue = 5; 
        }
        else
        {
            currentValue = CharacterRoll(hero);  //�Ը��¼�
        }
        return currentValue;
    }
    public bool SixLuckyCheck(float luckyBoost)
    {
        float par = Random.Range(0, 100);
        if (luckyBoost > par)
        {
            return true;
        }
        else return false;
    }
    public int CharacterRoll(Hero hero)
    {
        float par = Random.Range(0, 100); //�ο��������
        Character character = hero.Base.Character;
        if (character == Character.Ordinary) //��ͨ����
        {
            return DiceRollBase();
        }
        if (character == Character.Brave) //�¸����ӣ�ѪԽ�ٵ�Խ��
        {
            float boost = (1f - (float)hero.HP / hero.Base.MaxHP) * 50;
            if (boost > par)
            {
                return Mathf.Min(DiceRollBase() + 2, 5);
            }
            else return DiceRollBase();

        }
        if (character == Character.Timid)  //��С���ӣ�Ѫ���п�����1
        {
            float boost = (1f - (float)hero.HP / hero.Base.MaxHP) * 20;
            if (boost > par)
            {
                return 0;
            }
            else return DiceRollBase();
        }
        if (character == Character.Experienced) //�������ӣ���ǿ����������6
        {
            float boost = 4 * (hero.StatBoosts[Stat.Attack] + hero.StatBoosts[Stat.Defence] + hero.StatBoosts[Stat.Magic] + hero.StatBoosts[Stat.MagicDef] + hero.StatBoosts[Stat.Luck]);
            if (boost > par)
            {
                return Mathf.Min(DiceRollBase() + 1, 5);
            }
            else return DiceRollBase();
        }
        //------------------------------ר������------------------------------------
        if (character == Character.Slow_) //����������
        {
            float modifier = 4 * hero.StatBoosts[Stat.MagicDef] + (1f - (float)hero.HP / hero.Base.MaxHP) * 10;
            if (modifier > par)
            {
                return 1;
            }
            else return DiceRollBase();
        }
        return DiceRollBase();
    }

    //�Ӳ����ȵ���������
    public int DiceRollBase()
    {
        int par = Random.Range(0, 50);
        if (par < 5) { return 0; }
        else if (par < 15) { return 1; }
        else if (par < 25) { return 2; }
        else if (par < 35) { return 3; }
        else if (par < 45) { return 4; }
        else { return 5; }
    }


    public int SpeedCheck()
    {
        int firstPlayer;
        if (p1Hero.Speed > p2Hero.Speed)
        {
            firstPlayer = 1;
        }
        else if (p1Hero.Speed < p2Hero.Speed)
        {
            firstPlayer = 2;
        }
        else
        {
            firstPlayer = Random.Range(1, 3);
        }
        return firstPlayer;
    }
}
