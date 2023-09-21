using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CombatState { START, P1TURN, P2TURN, P1WON, P2WON }

public class CombatSystem : MonoBehaviour
{
    //public GameObject p1Prefab;
    //public GameObject p2Prefab;
    
    //public Transform p1Station;
    //public Transform p2Station;

    public Text combatStatus;

    [SerializeField] Unit p1Unit;
    [SerializeField] Unit p2Unit;

    [SerializeField] CombatHUD p1HUD;
    [SerializeField] CombatHUD p2HUD;

    [SerializeField] CombatDialogBox dialogBox;

    public CombatState state;
    private void Start()
    {
        state = CombatState.START;
        StartCoroutine(SetupCombat());
    }

    //SetupCombat 战斗初始化
    IEnumerator SetupCombat()
    {
        p1Unit.Setup();
        p1HUD.SetHUD(p1Unit .Hero);
        p2Unit.Setup();
        p2HUD.SetHUD(p2Unit.Hero);

        StartCoroutine( dialogBox.TypeDialog($"{p1Unit.Hero.Base.Name} VS {p2Unit.Hero.Base.Name}!"));

        //GameObject p1GO = Instantiate(p1Prefab, p1Station);
        //p1Unit = p1GO.GetComponent<Unit>();
        //GameObject p2GO = Instantiate(p2Prefab, p2Station);
        //p2Unit = p2GO.GetComponent<Unit>();

        //combatStatus.text = "Combat begin ! " + p1Unit.unitName + " VS " + p2Unit.unitName;

        yield return new WaitForSeconds(2f);

        //Debug.Log("P" + StaticStores.firstPlayer + "'s turn");
        //    if (SpeedCheck() == 1)
        //    {
        //        state = CombatState.P1TURN;
        //        StartCoroutine(Player1Turn());

        //    }
        //    else
        //    {
        //        state = CombatState.P2TURN;
        //        StartCoroutine(Player2Turn());
        //    }
    }

    //SpeedCheck 比较双方速度决定出手权
    //public int SpeedCheck()
    //{
    //int firstPlayer;
    //if(p1Unit.speed > p2Unit.speed)
    //{
    //    firstPlayer = 1;
    //}
    //else if (p1Unit.speed < p2Unit.speed)
    //{
    //    firstPlayer = 2;
    //}
    //else
    //{
    //    firstPlayer = Random.Range(1, 3);
    //}
    //return firstPlayer;
    //}

    //Player turn 玩家一回合的流程
    //IEnumerator Player1Turn()
    //{
    //combatStatus.text = p1Unit.unitName + "attacks!";
    //yield return new WaitForSeconds(1f);

    //bool isDead = p2Unit.TakeDamage(p1Unit.damage);
    //p2HUD.SetHp(p2Unit.currentHP);
    //combatStatus.text = p2Unit.unitName + " loses " + p1Unit.damage + " life ";

    //yield return new WaitForSeconds(1f);

    //if (isDead)
    //{
    //    state = CombatState.P1WON;
    //    EndCombat();
    //}
    //else
    //{
    //    state = CombatState.P2TURN;
    //    StartCoroutine(Player2Turn());
    //}
    //}

    //IEnumerator Player2Turn()
    //{
    //    combatStatus.text = p2Unit.unitName  + "attacks!";
    //    yield return new WaitForSeconds(1f);

    //    bool isDead = p1Unit.TakeDamage(p2Unit.damage);
    //    p1HUD.SetHp(p1Unit.currentHP);
    //    combatStatus.text = p1Unit.unitName + " loses " + p2Unit.damage + " life ";
    //    yield return new WaitForSeconds(1f);

    //    if (isDead)
    //    {
    //        state = CombatState.P2WON;
    //        EndCombat();
    //    }
    //    else
    //    {
    //        state = CombatState.P1TURN;
    //        StartCoroutine(Player1Turn());
    //    }
    //}

    //EndBattle 游戏结束
    //    void EndCombat()
    //    {
    //        if(state == CombatState.P1WON)
    //        {
    //            combatStatus.text = p1Unit.unitName + " Won!";
    //        }
    //        else if (state == CombatState.P2WON)
    //        {
    //            combatStatus.text = p2Unit.unitName + " Won!";
    //        }
    //    }


}
