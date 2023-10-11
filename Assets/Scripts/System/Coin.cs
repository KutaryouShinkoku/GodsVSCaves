using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin:MonoBehaviour
{
    public int coinAmount;
    public int borrowTimes = 0;
    public List<string> unlockedHeros = new List<string>();
    public void Start()
    {
        LoadCoin();
    }
    public void SaveCoin()
    {
        SaveSystem.SaveCoin(this);
    }
    public void LoadCoin()
    {
        CoinData data = SaveSystem.LoadCoin();
        coinAmount = data.coinAmount;
        borrowTimes = data.borrowTimes;
        unlockedHeros = data.unlockedHeros;
    }


}
