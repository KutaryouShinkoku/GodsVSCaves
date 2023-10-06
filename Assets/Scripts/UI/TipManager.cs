using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipManager : MonoBehaviour
{
    public static TipManager _instance;
    public Camera selectorCam;
    public GameObject tipGO;

    public Text heroNameText;
    public Text heroDiscText;
    public Image heroPortrait;

    private Vector2 screenPoint;
    private Vector2 uiPoint;
    private RectTransform uiRT;
    private Vector3 worldPoint;

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

        uiRT = tipGO.GetComponent<RectTransform>();
    }

    public void SetAndShowTip(string name,string desc,Sprite sprite)
    {
        gameObject.SetActive(true);
        heroNameText.text = name;
        heroDiscText.text = desc.Replace("\\n","\n");
        heroPortrait.sprite = sprite;
    }

    public void HideTip()
    {
        gameObject.SetActive(false);
        heroNameText.text = "";
        heroDiscText.text = "";
    }
    
    //×ø±ê×ª»»
    
}
