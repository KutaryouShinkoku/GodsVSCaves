using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public int currentValue;
    //ҡһ��D6
    public int DiceRoll(int luck)
    {
        if(sixLuckyCheck(luck))
        {
            currentValue = 6; 
        }
        else
        {
            currentValue = Random.Range(0, 6);
        }
        return currentValue;
    }

    //�������ֵ�Ƿ�ֱ�ӳ�6
    public bool sixLuckyCheck(float luckyBoost)
    {
        float par = Random.Range(0, 100);
        if (luckyBoost > par)
        {
            return true;
        }
        else return false;
    }
}
