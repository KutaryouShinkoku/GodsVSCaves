using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Slider HPSlider;
    //public Slider EXPSlider;

    public void SetHUD(Hero hero)
    {
        nameText.text = hero.Base.name;
        levelText.text = "LV" + hero.level;
        HPSlider.maxValue = hero.MaxHP;
        HPSlider.value = hero.HP;
    }

    public void SetHp(int hp)
    {
        HPSlider.value = hp;
    }

}
