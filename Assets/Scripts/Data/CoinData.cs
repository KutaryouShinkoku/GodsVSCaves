using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoinData
{
    public int coinAmount;
    public int borrowTimes;
    public List<string> unlockedHeros = new List<string>();

    public CoinData(Coin coin)
    {
        coinAmount = coin.coinAmount;
        borrowTimes = coin.borrowTimes;
        unlockedHeros = new List<string>();
        unlockedHeros = coin.unlockedHeros;
    }
}
