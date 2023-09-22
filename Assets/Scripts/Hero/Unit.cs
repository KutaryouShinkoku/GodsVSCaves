using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] HeroBase _base;
    [SerializeField] int level;
    //[SerializeField] bool isPlayer1; ���ã���������ɫ�����ı�

    public Hero Hero { get; set; }

    public void Setup()
    {
        Hero = new Hero(_base, level);
        //Debug.Log(Hero .Base.Name + "�ĳ�ʼѪ������:" + Hero.Base.MaxHP + " " + Hero.Base.Attack + " " + Hero.Base.Defence);
        GetComponent<SpriteRenderer>().sprite = Hero.Base.Sprite;
        //Debug.Log("Sprite name:" );
    }
}
