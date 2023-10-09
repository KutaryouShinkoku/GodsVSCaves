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
    [SerializeField] CoinStorer coin;

    public GameState state;

    //初始化：选择界面
    private void Start()
    {
        if (PlayerPrefs.HasKey("Coin"))
        {
            coin .coinAmount = PlayerPrefs.GetInt("Coin");
        }
        //coin.LoadCoin();
        state = GameState.SELECT;
        combatSystem.gameObject.SetActive(false);
        combatCamera.gameObject.SetActive(false);
        selectorCamera.gameObject.SetActive(true);
    }

    //切换到战斗界面
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

    //战斗结束，回到选择界面
    public IEnumerator CombatEnd()
    {
        state = GameState.SELECT;
        combatSystem.gameObject.SetActive(false);
        combatCamera.gameObject.SetActive(false);
        selectorCamera.gameObject.SetActive(true);
        //coin.SaveCoin();
        PlayerPrefs.SetInt("Coin", coin.coinAmount);
        yield return null;
    }
}
