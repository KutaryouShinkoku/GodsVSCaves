using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStorer : MonoBehaviour
{
    public int coinAmount;
    public Coin coin = new Coin();
    public void Start()
    {
        coin.LoadCoin();
    }
    public void Update()
    {
        coinAmount = coin.coinAmount;
        coin.SaveCoin();
    }
    public void CoinUp(int up)
    {
        coinAmount += up;
    }
}
