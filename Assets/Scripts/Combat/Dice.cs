using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public int currentValue;
    //摇一个D6
    public int DiceRoll(int luck,Hero hero)
    {
        //超幸运事件
        if(SixLuckyCheck(luck))
        {
            hero.LuckyRoll();
            currentValue = 6; 
        }
        else
        {
            currentValue = CharacterRoll(hero);  //性格事件
        }
        return currentValue;
    }

    //检查幸运值是否直接出6
    public bool SixLuckyCheck(float luckyBoost)
    {
        float par = Random.Range(0, 100);
        if (luckyBoost > par)
        {
            return true;
        }
        else return false;
    }

    //每种性格的加成机制
    public int CharacterRoll(Hero hero)
    {
        float par = Random.Range(0, 100); //参考用随机数
        if (hero .Base .Character  == Character.Ordinary) //普通骰子
        {
            return Random.Range(0, 6);
        }
        if(hero .Base .Character == Character.Brave) //勇敢骰子，血越少点越大
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
        if(hero .Base .Character == Character.Timid)  //胆小骰子，血少有可能扔1
        {
            float boost = (1f- (float)hero.HP / hero.Base.MaxHP)*20;
            if(boost >= par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return 0;
            }
            else return Random.Range(0, 6);
        }
        if(hero .Base .Character == Character.Experienced) //老练骰子，被强化更可能扔6
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
