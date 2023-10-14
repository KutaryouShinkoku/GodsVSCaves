using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipManager : MonoBehaviour
{
    public static TipManager _instance;
    public GameObject tipGO;

    public Text heroNameText;
    public Text heroDiscText;
    public Image heroPortrait;
    public Text heroCharacter;

    private void Awake()
    {
        if(_instance!=null&&_instance!=this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        Cursor.visible = true;
        tipGO.SetActive(false);
    }
    //-------------”¢–€–≈œ¢-------------
    public void SetAndShowTip(string name,string desc,Sprite sprite,Character character)
    {
        gameObject.SetActive(true);
        heroNameText.text = name;
        heroDiscText.text = desc.Replace("\\n","\n");
        heroPortrait.sprite = sprite;
        string characterLocalize = string .Format($"{Localize.GetInstance().GetTextByKey($"{character}")}");
        heroCharacter.text = string.Format($"{Localize.GetInstance().GetTextByKey("[ Character of dice: {0} ]")}", characterLocalize);
    }

    public void HideTip()
    {
        gameObject.SetActive(false);
        heroNameText.text = "";
        heroDiscText.text = "";
    }
}
