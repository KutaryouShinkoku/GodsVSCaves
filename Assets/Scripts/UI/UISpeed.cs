using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpeed : MonoBehaviour
{
    public void SetSpeed1x()
    {
        Time.timeScale = 1;
    }
    public void SetSpeed2x()
    {
        Time.timeScale = 2;
    }
    public void SetSpeedPause()
    {
        Time.timeScale = 0;
    }
}
