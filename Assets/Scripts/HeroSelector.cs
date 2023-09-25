using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum SelectState { NONE,SELECTP1,SELECTP2,READY}

public class HeroSelector : MonoBehaviour
{
    [SerializeField] Hero p1Hero;
    [SerializeField] Hero p2Hero;
    [SerializeField] Text guide;
    [SerializeField] List<Hero> heros;

    public SelectState state;
    //public event Action OnCombatStart;
    private void Start()
    {
        state = SelectState.NONE;
        guide.text = "Select Heros!";

        //foreach (var hero in heros)
        //{
        //    hero.Init();
        //}
    }


    public void ChangeP1()
    {
        state = SelectState.SELECTP1;
        guide.text = "Select Hero for Gods";
    }

    public void ChangeP2()
    {
        state = SelectState.SELECTP2;
        guide.text = "Select Hero for Caves";
    }

    public void Select()
    {
        if (state == SelectState.SELECTP1)
        {
            p1Hero = GetComponent<HeroSelectButton>().hero;
            Debug.Log("Set1");
        }
        else if (state == SelectState.SELECTP2)
        {
            p2Hero = GetComponent<HeroSelectButton>().hero;
            Debug.Log("Set2");
        }
    }

    public Hero GetP1Hero()
    {
        p1Hero.Init();
        return p1Hero;
    }

    public Hero GetP2Hero()
    {
        p2Hero.Init();
        return p2Hero;
    }

}
