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

    public Dice dice = new Dice();

    public void Init()
    {
        HP = MaxHP;

        Moves = new List<Move>();
        foreach(var move in Base.MovesOfDice)
        {
            //实装技能
            if(move.DiceNum <= 6) { Moves.Add(new Move(move.Base)); }

            if (Moves.Count >= 6) break;
        }
    }

    public Sprite Sprite{
        get { return Base.Sprite; }
    }

    public Sprite Frame{
        get { return Base.Frame; }
    }
    public int MaxHP{
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10; }
    }
    public int Attack{
        get { return Mathf.FloorToInt((Base.Attack * Level)/100f) +5; }
    }

    public int Defence{
        get { return Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5; }
    }

    public int Magic{
        get { return Mathf.FloorToInt((Base.Magic * Level) / 100f) + 5; }
    }

    public int MagicDef{
        get { return Mathf.FloorToInt((Base.MagicDef * Level) / 100f) + 5; }
    }

    public int Speed{
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    //CritCheck 暴击检查
    public bool CritCheck()
    {
        if (Random.value * 100f <= 4f)
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
        float attack = (move.Base.IsMagic) ? attacker.Magic : attacker.Attack;
        float defence = (move.Base.IsMagic) ? attacker.MagicDef : attacker.Defence ;

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

}
