using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBet : MonoBehaviour
{
    public int p1Coin;
    public int p2Coin;
    public float p1Odd;
    public float p2Odd;

    [Header("UI")]
    [SerializeField] Text odds;
    [SerializeField] Text p1CoinText;
    [SerializeField] Text p2CoinText;
    [SerializeField] Text p1Bet;
    [SerializeField] Text p2Bet;
    [SerializeField] Text timer;
    [SerializeField] Text timerTitle;
    [SerializeField] GameObject uiTimer;
    [SerializeField] GameObject btnP1GO;
    [SerializeField] GameObject btnP2GO;
    [SerializeField] Coin coinsStorer;
    private int bet;

    private float ftime;
    private bool isTimerActive;
    private float betTimer;
    private int countdown;

    void Start()
    {
        ResetBetUI();
    }

    void Update()
    {
        UpdateOddsAndCoins();

        if (isTimerActive == true)
        {
            ftime += Time.deltaTime;
            betTimer += Time.deltaTime;
        }
        if(ftime >= 1.2f)
        {
            CalculateOdds();
            ftime = 0f;
        }
        if(betTimer >= 1f)
        {
            countdown -= 1;
            if(countdown <= 5)
            {
                timer.text = $"{countdown }";
                timer.color = Color.red;
            }
            else { timer.text = $"{countdown }"; }
            betTimer = 0;
        }
        if(countdown == 0)
        {
            DisableBet();
        }
    }

    public void OnBetP1()
    {
        if(coinsStorer.coinAmount >= 1)
        {
            coinsStorer.coinAmount -= bet;
            p1Coin += bet;
        }
    }

    public void OnBetP2()
    {
        if (coinsStorer.coinAmount >= 1)
        {
            coinsStorer.coinAmount -= bet;
            p2Coin += bet;
        }
    }
    //更新下注和金币数
    public void UpdateOddsAndCoins()
    {
        odds.text = $"{p1Odd:N2} : {p2Odd:N2}";
        p1CoinText.text = $"{p1Coin}";
        p2CoinText.text = $"{p2Coin}";
        bet = coinsStorer.coinAmount / 2;
        p1Bet.text = $"{bet}";
        p2Bet.text = $"{bet}";
    }

    public void CalculateOdds()
    {
        float modifier = Random.Range(-0.2f, 0.2f);
        p1Odd = Mathf.Clamp (p1Odd + modifier,0.2f,5f);
        p2Odd = 1f / p1Odd;
    }

    public void ResetBetUI()
    {
        uiTimer.SetActive(true);
        countdown = 20;
        timerTitle.color = Color.white;
        timerTitle.text = $"{Localize.GetInstance().GetTextByKey("Countdown to placing a bet")}";
        timer.text = $"{countdown }";
        btnP1GO.SetActive(true);
        btnP2GO.SetActive(true);
        isTimerActive = true;
        p1Coin = 0;
        p2Coin = 0;
        p1Odd = 1f;
        p2Odd = 1f;
    }

    public void DisableBet()
    {
        uiTimer.SetActive(false);
        timerTitle.color = Color.red;
        timerTitle.text = $"\n{Localize.GetInstance().GetTextByKey("Betting ends!")}";
        timer.text = $"";
        btnP1GO.SetActive(false);
        btnP2GO.SetActive(false);
        isTimerActive = false ;
    }
}
