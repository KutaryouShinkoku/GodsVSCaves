using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] Sprite bgSky;
    [SerializeField] Sprite bgCity;
    [SerializeField] SpriteRenderer bg;

    public void SetBackground(HeroCamp p1camp,HeroCamp p2camp)
    {
        Debug.Log("setBG");
        if(p1camp == HeroCamp.GOD&&p2camp == HeroCamp.GOD)
        {
            bg.sprite = bgSky;
        }
        else
        {
            bg.sprite = bgCity;
        }
    }
}
