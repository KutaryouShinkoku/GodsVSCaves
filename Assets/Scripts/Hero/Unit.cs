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
        Debug.Log("Status:" + Hero.Base.MaxHP + " " + Hero.Base.Attack + " " + Hero.Base.Defence);
        GetComponent<SpriteRenderer>().sprite = Hero.Base.Sprite;
        Debug.Log("Sprite name:" );
    }


    //public string unitName;
    //public int unitLevel;

    //public int damage;//临时测试用，后面删掉
    //public int atk;
    //public int magic;
    //public int def;
    //public int magicDef;
    //public int evasion;

    //public int maxHP;
    //public int currentHP;
    //public int maxMP;
    //public int currentMP;

    //public int speed;

    //TakeDamage 受伤逻辑以及死亡判定
    //public bool TakeDamage(int dmg)
    //{
    //    currentHP -= dmg;

    //    if(currentHP <= 0)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
