using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin
{
    public int coinAmount;
    public void SaveCoin()
    {
        SaveSystem.SaveCoin(this);
    }
    public void LoadCoin()
    {
        CoinData data = SaveSystem.LoadCoin();

        coinAmount = data.coinData;
    }
}
