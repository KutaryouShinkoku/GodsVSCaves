using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeed : MonoBehaviour
{
    public static float timeScale;
    private bool isPause = false;
    [SerializeField] private Text pauseOrContinueText;

    public void Start()
    {
        timeScale = 1;
    }
    public void SetSpeedhx()
    {
        Time.timeScale = 0.5f;
        timeScale = 0.5f;
        CancelPause();
    }
    public void SetSpeed1x()
    {
        Time.timeScale = 1;
        timeScale = 1;
        CancelPause();
    }
    public void SetSpeed2x()
    {
        Time.timeScale = 2;
        timeScale = 2;
        CancelPause();
    }
    public void SetSpeed4x()
    {
        Time.timeScale = 4;
        timeScale = 4;
        CancelPause();
    }
    public void SetSpeedPause()
    {
        if (isPause == false)
        {
            Time.timeScale = 0;
            timeScale = 0;
            pauseOrContinueText.text = "G";
            isPause = true;
        }
        else
        {
            Time.timeScale = 1;
            timeScale = 1;
            CancelPause();
        }
    }
    //从显示上解除暂停状态
    void CancelPause() 
    {
        pauseOrContinueText.text = "II";
        isPause = false;
    }
}
