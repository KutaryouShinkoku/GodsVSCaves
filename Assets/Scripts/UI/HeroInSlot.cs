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

    //��ť�¼������Ӣ��ͼ��ѡ��Ӣ��
    public void SelectHero()
    {
        heroSelector.UpdateHeroSelection(hero);
    }
}
