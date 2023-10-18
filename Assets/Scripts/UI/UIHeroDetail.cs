using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroDetail : MonoBehaviour
{
    public GameObject detailGO;

    public Text heroNameText;
    public Text heroDiscText;
    public Image heroPortrait;
    public Text heroCharacter;
    public Text heroStats;
    // Start is called before the first frame update

    public void ShowHeroDetails(Hero hero)
    {
        heroNameText.text = hero.Base.HeroName;
        heroDiscText.text = hero.Base.Description.Replace("\\n", "\n");
        heroPortrait.sprite = hero.Base.Sprite;
        string characterLocalize = string.Format($"{Localize.GetInstance().GetTextByKey($"{hero.Base.Character}")}");
        heroCharacter.text = string.Format($"{Localize.GetInstance().GetTextByKey("[ Character of dice: {0} ]")}", characterLocalize);
        UpdateStats(hero);
    }

    public void UpdateStats(Hero hero)
    {
        Dictionary<Stat, int> stats = hero.Stats;
        string showStats = "";
        showStats += ($"{hero.Level}\n");
        showStats+=($"{ChangeHPDescColor(hero)}\n");
        foreach(var stat in stats)
        {
            int statVal = hero.Stats[stat.Key];
            int boost = hero.StatBoosts[stat.Key];
            showStats +=($"{ChangeStatColor(statVal,boost)}\n");
        }
        heroStats.text = $"{showStats}".Replace("\\n","\n");
    }
    public string ChangeStatColor(int statVal,int boost)
    {
        string curStat;
        if (boost > 0)
        {
            curStat = $"<color=\"#005C1D\">{statVal}</color>";
        }
        else if(boost == 0)
        {
            curStat = $"{statVal}";
        }
        else
        {
            curStat = $"<color=\"#FF5242\">{statVal}</color>";
        }
        return curStat;
    }
    public string ChangeHPDescColor(Hero hero)
    {
        string curHp;
        if (hero.HP >= hero.Base.MaxHP / 2)
        {
            curHp = $"<color=\"#005C1D\">{hero.HP}</color>";
        }
        else if(hero.HP >= hero.Base.MaxHP / 4)
        {
            curHp = $"<color=\"#E3DC00\">{hero.HP}</color>";
        }
        else
        {
            curHp = $"<color=\"#FF5242\">{hero.HP}</color>";
        }
        return curHp;
    }
}
