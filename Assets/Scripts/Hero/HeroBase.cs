using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Hero", menuName = "Hero/Create a new hero")]
public class HeroBase : ScriptableObject 
{
    [SerializeField] string name;
    [SerializeField] HeroCamp camp;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite sprite;
    [SerializeField] Sprite frame;

    //Base stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int magic;
    [SerializeField] int magicDef;
    [SerializeField] int speed;
    [SerializeField] int evasion;
    [SerializeField] List<MoveOfDice> movesOfDice;

    public string Name{
        get{ return name;}
    }

    public string Description{
        get { return description; }
    }

    public Sprite Sprite{
        get { return sprite; }
    }

    public Sprite Frame{
        get { return frame; }
    }

    public int MaxHP{
        get { return maxHP; }
    }

    public int Attack{
        get { return attack; }
    }

    public int Defence{
        get { return defence; }
    }

    public int Magic{
        get { return magic; }
    }

    public int MagicDef{
        get { return magicDef; }
    }

    public int Speed{
        get { return speed; }
    }

    public int Evasion{
        get { return evasion; }
    }

    public List<MoveOfDice> MovesOfDice{
        get { return movesOfDice; }
    }
}

//Moves of Dice 骰子对应的技能
[System.Serializable]
public class MoveOfDice
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int diceNum;

    public MoveBase Base{
        get { return moveBase; }
    }

    public int DiceNum{
        get { return diceNum; }
    }
}


//Camp 阵营
public enum HeroCamp
{
    GOD,CAVE,OTHER
}
