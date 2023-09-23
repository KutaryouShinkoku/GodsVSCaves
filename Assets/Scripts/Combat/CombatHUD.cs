using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text dmgText;
    [SerializeField] Slider hPSlider;
    [SerializeField] GameObject turnArrow;
    //[SerializeField Slider EXPSlider;

    Hero _hero;

    //HUD初始化
    public void SetHUD(Hero hero)
    {
        _hero = hero;

        nameText.text = hero.Base.name;
        levelText.text = "LV" + hero.Level;
        dmgText.text = "";
        hPSlider.maxValue = hero.MaxHP;
        hPSlider.value = hero.MaxHP;
        turnArrow.SetActive(false);
    }

    //显示血量
    public IEnumerator UpdateHp()
    {
        yield return SetHpSmooth (_hero .HP);
    }

    //血量降低
    public IEnumerator SetHpSmooth(float newHp)
    {
        float curHp = hPSlider.value;
        float changeAmt = curHp - newHp;

        while(curHp-newHp>Mathf .Epsilon)
        {
            curHp -= 4*changeAmt * Time.deltaTime;
            hPSlider.value = curHp ;
            yield return null;
        }

        hPSlider.value = newHp;
    }

    //回合标识箭头

    public IEnumerator SetArrow()
    {
        turnArrow.SetActive(true);
        yield return null;
    }
    public IEnumerator HideArrow()
    {
        turnArrow.SetActive(false);
        yield return null;
    }


    //显示伤害
    public IEnumerator ShowDamage(int damage,bool crit)
    {
        dmgText.color = Color.red;
        if (crit)
        {
            dmgText.color = Color.yellow;
        }
        if(damage != 0)
        {
            dmgText.text = "-" + damage;
            yield return new WaitForSeconds(0.5f);
        }
        dmgText.text = "";
    }

}
