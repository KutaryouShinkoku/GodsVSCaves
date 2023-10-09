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
    public Text p1OddText;
    public Text p2OddText;
    public Text p1CoinText;
    public Text p2CoinText;
    public Text p1Bet;
    public Text p2Bet;
    public Text timer;
    public GameObject btnP1GO;
    public GameObject btnP2GO;
    [SerializeField] CoinStorer coin;
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
        if(coin.coinAmount >= 2)
        {
            coin.coinAmount -= bet;
            p1Coin += bet;
        }
    }

    public void OnBetP2()
    {
        if (coin.coinAmount >= 2)
        {
            coin.coinAmount -= bet;
            p2Coin += bet;
        }
    }
    //更新下注和金币数
    public void UpdateOddsAndCoins()
    {
        p1OddText.text = $"{Localize.GetInstance().GetTextByKey("Odds:")} {string .Format("{0:N2}", p1Odd)}";
        p2OddText.text = $"{Localize.GetInstance().GetTextByKey("Odds:")} {string .Format("{0:N2}", p2Odd)}";
        p1CoinText.text = $"{p1Coin}";
        p2CoinText.text = $"{p2Coin}";
        bet = coin.coinAmount / 2;
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
        countdown = 20;
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
        timer.text = $"";
        btnP1GO.SetActive(false);
        btnP2GO.SetActive(false);
        isTimerActive = false ;
    }
}
