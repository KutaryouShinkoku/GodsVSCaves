using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CombatState { START, P1TURN, P2TURN, P1WON, P2WON }


public class CombatSystem : MonoBehaviour
{
    public Text combatStatus;

    [SerializeField] Unit p1Unit;
    [SerializeField] Unit p2Unit;

    [SerializeField] CombatHUD p1HUD;
    [SerializeField] CombatHUD p2HUD;
    [SerializeField] GameController gameController;

    [SerializeField] CombatDialogBox dialogBox;

    public CombatState state;

    public Dice dice = new Dice(); 
    public void Start()
    {
        
    }

    public void HandleCombatStart()
    {
        state = CombatState.START;
        StartCoroutine(SetupCombat());
        Debug.Log("战斗开始");
    }

    //SetupCombat 战斗初始化
    IEnumerator SetupCombat()
    {
        //重置位置和对话框
        combatStatus.text = "";
        StartCoroutine(p1Unit.AnimationReset());
        StartCoroutine(p2Unit.AnimationReset());

        //小人从天而降，产生出场特效
        p1Unit.Setup();
        p2Unit.Setup();

        //战斗HUD出现
        yield return new WaitForSeconds(0.25f);
        p1HUD.SetHUD(p1Unit.Hero);
        p2HUD.SetHUD(p2Unit.Hero);

        //开战播报
        StartCoroutine( dialogBox.TypeDialog($"{p1Unit.Hero.Base.Name} VS {p2Unit.Hero.Base.Name}!"));
        yield return new WaitForSeconds(2f);
        yield return dialogBox.TypeDialogSlow($"3......2......1.....GO!");
        yield return new WaitForSeconds(1f);

        Debug.Log("检查出手权：p" + SpeedCheck() + "先手");

        //检查出手权
        if (SpeedCheck() == 1)
        {
            state = CombatState.P1TURN;
            StartCoroutine(Player1Turn());

        }
        else
        {
            state = CombatState.P2TURN;
            StartCoroutine(Player2Turn());
        }
    }

    //SpeedCheck 比较双方速度决定出手权
    public int SpeedCheck()
    {
        int firstPlayer;
        if (p1Unit.Hero.Base .Speed > p2Unit.Hero.Base.Speed)
        {
            firstPlayer = 1;
        }
        else if (p1Unit.Hero.Base.Speed < p2Unit.Hero.Base.Speed)
        {
            firstPlayer = 2;
        }
        else
        {
            firstPlayer = Random.Range(1, 3);
        }
        return firstPlayer;
    }

    //PlayerMovePerform 放技能
    IEnumerator PerformP1Move()
    {
        //扔骰子
        int currentValue =  dice.DiceRoll();
        yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } rolled a ......{currentValue+1}!");
        yield return new WaitForSeconds(0.8f);

        //选技能
        var move = p1Unit.Hero.Moves[dice.CurrentValue];
        yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } used {move.Base.name}");
        //dialogBox.SetDialog($"{p1Unit.Hero.Base.name } used {move.Base.name}"); //备用，防止出字bug
        yield return new WaitForSeconds(0.5f);

        //技能动画
        StartCoroutine(p1Unit.PlayAttackAnimation());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(p2Unit.PlayHitAnimation());

        //计算伤害
        int damage = p2Unit.Hero.CalculateDamage(move, p1Unit.Hero, currentValue);

        //暴击判断
        bool isCrit = p2Unit.Hero.CritCheck();
        if(damage != 0 && isCrit)
        {
            yield return dialogBox.TypeDialog($"Critical Hit!");
            damage = (int)(damage * 1.5f);
            yield return new WaitForSeconds(0.7f);
        }

        //受伤与死亡
        bool isFainted = p2Unit.Hero.TakeDamage(damage);
        yield return p2HUD.ShowDamage(damage, isCrit);
        yield return p2HUD.UpdateHp();
        yield return p2HUD.HideDamage();
        if (damage != 0)
        {
            yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } lose {damage} life");
        }
        yield return new WaitForSeconds(0.7f);
        //Debug.Log("p1还有" + p1Unit.Hero.HP + "血");
        
        //判断死亡
        if (isFainted)
        {
            StartCoroutine(p2Unit.PlayFaintAnimation());
            state = CombatState.P1WON;
            StartCoroutine(EndCombat());
        }
        else
        {
            state = CombatState.P2TURN;
            StartCoroutine(Player2Turn());
        }
    }

    IEnumerator PerformP2Move()
    {
        //扔骰子
        int currentValue = dice.DiceRoll();
        yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } rolled a ......{currentValue + 1}!");
        yield return new WaitForSeconds(0.8f);

        //选技能
        var move = p2Unit.Hero.Moves[dice.CurrentValue];
        yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } used {move.Base.name}");
        //dialogBox.SetDialog($"{p2Unit.Hero.Base.name } used {move.Base.name}");  //备用，防止出字bug
        yield return new WaitForSeconds(0.5f);

        //技能动画
        StartCoroutine(p2Unit.PlayAttackAnimation());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(p1Unit.PlayHitAnimation());

        //计算伤害
        int damage = p1Unit.Hero.CalculateDamage(move, p2Unit.Hero, currentValue);

        //暴击判断
        bool isCrit = p1Unit.Hero.CritCheck();
        if (damage != 0 && isCrit)
        {
            yield return dialogBox.TypeDialog($"Critical Hit!");
            damage = (int)(damage * 1.5f);
            yield return new WaitForSeconds(0.5f);
        }

        //受伤与死亡
        bool isFainted = p1Unit.Hero.TakeDamage(damage);
        yield return p1HUD.ShowDamage(damage, isCrit);
        yield return p1HUD.UpdateHp();
        yield return p1HUD.HideDamage();
        if (damage != 0)
        {
            yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } lose {damage} life");
        }
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("p2还有" + p2Unit.Hero.HP + "血");

        //判断死亡
        if (isFainted)
        {
            StartCoroutine(p1Unit.PlayFaintAnimation());
            state = CombatState.P2WON;
            StartCoroutine(EndCombat());
        }
        else
        {
            state = CombatState.P1TURN;
            StartCoroutine(Player1Turn());
        }
    }


    //Player turn 玩家一回合的流程
    IEnumerator Player1Turn()
    {
        StartCoroutine(p1HUD.SetArrow());
        StartCoroutine(p2HUD.HideArrow());
        StartCoroutine(PerformP1Move());
        yield return new WaitForSeconds(1.2f);
    }

    IEnumerator Player2Turn()
    {
        StartCoroutine(p2HUD.SetArrow());
        StartCoroutine(p1HUD.HideArrow());
        StartCoroutine(PerformP2Move());
        yield return new WaitForSeconds(1.2f);
    }

    //EndBattle 游戏结束
    IEnumerator EndCombat()
    {
        if (state == CombatState.P1WON)
        {
            combatStatus.text = p1Unit.Hero.Base.Name + " Won!";
        }
        else if (state == CombatState.P2WON)
        {
            combatStatus.text = p2Unit.Hero.Base.Name + " Won!";
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(gameController.CombatEnd());
    }



}
