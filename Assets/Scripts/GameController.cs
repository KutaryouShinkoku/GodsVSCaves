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

    //��ʼ����ѡ�����
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

    //�л���ս������
    public void CombatStart()
    {
        state = GameState.COMBAT;
        combatSystem.gameObject.SetActive(true);
        combatCamera.gameObject.SetActive(true);
        selectorCamera.gameObject.SetActive(false);

        //��Ӣ��ѡ�����������ý���
        var p1Hero = heroSelector.GetComponent<HeroSelector>().GetP1Hero();
        var p2Hero = heroSelector.GetComponent<HeroSelector>().GetP2Hero();

        //��Ӣ�����ݷŽ�ս��ϵͳ
        combatSystem.HandleCombatStart(p1Hero ,p2Hero );
    }

    //ս���������ص�ѡ�����
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
