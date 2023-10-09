using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoinData
{
    public int coinData;

    public CoinData(Coin coin)
    {
        coinData = coin.coinAmount;
    }
}
