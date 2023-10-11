using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroUnlock : MonoBehaviour
{
    public bool isUnlock;
    public int price;
    [SerializeField] Text priceText;
    [SerializeField] Coin coinStorer;
    private Color originalColor;
    void Start()
    {
        List<string> list = new List<string>();
        list = coinStorer.unlockedHeros;
        Debug.Log(list);
        if (list.Exists(t => t== $"{gameObject.name}"))
        {
            isUnlock = true;
        }
        if (isUnlock)
        {
            gameObject.SetActive(false);
        }
        priceText.text = $"{price}";
        originalColor = priceText.color;
    }

    public void Unlock()
    {
        if (coinStorer.coinAmount >= price)
        {
            coinStorer.coinAmount -= price;
            coinStorer.unlockedHeros.Add($"{gameObject.name}");
            gameObject.SetActive(false);
            coinStorer.SaveCoin();
        }
        else
        {
            priceText.color = Color.red;
            Invoke("UIReset", 1);
        }
    }

    public void UIReset()
    {
        priceText.color = originalColor;
    }
}
