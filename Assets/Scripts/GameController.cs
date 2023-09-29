using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState {SELECT,COMBAT}


public class GameController : MonoBehaviour
{
    [SerializeField] HeroSelector heroSelector;
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] Camera selectorCamera;
    [SerializeField] Camera combatCamera;

    public GameState state;

    private void Start()
    {
        combatSystem.gameObject.SetActive(false);
        combatCamera.gameObject.SetActive(false);
        selectorCamera.gameObject.SetActive(true);
    }

    public void CombatStart()
    {
        state = GameState.COMBAT;
        combatSystem.gameObject.SetActive(true);
        combatCamera.gameObject.SetActive(true);
        selectorCamera.gameObject.SetActive(false);

        //把英雄选择界面的数据拿进来
        var p1Hero = heroSelector.GetComponent<HeroSelector>().GetP1Hero();
        var p2Hero = heroSelector.GetComponent<HeroSelector>().GetP2Hero();

        //把英雄数据放进战斗系统
        combatSystem.HandleCombatStart(p1Hero ,p2Hero );
    }

    public IEnumerator CombatEnd()
    {
        state = GameState.SELECT;
        combatSystem.gameObject.SetActive(false);
        combatCamera.gameObject.SetActive(false);
        selectorCamera.gameObject.SetActive(true);
        yield return null;
    }
}
