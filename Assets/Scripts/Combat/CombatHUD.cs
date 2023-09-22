using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text dmgText;
    [SerializeField] Slider HPSlider;
    //[SerializeField Slider EXPSlider;

    Hero _hero;
    public void SetHUD(Hero hero)
    {
        _hero = hero;

        nameText.text = hero.Base.name;
        levelText.text = "LV" + hero.Level;
        dmgText.text = "";
        HPSlider.maxValue = hero.MaxHP;
        HPSlider.value = hero.MaxHP;
    }

    public void UpdateHp()
    {
        HPSlider.value = _hero.HP;
    }

    public void ShowDamage()
    {
        //œ‘ æ…À∫¶÷µ
    }

}
