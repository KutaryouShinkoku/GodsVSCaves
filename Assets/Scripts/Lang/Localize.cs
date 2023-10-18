using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Localize
{
    private static Localize m_instance;
    private Dictionary<string, string> m_dic_lt;
    private List<LocalizeTxt> m_list_lt;
    private LanguageList m_currentLanguage;

    public static Localize GetInstance()
    {
        if(m_instance == null)
        {
            m_instance = new Localize();
        }
        return m_instance;
    }

    public enum LanguageList
    {
        en,
        cn
    }

    Localize()
    {
        m_dic_lt = new Dictionary<string, string>();
        m_list_lt = new List<LocalizeTxt>();
        m_currentLanguage = LanguageList.en;
        LoadLanguage();
        OnLanguageChanged();
    }

    public void RegisterLT(LocalizeTxt elt)
    {
        m_list_lt.Add(elt);
    }

    public void UnregisterLT(LocalizeTxt elt)
    {
        m_list_lt.Remove(elt);
    }

    public void ChangeLanguage(LanguageList list)
    {
        if (m_currentLanguage == list) return;
        m_currentLanguage = list;
        m_dic_lt.Clear();
        LoadLanguage();
        OnLanguageChanged();
    }

    public void OnLanguageChanged()
    {
        foreach(var lt in m_list_lt)
        {
            lt.OnLanguageChanged();
        }
    }

    public string GetTextByKey(string key)
    {
        return m_dic_lt[key];
    }

    public void LoadLanguage()
    {
        switch (m_currentLanguage)
        {
            case LanguageList.en:
                {
                    LoadLanguageFile("Lang/en_system");
                    LoadLanguageFile("Lang/en_hero");
                    LoadLanguageFile("Lang/en_move");
                    break;
                }
            case LanguageList.cn:
                {
                    LoadLanguageFile("Lang/cn_system");
                    LoadLanguageFile("Lang/cn_hero");
                    LoadLanguageFile("Lang/cn_move");
                    break;
                }
        }
    }
    public void LoadLanguageFile(string filename)
    {
        TextAsset asset = Resources.Load(filename) as TextAsset;
        Stream st = new MemoryStream(asset.bytes);
        StreamReader sr = new StreamReader(st);
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            string[] tempStrings = line.Split('$');
            m_dic_lt[tempStrings[0]] = tempStrings[1];
            //Debug.Log(line);
        }
    }
}
