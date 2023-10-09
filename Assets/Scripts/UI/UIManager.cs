using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Localize m_localize;
    [SerializeField] CoinStorer coin;
    [SerializeField] Text coinAmount;

    private void Update()
    {
        coinAmount.text = "" + coin.coinAmount;
    }
    private void Awake()
    {
        m_localize = Localize.GetInstance();
    }
    public void OnButtonCNClick()
    {
        m_localize.ChangeLanguage(Localize.LanguageList.cn);
    }

    public void OnButtonENClick()
    {
        m_localize.ChangeLanguage(Localize.LanguageList.en);
    }
}
