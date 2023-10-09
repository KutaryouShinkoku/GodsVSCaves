using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStorer : MonoBehaviour
{
    public int coinAmount;

    public void CoinUp(int up)
    {
        coinAmount += up;
    }
}
