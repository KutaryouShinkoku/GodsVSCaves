using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Hero/Create a new move")]


public class MoveBase : ScriptableObject 
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool isMagic;
    [SerializeField] int moveActionType; //1����ս| 2��Զ��| 3������| 4������

    public string Name{
        get { return name; }
    }

    public string Description{
        get { return description; }
    }

    public int Power{
        get { return power; }
    }

    public int Accuracy{
        get { return accuracy; }
    }

    public bool IsMagic{
        get { return isMagic; }
    }

    public int MoveActionType{
        get { return moveActionType; }
    }
}
