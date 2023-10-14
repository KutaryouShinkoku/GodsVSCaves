using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangManager : MonoBehaviour
{
    private Localize m_localize;
    [SerializeField] private AudioSource clickSE;

    private void Awake()
    {
        m_localize = Localize.GetInstance();
    }
    public void OnButtonCNClick()
    {
        m_localize.ChangeLanguage(Localize.LanguageList.cn);
        clickSE.Play();
    }

    public void OnButtonENClick()
    {
        m_localize.ChangeLanguage(Localize.LanguageList.en);
        clickSE.Play();
    }
}
