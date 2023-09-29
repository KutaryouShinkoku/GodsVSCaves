using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelectButton : MonoBehaviour
{
    public Hero hero;
    public Hero GetHero()
    {
        Hero curHero = GetComponent<HeroSelectButton>().hero;
        return curHero;
    }



}
