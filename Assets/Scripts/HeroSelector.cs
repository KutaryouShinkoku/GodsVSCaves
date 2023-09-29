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


    public HeroSlots[] heroSlots;
    public GameObject heroPrefab;
    public HeroSelector heroSelector;
    public Image p1Showcase;
    public Image p2Showcase;
    public Text p1Status;
    public Text p2Status;
    public Text p1Name;
    public Text p2Name;

    public SelectState state;

    private void Start()
    {

        Debug.Log("Static Hero Updated!!P1:" + p1Hero.Base.name + "||P2:" + p2Hero.Base.name);
        state = SelectState.NONE;

        guide.text = "Select Heros!";

        //��Ӣ�۷ŵ�ѡ�˽���
        foreach (var hero in heros)
        {
            AddHero(hero);
            Debug.Log("Add" + hero.Base.name);
        }
    }



    //�ѵ���Ӣ����Ϣ��ʼ����ѡ�˽���
    public void AddHero(Hero hero)
    {
        for(int i = 0; i < heroSlots.Length; i++)
        {
            HeroSlots slot = heroSlots[i];
            HeroInSlot heroInSlot = slot.GetComponentInChildren<HeroInSlot>();
            if(heroInSlot == null)
            {
                SpawnNewHero(hero, slot);
                return;
            }
        }
    }
    //��slot������Ӣ����Ϣ
    void SpawnNewHero(Hero hero, HeroSlots slot)
    {
        GameObject newHeroGO = Instantiate(heroPrefab, slot.transform);
        HeroInSlot heroInSlot = newHeroGO.GetComponent<HeroInSlot>();
        heroInSlot.InitialiseHero(hero,heroSelector); 
    }

    //ѡ�˰�ť
    public void ChangeP1()
    {
        state = SelectState.SELECTP1;
        Debug.Log("state=" + state);
        guide.text = "Select Hero for Gods";
    }

    public void ChangeP2()
    {
        state = SelectState.SELECTP2;
        Debug.Log("state=" + state);
        guide.text = "Select Hero for Caves";
    }

    //ѡ�˺����Ӣ����Ϣ
    public void UpdateHeroSelection(Hero hero)
    {
        if (state == SelectState.SELECTP1)
        {
            p1Hero = hero;
            UpdateSelectedHeroInfoP1();
            state = SelectState.NONE;
            Debug.Log("Set1Hero = " + p1Hero.Base.name);
        }
        else if (state == SelectState.SELECTP2)
        {
            p2Hero = hero;
            UpdateSelectedHeroInfoP2();
            state = SelectState.NONE;
            Debug.Log("Set1Hero = " + p2Hero.Base.name);
        }
        Debug.Log("Hero Selection Updated!P1:" + p1Hero.Base.name + "Hero Selection Updated!P2:" + p2Hero.Base.name);
    }

    //����Ӣ�۶�սԤ�������������
    public void UpdateSelectedHeroInfoP1()
    {
        //����
        p1Showcase.sprite = p1Hero.Base.Sprite;
        //����
        p1Name.text = p1Hero.Base.name;
        //��ֵ
        p1Status.text = p1Hero.Level + "\n" + p1Hero.Base.MaxHP + "\n" + p1Hero.Base.Attack + "\n" + p1Hero.Base.Defence + "\n" + p1Hero.Base.Magic + "\n" + p1Hero.Base.MagicDef + "\n" + p1Hero.Base.Speed + "\n" + p1Hero.Base.Evasion + "%";
    }
    public void UpdateSelectedHeroInfoP2()
    {
        //����
        p2Showcase.sprite = p2Hero.Base.Sprite;
        //����
        p2Name.text = p2Hero.Base.name;
        //��ֵ
        p2Status.text = p2Hero.Level + "\n" + p2Hero.Base.MaxHP + "\n" + p2Hero.Base.Attack + "\n" + p2Hero.Base.Defence + "\n" + p2Hero.Base.Magic + "\n" + p2Hero.Base.MagicDef + "\n" + p2Hero.Base.Speed + "\n" + p2Hero.Base.Evasion + "%";
    }


    //����ѡ�˽���ѡ����Ӣ����Ϣ
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
