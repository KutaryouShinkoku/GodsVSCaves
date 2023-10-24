using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public int currentValue;
    //ҡһ��D6
    public int DiceRoll(int luck,Hero hero)
    {
        //�������¼�
        if(SixLuckyCheck(luck))
        {
            hero.LuckyRoll();
            currentValue = 6; 
        }
        else
        {
            currentValue = CharacterRoll(hero);  //�Ը��¼�
        }
        return currentValue;
    }

    //�������ֵ�Ƿ�ֱ�ӳ�6
    public bool SixLuckyCheck(float luckyBoost)
    {
        float par = Random.Range(0, 100);
        if (luckyBoost > par)
        {
            return true;
        }
        else return false;
    }

    //ÿ���Ը�ļӳɻ���
    public int CharacterRoll(Hero hero)
    {
        float par = Random.Range(0, 100); //�ο��������
        Character character = hero.Base.Character;
        if (character == Character.Ordinary) //��ͨ����
        {
            return DiceRollBase();
        }
        if(character == Character.Brave) //�¸����ӣ�ѪԽ�ٵ�Խ��
        {
            float boost = (1f-(float)hero.HP / hero.Base.MaxHP)*50;
            if (boost > par)
            {
                hero.CharacterRoll(hero.Base.Character); 
                return Mathf.Min(DiceRollBase() + 2, 5);
            }
            else return DiceRollBase();

        }
        if(character == Character.Timid)  //��С���ӣ�Ѫ���п�����1
        {
            float boost = (1f- (float)hero.HP / hero.Base.MaxHP)*20;
            if(boost > par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return 0;
            }
            else return DiceRollBase();
        }
        if(character == Character.Experienced) //�������ӣ���ǿ����������6
        {
            float boost = 4 * (hero.StatBoosts[Stat.Attack] + hero.StatBoosts[Stat.Defence] + hero.StatBoosts[Stat.Magic] + hero.StatBoosts[Stat.MagicDef] + hero.StatBoosts[Stat.Luck]);
            if(boost > par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return Mathf.Min(DiceRollBase() + 1, 5);
            }
            else return DiceRollBase();
        }
        //------------------------------ר������------------------------------------
        if(character == Character.Slow_) //����������
        {
            float modifier = 4*hero.StatBoosts[Stat.MagicDef]+ (1f - (float)hero.HP / hero.Base.MaxHP) * 10;
            if(modifier >par) 
            {
                hero.CharacterRoll(hero.Base.Character);
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
        if (par < 5){return 0;}
        else if(par < 15) { return 1; }
        else if (par < 25) { return 2; }
        else if (par < 35) { return 3; }
        else if(par < 45) { return 4; }
        else { return 5; }
    }
}
