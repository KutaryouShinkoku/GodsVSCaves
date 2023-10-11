using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CombatState { START, SPEEDCHECK,P1TURN, P2TURN, P1WON, P2WON }


public class CombatSystem : MonoBehaviour
{
    public Text combatStatus;

    [SerializeField] Unit p1Unit;
    [SerializeField] Unit p2Unit;
    [SerializeField] CombatHUD p1HUD;
    [SerializeField] CombatHUD p2HUD;
    [SerializeField] GameController gameController;
    [SerializeField] CombatDialogBox dialogBox;
    [SerializeField] UIBet uiBet;
    [SerializeField] Coin coinStorer;
    [SerializeField] BackgroundManager bgManager;

    public CombatState state;
    public Dice dice = new Dice();

    Hero p1Hero;
    Hero p2Hero;
    bool isLastPlayer;
    public void Start()
    {
        
    }

    //以输入的英雄信息开始游戏
    public void HandleCombatStart(Hero p1Hero,Hero p2Hero)
    {
        state = CombatState.START;

        this.p1Hero = p1Hero;
        this.p2Hero = p2Hero;
        //设置背景
        bgManager.SetBackground(p1Hero.Base.HeroCamp, p2Hero.Base.HeroCamp);
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

        //重置下注
        uiBet.ResetBetUI();

        //重置异常状态
        p1Hero.ResetStatus();
        p2Hero.ResetStatus();

        //小人从天而降，产生出场特效
        p1Unit.Setup(p1Hero);
        p2Unit.Setup(p2Hero);

        //战斗HUD出现
        yield return new WaitForSeconds(0.25f);
        p1HUD.SetHUD(p1Unit.Hero);
        p2HUD.SetHUD(p2Unit.Hero);

        //开战播报
        StartCoroutine( dialogBox.TypeDialog(string.Format($"{Localize .GetInstance ().GetTextByKey("{0} VS {1}!")}", p1Unit.Hero.Base.HeroName, p2Unit.Hero.Base.HeroName)));
        yield return new WaitForSeconds(2f);
        yield return dialogBox.TypeDialogSlow($"3......2......1.....GO!");
        yield return new WaitForSeconds(1f);

        isLastPlayer = false;
        StartCoroutine(NewTurn());
    }
    //开始一个新回合
    IEnumerator NewTurn()
    {
        if (SpeedCheck() == 1)
        {
            Debug.Log("检查出手权：p" + SpeedCheck() + "先手");
            state = CombatState.P1TURN;
            StartCoroutine(Player1Turn());

        }
        else
        {
            Debug.Log("检查出手权：p" + SpeedCheck() + "先手");
            state = CombatState.P2TURN;
            StartCoroutine(Player2Turn());
        }
        yield return null;
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
        int currentValue = dice.DiceRoll(p1Unit.Hero.Base.Luck,p1Unit .Hero);
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
        int currentValue = dice.DiceRoll(p2Unit.Hero.Base.Luck,p2Unit.Hero);
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
        //文字提示更新
        while (sourceUnit.Hero.CharacterBoost.Count > 0)
        {
            var message = sourceUnit.Hero.CharacterBoost.Dequeue();
            yield return dialogBox.TypeDialog(message);
            yield return new WaitForSeconds(0.8f);
        }
        if (value >= 6) //重置过量的点数
        {
            value = 5;
        }
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} rolled a ......{1}!")}", sourceUnit.Hero.Base.HeroName, value + 1));
        yield return new WaitForSeconds(0.8f);
    }

    //-----------------------------处理技能-----------------------------

    IEnumerator RunMove(Unit sourceUnit, Unit targetUnit,Move move,CombatHUD sourceHUD,CombatHUD targetHUD,int currentValue)
    {
        //技能基础信息初始化
        MoveActionType moveActionType = move.Base.MoveActionType;
        bool isMagic = move.Base.IsMagic;
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} used {1}")}", sourceUnit.Hero.Base.HeroName, move.Base.MoveName));
        yield return new WaitForSeconds(0.5f);

        //技能动画
        //算伤害
        if (move .Base.ExtraTime > 0)
        {
            for(int i = move.Base.ExtraTime; i > 0; i--)
            {
                yield return (StartCoroutine(sourceUnit.PlayAttackAnimation(moveActionType)));
                yield return (StartCoroutine(targetUnit.PlayHitAnimation(moveActionType)));
                yield return (StartCoroutine(HandleDamage(sourceUnit, targetUnit, move, targetHUD, currentValue, move.Base.IsMagic)));
            }
        }
        yield return (StartCoroutine(sourceUnit.PlayAttackAnimation(moveActionType)));
        yield return (StartCoroutine(targetUnit.PlayHitAnimation(moveActionType)));
        yield return (StartCoroutine(HandleDamage(sourceUnit ,targetUnit,move,targetHUD,currentValue,move.Base.IsMagic)));

        //处理技能特效 //TODO:现在状态改变只能指定单目标，有时间的话分开写，然后把这一坨放到新的函数里
        yield return (StartCoroutine(HandleMoveEffects(move, sourceUnit, targetUnit,currentValue)));
        yield return sourceHUD.UpdateHpBarColor();
        yield return targetHUD.UpdateHpBarColor();
        yield return sourceHUD.UpdateHp();
        yield return targetHUD.UpdateHp();

        //回合结束阶段处理各类效果
        sourceUnit.Hero.OnAfterTurn();
        StartCoroutine(sourceUnit.PlayStatusAnimation());
        yield return ShowStatusChanges(sourceUnit);
        yield return sourceHUD.UpdateHp();
        yield return targetHUD.UpdateHp();
        yield return new WaitForSeconds(0.3f);

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
    //算伤害
    IEnumerator HandleDamage(Unit sourceUnit, Unit targetUnit, Move move, CombatHUD targetHUD, int currentValue,bool isMagic)
    {
        if (move.Base.Power != 0)
        {
            //暴击判断
            bool isCrit = targetUnit.Hero.CritCheck(sourceUnit.Hero.Base.Luck, targetUnit.Hero.Base.Luck);

            //计算伤害
            int damage = targetUnit.Hero.CalculateDamage(move, sourceUnit.Hero, targetUnit.Hero, currentValue,isCrit);

            //暴击伤害处理
            if (damage != 0 && isCrit)
            {
                yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Critical Hit!")}");
                damage = (int)(damage * 1.5f);
                yield return new WaitForSeconds(0.7f);
            }

            //受伤与死亡
            targetUnit.Hero.UpdateHp(damage);
            yield return targetHUD.ShowDamage(damage, isCrit, isMagic);
            yield return targetHUD.UpdateHp();
            if (damage != 0)
            {
                yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} lose {1} life")}", targetUnit.Hero.Base.HeroName, damage));
            }
            yield return new WaitForSeconds(0.7f);
            yield return targetHUD.HideDamage();
        }
    }

    //处理技能效果
    IEnumerator HandleMoveEffects(Move move,Unit source, Unit target,int diceValue)
    {
        yield return StartCoroutine(HandleBoosts(move,source ,target)); //处理属性增减
        yield return StartCoroutine(HandleStatus(move, source, target)); //处理特殊状态
        yield return StartCoroutine(HandleHeal(move, source, target, diceValue)); //处理治疗
    }

    //显示任何变化需要的文字
    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.Hero.StatusChanges.Count > 0)
        {
            var message = unit.Hero.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
            yield return new WaitForSeconds(0.8f);
        }
    }

    //过回合
    void PassTurn(Unit turnUnit)
    {
        if(isLastPlayer) { StartCoroutine(NewTurn()); }
        else
        {
            if (turnUnit == p1Unit)
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
            yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("Victory won")}! \n...............{Localize.GetInstance().GetTextByKey("for now")}........");
        }
        else if (state == CombatState.P2WON)
        {
            yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("Victory won")}! \n...............{Localize.GetInstance().GetTextByKey("for now")}........");
        }
        yield return new WaitForSeconds(0.5f);
        uiBet.DisableBet();
        p1HUD.HideHUD();
        p2HUD.HideHUD();
        Time.timeScale = 1;
        yield return StartCoroutine(ResolveCoins(uiBet .p1Odd,uiBet.p2Odd,uiBet.p1Coin,uiBet.p2Coin,state));
        yield return new WaitForSeconds(1f);
        StartCoroutine(gameController.CombatEnd());
    }

    //------------------------------------------------------------------
    //各种技能特效
    IEnumerator HandleBoosts(Move move, Unit source, Unit target) //属性增减
    {
        var effect = move.Base.MoveEffects;
        if (effect.Boosts.Count != 0)
        {
            if (move.Base.EffectTarget == EffectTarget.Self)
            {
                source.Hero.ApplyBoosts(effect.Boosts);
                StartCoroutine(source.PlayBoostedAnimation());
            }
            else if (move.Base.EffectTarget == EffectTarget.Enemy)
            {
                target.Hero.ApplyBoosts(effect.Boosts);
                StartCoroutine(target.PlayBoostedAnimation());
            }
            else if (move.Base.EffectTarget == EffectTarget.All)
            {
                source.Hero.ApplyBoosts(effect.Boosts);
                target.Hero.ApplyBoosts(effect.Boosts);
                StartCoroutine(source.PlayBoostedAnimation());
                StartCoroutine(target.PlayBoostedAnimation());
            }
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator HandleStatus(Move move, Unit source, Unit target)//特殊状态
    {
        var effect = move.Base.MoveEffects;
        if(effect .Status != ConditionID.none)
        {
            target.Hero.SetStatus(effect.Status);
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }

    IEnumerator HandleHeal(Move move, Unit source, Unit target,int diceValue) //治疗
    {
        var effect = move.Base.MoveEffects;
        if(effect.Heal != 0)
        {
            source.Hero.UpdateHpHeal(effect.Heal, diceValue);
            StartCoroutine(source.PlayBoostedAnimation());
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }
    //------------------------------------------------------------------
    //局外
    IEnumerator ResolveCoins(float p1Odd,float p2Odd,int p1Coin,int p2Coin,CombatState state) //结算金币
    {
        int coinChange = 0;
        if (state == CombatState.P1WON)
        {
            coinChange = (int)((1f + p1Odd) * p1Coin);
        }
        if (state == CombatState.P2WON)
        {
            coinChange = (int)((1f + p2Odd) * p2Coin);
        }
        coinStorer.coinAmount+=coinChange ;
        coinStorer.SaveCoin();

        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("You get {0} coins!")}", coinChange));
        yield return new WaitForSeconds(1f);
        if (coinStorer.coinAmount < 100)
        {
            yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("You have spent all your coins, and borrow some from the developer, remember to pay it back~")}"));
            coinStorer.coinAmount = 100;
            coinStorer.SaveCoin();
            yield return new WaitForSeconds(2f);
        }
    }

}
