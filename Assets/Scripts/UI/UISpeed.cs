using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeed : MonoBehaviour
{
    private bool isPause = false;
    [SerializeField] private Text pauseOrContinueText;

    public void SetSpeed1x()
    {
        Time.timeScale = 1;
    }
    public void SetSpeed2x()
    {
        Time.timeScale = 2;
    }
    public void SetSpeed4x()
    {
        Time.timeScale = 4;
    }
    public void SetSpeedPause()
    {
        if (isPause == false)
        {
            Time.timeScale = 0;
            pauseOrContinueText.text = "G";
            isPause = true;
        }
        else
        {
            Time.timeScale = 1;
            pauseOrContinueText.text = "II";
            isPause = false;
        }
        
    }
}
