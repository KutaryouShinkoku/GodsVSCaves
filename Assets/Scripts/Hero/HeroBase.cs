using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Hero", menuName = "Hero/Create a new hero")]
public class HeroBase : ScriptableObject 
{
    [SerializeField] string heroName;
    [SerializeField] HeroCamp camp;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite sprite;
    [SerializeField] GameObject bullet;

    //Base stats 英雄属性
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int magic;
    [SerializeField] int magicDef;
    [SerializeField] int speed;
    [SerializeField] int evasion;
    [SerializeField] int luck;
    [SerializeField] Character character;
    [SerializeField] List<MoveOfDice> movesOfDice;

    //-----------------------------使属性可读-----------------------------
    public string HeroName{
        get{ return $"{Localize.GetInstance().GetTextByKey($"{heroName}")}";}
    }
    public HeroCamp HeroCamp{
        get { return camp; }
    }

    public string Description{
        get { return $"{Localize.GetInstance().GetTextByKey($"{description}")}"; }
    }

    public Sprite Sprite{
        get { return sprite; }
    }

    public GameObject Bullet{
        get { return bullet; }
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

    public int Luck{
        get { return luck; }
    }

    public List<MoveOfDice> MovesOfDice{
        get { return movesOfDice; }
    }

    public Character Character{
        get { return character ; }
    }
}

//Moves of Dice 骰子对应的技能
[System.Serializable]
public class MoveOfDice
{
    [SerializeField] MoveBase moveBase;

    public MoveBase Base{
        get { return moveBase; }
    }
}


//Camp 阵营
public enum HeroCamp
{
    GOD,CAVE,OTHER
}

//属性
public enum Stat {Attack,Defence,Magic,MagicDef,Speed,Luck}

//骰子性格
public enum Character
{
    Ordinary, //普通
    Brave, //勇敢
    Timid, //胆小
    Experienced, //老练
}
