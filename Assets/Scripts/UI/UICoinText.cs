using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoinText : MonoBehaviour
{
    [SerializeField] Coin coinStorer;
    Text coinText;
    private void Start()
    {
        coinText = GetComponent<Text>();
    }
    void Update()
    {
        UpdateCoin();
    }
    void UpdateCoin()
    {
        coinText.text = $"{coinStorer.coinAmount}";
    }
}
