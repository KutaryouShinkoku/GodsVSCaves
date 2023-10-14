using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum SelectState { NONE,SELECTP1,SELECTP2,READY,ERROR}

public class HeroSelector : MonoBehaviour
{
    [SerializeField] Hero p1Hero;
    [SerializeField] Hero p2Hero;

    [SerializeField] Text guide;
    [SerializeField] List<Hero> heros;

    [Header("UI")]
    public HeroSlots[] heroSlots;
    public GameObject heroPrefab;
    public HeroSelector heroSelector;
    public Image p1Showcase;
    public Image p2Showcase;
    public Text p1Status;
    public Text p2Status;
    public Text p1Name;
    public Text p2Name;
    public GameObject selectMask;

    [Header("Audio")]
    [SerializeField] private AudioSource clickSE;

    [HideInInspector] public SelectState state;

    //-----------------------------��ʼ��-----------------------------
    private void Start()
    {
        //Debug.Log("Static Hero Updated!!P1:" + p1Hero.Base.name + "||P2:" + p2Hero.Base.name);
        //��ʼ��ѡ��״̬
        state = SelectState.NONE;

        //��ʼ��ѡ�˽���
        foreach (var hero in heros)
        {
            AddHero(hero);
            Debug.Log("Add" + hero.Base.name);
        }
        //Ĭ��Ӣ��
        p1Hero = heros[0];
        p2Hero = heros[1];

        //��ʼ����ɫ������ֵ
        UpdateSelectedHeroInfoP1();
        UpdateSelectedHeroInfoP2();

        //�������ѡ������Գ�ʼ��������
        guide.text =$"{Localize.GetInstance().GetTextByKey($"Select Heros")}!";

        //��ʼ������UI
        selectMask.SetActive(false);

    }
    private void Update()
    {
        UpdateSelectedHeroInfoP1();
        UpdateSelectedHeroInfoP2();

        if (Input.GetMouseButtonDown(1))
        {
            selectMask.SetActive(false);
            state = SelectState.NONE;
        }
    }

    //-----------------------------����ѡ�˽���-----------------------------
    //��slot��ʼ����ѡ�˽���
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
    //-----------------------------ѡ����ؽ���-----------------------------
    //�л�Ӣ�۰�ť
    public void ChangeP1()
    {
        state = SelectState.SELECTP1;
        selectMask.SetActive(true);
        guide.text = $"{Localize.GetInstance().GetTextByKey($"Select Hero for Gods")}";
        clickSE.Play();
    }

    public void ChangeP2()
    {
        state = SelectState.SELECTP2;
        selectMask.SetActive(true);
        guide.text = $"{Localize.GetInstance().GetTextByKey($"Select Hero for Caves")}";
        clickSE.Play();
    }

    //ѡ�˺����Ӣ����Ϣ
    public void UpdateHeroSelection(Hero hero)
    {
        if (state == SelectState.SELECTP1)
        {
            if(hero == p2Hero)
            {
                guide.text = $"{Localize.GetInstance().GetTextByKey($"Unable to select the same hero to fight against!")}";
                state = SelectState.ERROR;
                Invoke("ChangeP1", 1);
            }
            else
            {
                p1Hero = hero;
                selectMask.SetActive(false);
                UpdateSelectedHeroInfoP1();
                state = SelectState.NONE;
                guide.text = $"{Localize.GetInstance().GetTextByKey($"Select Heros")}!";
                Debug.Log("Set1Hero = " + p1Hero.Base.HeroName);
            }
        }
        else if (state == SelectState.SELECTP2)
        {
            if (hero == p1Hero)
            {
                guide.text = $"{Localize.GetInstance().GetTextByKey($"Unable to select the same hero to fight against!")}";
                state = SelectState.ERROR;
                Invoke("ChangeP2", 1);
            }
            else
            {
                p2Hero = hero;
                selectMask.SetActive(false);
                UpdateSelectedHeroInfoP2();
                state = SelectState.NONE;
                guide.text = $"{Localize.GetInstance().GetTextByKey($"Select Heros")}!";
                Debug.Log("Set1Hero = " + p2Hero.Base.HeroName);
            }
        }
        Debug.Log("Hero Selection Updated!P1:" + p1Hero.Base.HeroName + "Hero Selection Updated!P2:" + p2Hero.Base.HeroName);
    }

    //����Ӣ�۶�սԤ�������������
    public void UpdateSelectedHeroInfoP1()
    {
        //����
        p1Showcase.sprite = p1Hero.Base.Sprite;
        //����
        p1Name.text = p1Hero.Base.HeroName;
        //��ֵ
        p1Status.text = p1Hero.Level + "\n" + p1Hero.Base.MaxHP + "\n" + p1Hero.Base.Attack + "\n" + p1Hero.Base.Defence + "\n" + p1Hero.Base.Magic + "\n" + p1Hero.Base.MagicDef + "\n" + p1Hero.Base.Speed + "\n" + p1Hero.Base.Luck + "%";
    }
    public void UpdateSelectedHeroInfoP2()
    {
        //����
        p2Showcase.sprite = p2Hero.Base.Sprite;
        //����
        p2Name.text = p2Hero.Base.HeroName;
        //��ֵ
        p2Status.text = p2Hero.Level + "\n" + p2Hero.Base.MaxHP + "\n" + p2Hero.Base.Attack + "\n" + p2Hero.Base.Defence + "\n" + p2Hero.Base.Magic + "\n" + p2Hero.Base.MagicDef + "\n" + p2Hero.Base.Speed + "\n" + p2Hero.Base.Luck + "%";
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
