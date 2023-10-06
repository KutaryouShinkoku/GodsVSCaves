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

    Hero p1Hero;
    Hero p2Hero;
    public void Start()
    {
        
    }

    //以输入的英雄信息开始游戏
    public void HandleCombatStart(Hero p1Hero,Hero p2Hero)
    {
        state = CombatState.START;

        this.p1Hero = p1Hero;
        this.p2Hero = p2Hero;
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
        p1Unit.Setup(p1Hero);
        p2Unit.Setup(p2Hero);

        //战斗HUD出现
        yield return new WaitForSeconds(0.25f);
        p1HUD.SetHUD(p1Unit.Hero);
        p2HUD.SetHUD(p2Unit.Hero);

        //开战播报
        StartCoroutine( dialogBox.TypeDialog($"{p1Unit.Hero.Base.HeroName} {Localize .GetInstance ().GetTextByKey("VS")} {p2Unit.Hero.Base.HeroName}!"));
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

    //-----------------------------一个回合-----------------------------
    //PlayerMovePerform 放技能
    IEnumerator PerformP1Move()
    {
        //扔骰子
        int currentValue = dice.DiceRoll(p1Unit.Hero.Base.Luck);
        yield return StartCoroutine(HandleDiceRoll(p1Unit, currentValue));
        //处理任何因为技能效果增加的点数，大于6的调整为6
        if(currentValue > 5)
        {
            currentValue = 5;
        }

        //选技能
        var move = p1Unit.Hero.Moves[currentValue];

        //放技能
        StartCoroutine((RunMove(p1Unit,p2Unit,move,p1HUD,p2HUD,currentValue)));
    }

    IEnumerator PerformP2Move()
    {
        //扔骰子
        int currentValue = dice.DiceRoll(p2Unit.Hero.Base.Luck);
        yield return StartCoroutine(HandleDiceRoll(p2Unit, currentValue));
        //处理任何因为技能效果增加的点数，大于6的调整为6
        if (currentValue > 5)
        {
            currentValue = 5;
        }

        //选技能
        var move = p2Unit.Hero.Moves[currentValue];
        //放技能
        StartCoroutine((RunMove(p2Unit, p1Unit, move, p2HUD, p1HUD, currentValue)));
    }
    //-----------------------------处理骰子-----------------------------

    IEnumerator HandleDiceRoll(Unit sourceUnit,int value)
    {
        if (value == 6)
        {
            yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Lucky Boost")}!!!");
            yield return new WaitForSeconds(0.5f);
            value = 5;
        }
        yield return dialogBox.TypeDialog($"{sourceUnit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("rolled a")} ......{value + 1}!");
        yield return new WaitForSeconds(0.8f);
    }

    //-----------------------------处理技能-----------------------------

    IEnumerator RunMove(Unit sourceUnit, Unit targetUnit,Move move,CombatHUD sourceHUD,CombatHUD targetHUD,int currentValue)
    {
        //技能基础信息初始化
        MoveActionType moveActionType = move.Base.MoveActionType;
        bool isMagic = move.Base.IsMagic;
        yield return dialogBox.TypeDialog($"{sourceUnit.Hero.Base.HeroName } {Localize.GetInstance().GetTextByKey("used")} {move.Base.MoveName}");
        yield return new WaitForSeconds(0.5f);

        //技能动画
        yield return(StartCoroutine(sourceUnit.PlayAttackAnimation(moveActionType)));
        yield return(StartCoroutine(targetUnit.PlayHitAnimation(moveActionType)));

        //计算伤害
        int damage = targetUnit.Hero.CalculateDamage(move, sourceUnit.Hero, currentValue);

        //暴击判断
        bool isCrit = targetUnit.Hero.CritCheck(targetUnit.Hero .Base .Luck);
        if(damage != 0 && isCrit)
        {
            yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Critical Hit")}!");
            damage = (int)(damage * 1.5f);
            yield return new WaitForSeconds(0.7f);
        }

        //受伤与死亡
        bool isFainted = targetUnit.Hero.TakeDamage(damage);
        yield return targetHUD.ShowDamage(damage, isCrit, isMagic);
        yield return targetHUD.UpdateHp();
        yield return targetHUD.HideDamage();
        if (damage != 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Hero.Base.HeroName } {Localize.GetInstance().GetTextByKey("lose")} {damage} {Localize.GetInstance().GetTextByKey("life")}");
        }
        yield return new WaitForSeconds(0.7f);

        //状态改变 //TODO:现在状态改变只能指定单目标，有时间的话分开写，然后把这一坨放到新的函数里
        yield return (StartCoroutine(HandleMoveEffects(move, sourceUnit, targetUnit)));

        //判断死亡
        if (targetUnit.Hero .HP<=0)
        {
            CheckBattleOver(targetUnit);
        }
        else if (sourceUnit.Hero.HP <= 0)
        {
            CheckBattleOver(sourceUnit);
        }
        else
        {
            PassTurn(sourceUnit);
        }
    }

    //处理技能效果
    IEnumerator HandleMoveEffects(Move move,Unit source, Unit target)
    {
        var effect = move.Base.MoveEffects;
        if (effect.Boosts != null)
        {
            if (move.Base. EffectTarget == EffectTarget.Self)
            {
                source.Hero.ApplyBoosts(effect.Boosts);
                //Debug.Log("Magic:" + source.Hero.Magic);

            }
            else if (move.Base.EffectTarget == EffectTarget.Enemy)
            {
                target.Hero.ApplyBoosts(effect.Boosts);
            }
            else if (move.Base.EffectTarget == EffectTarget.All)
            {
                source.Hero.ApplyBoosts(effect.Boosts);
                target.Hero.ApplyBoosts(effect.Boosts);
            }
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    //显示属性增减文字和动画
    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.Hero.StatusChanges.Count > 0)
        {
            var message = unit.Hero.StatusChanges.Dequeue();
            StartCoroutine(unit.PlayBoostedAnimation());
            yield return dialogBox.TypeDialog(message);
            yield return new WaitForSeconds(0.4f);
        }
        yield return new WaitForSeconds(0.4f);
    }

    //过回合
    void PassTurn(Unit turnUnit)
    {
        if(turnUnit == p1Unit)
        {
            state = CombatState.P2TURN;
            StartCoroutine(Player2Turn());
        }
        else if (turnUnit == p2Unit)
        {
            state = CombatState.P1TURN;
            StartCoroutine(Player1Turn());
        }
    }

    //处理角色死亡
    void CheckBattleOver(Unit faintedUnit)
    {
        if(faintedUnit == p1Unit)
        {
            StartCoroutine(faintedUnit.PlayFaintAnimation());
            state = CombatState.P2WON;
            StartCoroutine(EndCombat());
        }
        else if (faintedUnit == p2Unit)
        {
            StartCoroutine(faintedUnit.PlayFaintAnimation());
            state = CombatState.P1WON;
            StartCoroutine(EndCombat());
        }
    }

    //------------------------------------------------------------------
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
            yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("Victory won")}! \n \n...............{Localize.GetInstance().GetTextByKey("for now")}........");
        }
        else if (state == CombatState.P2WON)
        {
            yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("Victory won")}! \n \n...............{Localize.GetInstance().GetTextByKey("for now")}........");
        }

        yield return new WaitForSeconds(3f);

        StartCoroutine(gameController.CombatEnd());
    }
}
