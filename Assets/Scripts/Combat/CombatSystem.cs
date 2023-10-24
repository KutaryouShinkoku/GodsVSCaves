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
    [SerializeField] Coin coinStorer;
    [SerializeField] BackgroundManager bgManager;
    [SerializeField] AudioManager audioManager;

    [Header("UI")]
    [SerializeField] Text turnCounter;
    [SerializeField] CombatDialogBox dialogBox;
    [SerializeField] UIBet uiBet;
    [SerializeField] UIHeroDetail detailP1;
    [SerializeField] UIHeroDetail detailP2;
    [SerializeField] GameObject uiDetail;

    public CombatState state;
    public Dice dice = new Dice();

    Hero p1Hero;
    Hero p2Hero;
    bool isLastPlayer;
    int turnCount;

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
        turnCount = 0;
        turnCounter.text = $"{Localize.GetInstance().GetTextByKey("Turn")} {turnCount}"; 
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
        turnCount += 1;
        Debug.Log($"Turn{turnCount}");
        turnCounter.text = $"{Localize.GetInstance().GetTextByKey("Turn")} {turnCount}";
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
        if (p1Unit.Hero.Speed > p2Unit.Hero.Speed)
        {
            firstPlayer = 1;
        }
        else if (p1Unit.Hero.Speed < p2Unit.Hero.Speed)
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
        audioManager.Play(2, "diceRoll", false);
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
        audioManager.Play(2, "diceRoll", false);
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
            audioManager.Play(2, "luckyBoost", false);
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
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} used {1}")}", sourceUnit.Hero.Base.HeroName, move.Base.MoveName));
        yield return new WaitForSeconds(0.5f);

        //技能动画，然后算伤害
        if (move.Base.MoveEffects.Delay == 0) { yield return StartCoroutine(HandleBaseAttack(sourceUnit, targetUnit, move, targetHUD, currentValue, move.Base.IsMagic)); }
        else
        {
            yield return (StartCoroutine(sourceUnit.PlayAttackAnimation(move)));
            targetUnit.Hero.DelayDamage = targetUnit.Hero.CalculateDamage(move, sourceUnit.Hero, targetUnit.Hero, currentValue, false);
            targetUnit.Hero.IsDelayDamageMagic = move.Base.IsMagic;
        }

        //处理技能特效
        yield return (StartCoroutine(HandleMoveEffects(move, sourceUnit, targetUnit,currentValue)));
        yield return sourceHUD.UpdateHpBarColor();
        yield return targetHUD.UpdateHpBarColor();
        yield return sourceHUD.UpdateHp();
        yield return targetHUD.UpdateHp();

        //结算延迟伤害
        if (sourceUnit.Hero.DelayDamage!=0)
        {
            yield return (StartCoroutine(TakeDelayDamage(targetUnit,sourceUnit,sourceHUD)));
        }

        //回合结束阶段处理各类效果
        sourceUnit.Hero.OnAfterTurn();
        StartCoroutine(sourceUnit.PlayStatusAnimation());
        //疲劳检定
        StartCoroutine(CheckExhausted(sourceUnit, turnCount));
        //上述内容的文字播报
        yield return ShowStatusChanges(sourceUnit);
        yield return sourceHUD.UpdateHp();
        yield return targetHUD.UpdateHp();
        yield return new WaitForSeconds(0.3f);

        //判断死亡
        yield return StartCoroutine(CheckFainted(sourceUnit, targetUnit));
    }
    //攻击的基础部分
    IEnumerator HandleBaseAttack(Unit sourceUnit, Unit targetUnit, Move move, CombatHUD targetHUD, int currentValue, bool isMagic)
    {
        if (move.Base.ExtraTime > 0)
        {
            for (int i = move.Base.ExtraTime; i > 0; i--)
            {
                yield return (StartCoroutine(sourceUnit.PlayAttackAnimation(move)));
                yield return (StartCoroutine(targetUnit.PlayHitAnimation(move)));
                yield return (StartCoroutine(HandleDamage(sourceUnit, targetUnit, move, targetHUD, currentValue, move.Base.IsMagic)));
            }
        }
        yield return (StartCoroutine(sourceUnit.PlayAttackAnimation(move)));
        yield return (StartCoroutine(targetUnit.PlayHitAnimation(move)));
        yield return (StartCoroutine(HandleDamage(sourceUnit, targetUnit, move, targetHUD, currentValue, move.Base.IsMagic)));
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
            //非致死效果处理
            if (damage >= targetUnit.Hero.HP&&move.Base.IsNonLethal==true)
            {
                damage = targetUnit.Hero.HP - 1;
                yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} showed mercy")}",sourceUnit .Hero.Base.HeroName));
                yield return new WaitForSeconds(0.7f);
            }

            //受伤
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
    //延迟伤害
    IEnumerator TakeDelayDamage(Unit source, Unit target, CombatHUD targetHUD)
    {
        //暴击判断
        bool isCrit = target.Hero.CritCheck(source.Hero.Base.Luck, target.Hero.Base.Luck);

        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} is influenced by something from the past")}", target.Hero.Base.HeroName));
        yield return new WaitForSeconds(0.8f);
        audioManager.Play(1, "futureAttack",false);
        yield return target.PlayHurtAnimation();

        if (isCrit)
        {
            yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Critical Hit!")}");
            target.Hero.DelayDamage = (int)(target.Hero.DelayDamage * 1.5f);
            yield return new WaitForSeconds(0.7f);
        }

        target.Hero.UpdateHp(target .Hero.DelayDamage);
        yield return targetHUD.ShowDamage(target.Hero.DelayDamage, isCrit, target .Hero.IsDelayDamageMagic);
        yield return targetHUD.UpdateHp();
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} lose {1} life")}", target.Hero.Base.HeroName, target.Hero.DelayDamage));
        target.Hero.DelayDamage = 0;
        yield return new WaitForSeconds(0.7f);
        yield return targetHUD.HideDamage();
    }

    //处理技能效果
    IEnumerator HandleMoveEffects(Move move,Unit source, Unit target,int diceValue)
    {
        audioManager.PlayMoveEffectAudio(1, move, false);
        yield return StartCoroutine(HandleDelayDamage(move, source, target)); //处理延迟伤害
        yield return StartCoroutine(HandleBoosts(move,source ,target)); //处理属性增减
        yield return StartCoroutine(HandleAdaptiveBoosts(move, source, target)); //处理智能属性增减
        yield return StartCoroutine(HandleStatus(move, source, target)); //处理特殊状态
        yield return StartCoroutine(HandleHeal(move, source, target, diceValue)); //处理治疗
        yield return StartCoroutine(HandlePercentDamage(move, source, target)); //处理当前生命百分比掉血
    }

    //显示任何变化需要的文字
    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.Hero.StatsChanges.Count > 0)
        {
            var message = unit.Hero.StatsChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
            yield return new WaitForSeconds(0.8f);
        }
    }
    //疲劳鉴定
    IEnumerator CheckExhausted(Unit unit,int turnCount)
    {
        if(turnCount > 20)
        {
            unit.Hero.ExhaustedDamage(turnCount);
        }
        yield return null;
    }

    //过回合
    void PassTurn(Unit turnUnit)
    {
        if (isLastPlayer) { isLastPlayer = false; StartCoroutine(NewTurn());}
        else
        {
            if (turnUnit == p1Unit)
            {
                isLastPlayer = true;
                state = CombatState.P2TURN;
                StartCoroutine(Player2Turn());
            }
            else if (turnUnit == p2Unit)
            {
                isLastPlayer = true;
                state = CombatState.P1TURN;
                StartCoroutine(Player1Turn());
            }
        }
    }
    //判断角色是否死亡
    IEnumerator CheckFainted(Unit source,Unit target)
    {

        if (target.Hero.HP <= 0)
        {
             CheckBattleOver(target);
        }
        else if (source.Hero.HP <= 0)
        {
            CheckBattleOver(source);
        }
        else
        {
            PassTurn(source);
        }
        yield return null;
    }

    //判定游戏结束
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
        //箭头切换
        StartCoroutine(p1HUD.SetArrow());
        StartCoroutine(p2HUD.HideArrow());
        //出招
        StartCoroutine(PerformP1Move());
        yield return new WaitForSeconds(1.2f);
    }
    IEnumerator Player2Turn()
    {
        //箭头切换
        StartCoroutine(p2HUD.SetArrow());
        StartCoroutine(p1HUD.HideArrow());
        //出招
        StartCoroutine(PerformP2Move());
        yield return new WaitForSeconds(1.2f);
    }

    //EndBattle 游戏结束
    IEnumerator EndCombat()
    {
        audioManager.Play(2, "combatEnd", false);
        if (state == CombatState.P1WON)
        {
            yield return dialogBox.TypeDialogSlow($"{p1Unit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("Victory won")}! \n{Localize.GetInstance().GetTextByKey("for now")}");
        }
        else if (state == CombatState.P2WON)
        {
            yield return dialogBox.TypeDialogSlow($"{p2Unit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("Victory won")}! \n{Localize.GetInstance().GetTextByKey("for now")}");
        }
        yield return new WaitForSeconds(2f);
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
                StartCoroutine(source.PlayBoostedAnimation(source.Hero.BoostValue(effect.Boosts)));
            }
            else if (move.Base.EffectTarget == EffectTarget.Enemy)
            {
                target.Hero.ApplyBoosts(effect.Boosts);
                StartCoroutine(target.PlayBoostedAnimation(source.Hero.BoostValue(effect.Boosts)));
            }
            else if (move.Base.EffectTarget == EffectTarget.All)
            {
                source.Hero.ApplyBoosts(effect.Boosts);
                target.Hero.ApplyBoosts(effect.Boosts);
                StartCoroutine(source.PlayBoostedAnimation(source.Hero.BoostValue(effect.Boosts)));
                StartCoroutine(target.PlayBoostedAnimation(source.Hero.BoostValue(effect.Boosts)));
            }
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator HandleAdaptiveBoosts(Move move, Unit source, Unit target) //智能属性增减
    {
        var effect = move.Base.MoveEffects;
        if (effect.AdaptiveDecrease.boost != 0)
        {

            if (effect .AdaptiveDecrease.adaptiveType==AdaptiveType.Attack)
            {
                List<StatBoost> statboosts = new List<StatBoost>();
                StatBoost statBoost = new StatBoost(); ;
                if(target.Hero.Base.Attack>= target.Hero.Base.Magic){statBoost.stat = Stat.Attack;}
                else{statBoost.stat = Stat.Magic;}
                statBoost.boost = effect.AdaptiveDecrease.boost;
                statboosts.Add(statBoost);
                target.Hero.ApplyBoosts(statboosts);
                StartCoroutine(target.PlayBoostedAnimation(effect.AdaptiveDecrease.boost));
            }
            else if (effect.AdaptiveDecrease.adaptiveType == AdaptiveType.Defence)
            {
                List<StatBoost> statboosts = new List<StatBoost>();
                StatBoost statBoost = new StatBoost(); ;
                if (target.Hero.Base.Defence >= target.Hero.Base.MagicDef) { statBoost.stat = Stat.Defence; }
                else { statBoost.stat = Stat.MagicDef; }
                statBoost.boost = effect.AdaptiveDecrease.boost;
                statboosts.Add(statBoost);
                target.Hero.ApplyBoosts(statboosts);
                StartCoroutine(target.PlayBoostedAnimation(effect.AdaptiveDecrease.boost));
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
            StartCoroutine(source.PlayHealAnimation());
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }

    IEnumerator HandlePercentDamage(Move move, Unit source, Unit target) //当前生命百分比扣血
    {
        var effect = move.Base.MoveEffects;
        var per = effect.LosePercentLife;
        if(per.max != 0) 
        {
            if(per.target == EffectTarget.Self)
            {
                source.Hero.PercentDamage(per);
                yield return (StartCoroutine(source.PlayHitAnimation(move)));
            }
            else if (per.target == EffectTarget.Enemy)
            {
                target.Hero.PercentDamage(per);
                yield return (StartCoroutine(target.PlayHitAnimation(move)));
            }
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }
    IEnumerator HandleDelayDamage(Move move, Unit source, Unit target) //延迟伤害
    {
        var effect = move.Base.MoveEffects;
        if (effect.Delay != 0)
        {
            yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} sent something to the future")}", source.Hero.Base.HeroName));
            yield return new WaitForSeconds(0.8f);
        }
    }
    //------------------------------局外和UI------------------------------------
    //结算金币
    IEnumerator ResolveCoins(float p1Odd,float p2Odd,int p1Coin,int p2Coin,CombatState state) 
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
        if (coinChange != 0)
        {
            audioManager.Play(2, "getCoin", false);
        }
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("You get {0} coins!")}", coinChange));
        if (coinChange == 0)
        {
            coinStorer.loseTimes++;
        }
        else { coinStorer.loseTimes = 0; }
        yield return new WaitForSeconds(1f);
        if (coinStorer.coinAmount < 100)
        {
            yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("You have spent all your coins, and borrow some from the developer, remember to pay it back~")}"));
            coinStorer.coinAmount = 100;
            coinStorer.SaveCoin();
            yield return new WaitForSeconds(2f);
        }
    }

    //英雄信息
    public void showHeroDetails() 
    {
        uiDetail.SetActive(true);
        detailP1.ShowHeroDetails(p1Unit.Hero);
        detailP2.ShowHeroDetails(p2Unit.Hero);
        Time.timeScale = 0;
    }
    public void CloseHeroDetail()
    {
        uiDetail.SetActive(false);
        Time.timeScale = UISpeed.timeScale;
    }
}
