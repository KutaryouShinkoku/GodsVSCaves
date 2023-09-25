using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState {SELECT,COMBAT}


public class GameController : MonoBehaviour
{
    [SerializeField] HeroSelector heroSelector;
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] Camera selectorCamera;
    [SerializeField] Camera combatCamera;

    public GameState state;

    private void Start()
    {

    }

    public void CombatStart()
    {
        state = GameState.COMBAT;
        combatSystem.gameObject.SetActive(true);
        combatCamera.gameObject.SetActive(true);
        selectorCamera.gameObject.SetActive(false);

        var p1Hero = heroSelector.GetComponent<HeroSelector>().GetP1Hero();
        var p2Hero = heroSelector.GetComponent<HeroSelector>().GetP2Hero();

        combatSystem.HandleCombatStart(p1Hero ,p2Hero );
    }

    public IEnumerator CombatEnd()
    {
        state = GameState.SELECT;
        combatSystem.gameObject.SetActive(false);
        combatCamera.gameObject.SetActive(false);
        selectorCamera.gameObject.SetActive(true);
        yield return null;
    }
}
