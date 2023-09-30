using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInSlot : MonoBehaviour
{
    [HideInInspector] public Hero hero;

    public Image image;
    public HeroSelector heroSelector;

    void Start()
    {
        InitialiseHero(hero, heroSelector);
    }

    public void InitialiseHero(Hero newHero,HeroSelector selector)
    {
        hero = newHero;
        image.sprite = newHero.Sprite;
        heroSelector = selector;
    }

    //按钮事件：点击英雄图标选择英雄
    public void SelectHero()
    {
        heroSelector.UpdateHeroSelection(hero);
    }
}
