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
        Character character = hero.Base.Character;
        if (character == Character.Ordinary) //普通骰子
        {
            return DiceRollBase();
        }
        if(character == Character.Brave) //勇敢骰子，血越少点越大
        {
            float boost = (1f-(float)hero.HP / hero.Base.MaxHP)*50;
            if (boost > par)
            {
                hero.CharacterRoll(hero.Base.Character); 
                return Mathf.Min(DiceRollBase() + 2, 5);
            }
            else return DiceRollBase();

        }
        if(character == Character.Timid)  //胆小骰子，血少有可能扔1
        {
            float boost = (1f- (float)hero.HP / hero.Base.MaxHP)*20;
            if(boost > par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return 0;
            }
            else return DiceRollBase();
        }
        if(character == Character.Experienced) //老练骰子，被强化更可能扔6
        {
            float boost = 4 * (hero.StatBoosts[Stat.Attack] + hero.StatBoosts[Stat.Defence] + hero.StatBoosts[Stat.Magic] + hero.StatBoosts[Stat.MagicDef] + hero.StatBoosts[Stat.Luck]);
            if(boost > par)
            {
                hero.CharacterRoll(hero.Base.Character);
                return Mathf.Min(DiceRollBase() + 1, 5);
            }
            else return DiceRollBase();
        }
        //------------------------------专属骰子------------------------------------
        if(character == Character.Slow_) //吧主甲骰子
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

    //扔不均匀的六面骰子
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
