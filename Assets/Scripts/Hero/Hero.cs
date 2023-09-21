using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero
{
    public HeroBase Base { get; set; }
    public int level { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Hero(HeroBase hBase,int hLevel)
    {
        Base = hBase;
        level = hLevel;
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
        get { return Mathf.FloorToInt((Base.MaxHP * level) / 100f) + 10; }
    }
    public int Attack{
        get { return Mathf.FloorToInt((Base.Attack * level)/100f) +5; }
    }

    public int Defence{
        get { return Mathf.FloorToInt((Base.Defence * level) / 100f) + 5; }
    }

    public int Magic{
        get { return Mathf.FloorToInt((Base.Magic * level) / 100f) + 5; }
    }

    public int MagicDef{
        get { return Mathf.FloorToInt((Base.MagicDef * level) / 100f) + 5; }
    }

    public int Speed{
        get { return Mathf.FloorToInt((Base.Speed * level) / 100f) + 5; }
    }
}
