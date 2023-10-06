using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Localize m_localize;

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
