using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBorrowMoney : MonoBehaviour
{
    [SerializeField] GameObject uiAlert;
    [SerializeField] Coin coinStorer;
    [SerializeField] GameObject achievement;//TODO:成就系统的临时方案，以后重写
    [SerializeField] GameObject tipBox;
    [SerializeField] private AudioSource clickSE;

    public void BorrowCoins()
    {
        
        if(coinStorer.borrowTimes == 0)
        {
            uiAlert.SetActive(true);
        }
        if (coinStorer.coinAmount < 500 + 100 * coinStorer.loseTimes)
        {
            coinStorer.coinAmount = 500+100*coinStorer.loseTimes;
            clickSE.Play();
            coinStorer.borrowTimes++;
            coinStorer.SaveCoin();
            if (coinStorer.borrowTimes == 100)
            {
                ShowAchievement();
            }
        }
    }

    public void CloseAlerts()
    {
        uiAlert.SetActive(false);
    }

    public void ShowAchievement()
    {
        achievement.SetActive(true);
    }
    public void CloseAchievement()
    {
        achievement.SetActive(false);
    }

    public void OnMouseEnter()
    {
        tipBox.SetActive(true);
    }

    public void OnMouseExit()
    {
        tipBox.SetActive(false);
    }

}
