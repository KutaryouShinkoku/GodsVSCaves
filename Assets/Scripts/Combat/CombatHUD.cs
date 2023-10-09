using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CombatHUD : MonoBehaviour
{
    [SerializeField] GameObject combatHUD;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text dmgText;
    [SerializeField] Slider hPSlider;
    [SerializeField] Image hPColor;
    [SerializeField] GameObject turnArrow;
    //[SerializeField Slider EXPSlider;

    Hero _hero;
    Vector3 originalDmgPos;

    //HUD��ʼ��
    public void SetHUD(Hero hero)
    {
        combatHUD.SetActive(true); //��ʾHUD

        //Ӣ�����ݳ�ʼ��
        _hero = hero; 
        nameText.text = hero.Base.HeroName;
        levelText.text = "LV" + hero.Level;

        //Ѫ����ʼ��
        hPSlider.maxValue = hero.MaxHP;
        hPSlider.value = hero.MaxHP;
        hPColor.color = new Color(0.20f, 0.87f, 0.24f, 1f);

        turnArrow.SetActive(false); //�غϱ�ǳ�ʼ��

        //�˺���ֵ������ʼ��
        dmgText.text = "";
        originalDmgPos = dmgText .transform.localPosition;
    }

    //��ʾѪ��
    public IEnumerator UpdateHp()
    {
        if (_hero.HpChanged)
        {
            _hero.HpChanged = false;
            yield return SetHpSmooth(_hero.HP);
        }
    }

    //�����쳣״̬����Ѫ����ɫ
    public IEnumerator UpdateHpBarColor()
    {
        if(_hero.Status == ConditionsDB.Conditions[ConditionID.psn])
        {
            hPColor.color = new Color(0.66f, 0.12f, 0.61f, 1f);
        }
        yield return null;
    }

    //Ѫ������
    public IEnumerator SetHpSmooth(float newHp)
    {
        float curHp = hPSlider.value;
        float changeAmt = curHp - newHp;
        if(changeAmt >= 0)
        {
            while (curHp - newHp > Mathf.Epsilon)
            {
                curHp -= 4 * changeAmt * Time.deltaTime;
                hPSlider.value = curHp;
                yield return null;
            }
        }
        else
        {
            while (newHp - curHp > Mathf.Epsilon)
            {
                curHp -= 4 * changeAmt * Time.deltaTime;
                hPSlider.value = curHp;
                yield return null;
            }
        }


        hPSlider.value = newHp;
    }

    //�غϱ�ʶ��ͷ

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


    //��ʾ�˺�
    public IEnumerator ShowDamage(int damage,bool crit,bool isMagic)
    {
        var sequence = DOTween.Sequence();

        dmgText.color = Color.red;
        if(isMagic)
        {
            dmgText.color = Color.blue ;
        }
        if (crit)
        {
            dmgText.color = Color.yellow;
        }
        if(damage != 0)
        {
            dmgText.text = "-" + damage;
            yield return sequence.Append(dmgText .transform.DOLocalMoveY(originalDmgPos.y + 20f, 0.8f));
            yield return sequence.Join(dmgText.DOFade(0, 0.8f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    //�����˺�
    public IEnumerator HideDamage()
    {
        var sequence = DOTween.Sequence();

        dmgText.text = "";
        sequence.Append(dmgText.transform.DOLocalMoveY(originalDmgPos.y, 0.1f));

        yield return null;
    }

}
