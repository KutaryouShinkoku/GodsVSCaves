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
        if (hero .Base .Character  == Character.Ordinary) //��ͨ����
        {
            return Random.Range(0, 6);
        }
        if(hero .Base .Character == Character.Brave) //�¸����ӣ�ѪԽ�ٵ�Խ��
        {
            float boost = (1f-(float)hero.HP / hero.Base.MaxHP)*50;
            Debug.Log(par + "/" + boost);
            if (boost >= par)
            {
                hero.CharacterRoll(hero.Base.Character); 
                return Mathf.Min(Random.Range(0, 6) + 2, 5);
            }
            else return Random.Range(0, 6);

        }
        if(hero .Base .Character == Character.Timid)  //��С���ӣ�Ѫ���п�����1
        {
            float boost = (1f- (float)hero.HP / hero.Base.MaxHP)*20;
            if(boost >= par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return 0;
            }
            else return Random.Range(0, 6);
        }
        if(hero .Base .Character == Character.Experienced) //�������ӣ���ǿ����������6
        {
            float boost = 4 * (hero.StatBoosts[Stat.Attack] + hero.StatBoosts[Stat.Defence] + hero.StatBoosts[Stat.Magic] + hero.StatBoosts[Stat.MagicDef] + hero.StatBoosts[Stat.Luck]);
            if(boost >= par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return Mathf.Min(Random.Range(0, 6) + 1, 5);
            }
            else return Random.Range(0, 6);
        }
        return Random.Range(0, 6);
    }
}
