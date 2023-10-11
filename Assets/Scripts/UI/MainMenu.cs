using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string combat = "Combat";
    private Localize m_localize;

    private void Awake()
    {
        m_localize = Localize.GetInstance();
    }
    public void StartEN()
    {
        m_localize.ChangeLanguage(Localize.LanguageList.en);
        SceneManager.LoadScene(combat);
    }

    public void StartCN()
    {
        m_localize.ChangeLanguage(Localize.LanguageList.cn);
        SceneManager.LoadScene(combat);
    }
}
