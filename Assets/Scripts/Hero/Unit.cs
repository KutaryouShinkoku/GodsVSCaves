using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] HeroBase _base;
    [SerializeField] int level;
    //[SerializeField] bool isPlayer1; 备用，用来检查角色属于哪边

    public Hero Hero { get; set; }

    public void Setup()
    {
        Hero = new Hero(_base, level);
        //Debug.Log(Hero .Base.Name + "的初始血量攻防:" + Hero.Base.MaxHP + " " + Hero.Base.Attack + " " + Hero.Base.Defence);
        GetComponent<SpriteRenderer>().sprite = Hero.Base.Sprite;
        //Debug.Log("Sprite name:" );
    }
}
