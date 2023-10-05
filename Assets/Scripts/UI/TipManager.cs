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

    void Update()
    {
        //世界转屏幕
        worldPoint = Input.mousePosition;
        Debug.Log($"世界坐标：{worldPoint}");
        screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        Debug.Log($"屏幕坐标：{screenPoint}");

        //屏幕转UI
        Debug.Log($"当前相机：{selectorCam}");
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiRT, screenPoint, selectorCam, out uiPoint);
        Debug.Log($"UI坐标：{uiPoint}");

        tipGO.transform.position = uiPoint;
    }

    public void SetAndShowTip(string name,string desc)
    {
        gameObject.SetActive(true);
        heroNameText.text = name;
        heroDiscText.text = desc;
    }

    public void HideTip()
    {
        gameObject.SetActive(false);
        heroNameText.text = "";
        heroDiscText.text = "";
    }
    
    //坐标转换
    
}
