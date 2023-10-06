using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeTxt : MonoBehaviour
{
    public string m_key;
    private Text m_text;

    public void Start()
    {
        m_text = GetComponent<Text>();

        Localize.GetInstance().RegisterLT(this);
    }

    public void OnLanguageChanged()
    {
        m_text.text = Localize.GetInstance().GetTextByKey(m_key);
    }
}
