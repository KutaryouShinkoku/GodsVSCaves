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

    //�������Ӣ����Ϣ��ʼ��Ϸ
    public void HandleCombatStart(Hero p1Hero,Hero p2Hero)
    {
        state = CombatState.START;

        this.p1Hero = p1Hero;
        this.p2Hero = p2Hero;
        //���ñ���
        bgManager.SetBackground(p1Hero.Base.HeroCamp, p2Hero.Base.HeroCamp);
        StartCoroutine(SetupCombat());
        Debug.Log("ս����ʼ");
    }

    //SetupCombat ս����ʼ��
    IEnumerator SetupCombat()
    {
        //����λ�úͶԻ���
        combatStatus.text = "";
        turnCount = 0;
        turnCounter.text = $"{Localize.GetInstance().GetTextByKey("Turn")} {turnCount}"; 
        StartCoroutine(p1Unit.AnimationReset());
        StartCoroutine(p2Unit.AnimationReset());

        //������ע
        uiBet.ResetBetUI();

        //�����쳣״̬
        p1Hero.ResetStatus();
        p2Hero.ResetStatus();

        //С�˴������������������Ч
        p1Unit.Setup(p1Hero);
        p2Unit.Setup(p2Hero);

        //ս��HUD����
        yield return new WaitForSeconds(0.25f);
        p1HUD.SetHUD(p1Unit.Hero);
        p2HUD.SetHUD(p2Unit.Hero);

        //��ս����
        StartCoroutine( dialogBox.TypeDialog(string.Format($"{Localize .GetInstance ().GetTextByKey("{0} VS {1}!")}", p1Unit.Hero.Base.HeroName, p2Unit.Hero.Base.HeroName)));
        yield return new WaitForSeconds(2f);
        yield return dialogBox.TypeDialogSlow($"3......2......1.....GO!");
        yield return new WaitForSeconds(1f);

        isLastPlayer = false;
        StartCoroutine(NewTurn());
    }
    //��ʼһ���»غ�
    IEnumerator NewTurn()
    {
        turnCount += 1;
        Debug.Log($"Turn{turnCount}");
        turnCounter.text = $"{Localize.GetInstance().GetTextByKey("Turn")} {turnCount}";
        if (SpeedCheck() == 1)
        {
            Debug.Log("������Ȩ��p" + SpeedCheck() + "����");
            state = CombatState.P1TURN;
            StartCoroutine(Player1Turn());

        }
        else
        {
            Debug.Log("������Ȩ��p" + SpeedCheck() + "����");
            state = CombatState.P2TURN;
            StartCoroutine(Player2Turn());
        }
        yield return null;
    }

    //SpeedCheck �Ƚ�˫���ٶȾ�������Ȩ
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

    //-----------------------------һ���غ�-----------------------------
    //PlayerMovePerform �ż���
    IEnumerator PerformP1Move()
    {
        //������
        audioManager.Play(2, "diceRoll", false);
        int currentValue = dice.DiceRoll(p1Unit.Hero.Base.Luck,p1Unit .Hero);
        yield return StartCoroutine(HandleDiceRoll(p1Unit, currentValue));
        //�����κ���Ϊ����Ч�����ӵĵ���������6�ĵ���Ϊ6
        if(currentValue > 5)
        {
            currentValue = 5;
        }

        //ѡ����
        var move = p1Unit.Hero.Moves[currentValue];

        //�ż���
        StartCoroutine((RunMove(p1Unit,p2Unit,move,p1HUD,p2HUD,currentValue)));
    }

    IEnumerator PerformP2Move()
    {
        //������
        audioManager.Play(2, "diceRoll", false);
        int currentValue = dice.DiceRoll(p2Unit.Hero.Base.Luck,p2Unit.Hero);
        yield return StartCoroutine(HandleDiceRoll(p2Unit, currentValue));
        //�����κ���Ϊ����Ч�����ӵĵ���������6�ĵ���Ϊ6
        if (currentValue > 5)
        {
            currentValue = 5;
        }

        //ѡ����
        var move = p2Unit.Hero.Moves[currentValue];
        //�ż���
        StartCoroutine((RunMove(p2Unit, p1Unit, move, p2HUD, p1HUD, currentValue)));
    }
    //-----------------------------��������-----------------------------

    IEnumerator HandleDiceRoll(Unit sourceUnit,int value)
    {
        //������ʾ����
        while (sourceUnit.Hero.CharacterBoost.Count > 0)
        {
            audioManager.Play(2, "luckyBoost", false);
            var message = sourceUnit.Hero.CharacterBoost.Dequeue();
            yield return dialogBox.TypeDialog(message);
            yield return new WaitForSeconds(0.8f);
        }
        if (value >= 6) //���ù����ĵ���
        {
            value = 5;
        }
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} rolled a ......{1}!")}", sourceUnit.Hero.Base.HeroName, value + 1));
        yield return new WaitForSeconds(0.8f);
    }

    //-----------------------------������-----------------------------

    IEnumerator RunMove(Unit sourceUnit, Unit targetUnit,Move move,CombatHUD sourceHUD,CombatHUD targetHUD,int currentValue)
    {
        //���ܻ�����Ϣ��ʼ��
        yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} used {1}")}", sourceUnit.Hero.Base.HeroName, move.Base.MoveName));
        yield return new WaitForSeconds(0.5f);

        //���ܶ�����Ȼ�����˺�
        if (move.Base.MoveEffects.Delay == 0) { yield return StartCoroutine(HandleBaseAttack(sourceUnit, targetUnit, move, targetHUD, currentValue, move.Base.IsMagic)); }
        else
        {
            yield return (StartCoroutine(sourceUnit.PlayAttackAnimation(move)));
            targetUnit.Hero.DelayDamage = targetUnit.Hero.CalculateDamage(move, sourceUnit.Hero, targetUnit.Hero, currentValue, false);
            targetUnit.Hero.IsDelayDamageMagic = move.Base.IsMagic;
        }

        //��������Ч
        yield return (StartCoroutine(HandleMoveEffects(move, sourceUnit, targetUnit,currentValue)));
        yield return sourceHUD.UpdateHpBarColor();
        yield return targetHUD.UpdateHpBarColor();
        yield return sourceHUD.UpdateHp();
        yield return targetHUD.UpdateHp();

        //�����ӳ��˺�
        if (sourceUnit.Hero.DelayDamage!=0)
        {
            yield return (StartCoroutine(TakeDelayDamage(targetUnit,sourceUnit,sourceHUD)));
        }

        //�غϽ����׶δ������Ч��
        sourceUnit.Hero.OnAfterTurn();
        StartCoroutine(sourceUnit.PlayStatusAnimation());
        //ƣ�ͼ춨
        StartCoroutine(CheckExhausted(sourceUnit, turnCount));
        //�������ݵ����ֲ���
        yield return ShowStatusChanges(sourceUnit);
        yield return sourceHUD.UpdateHp();
        yield return targetHUD.UpdateHp();
        yield return new WaitForSeconds(0.3f);

        //�ж�����
        yield return StartCoroutine(CheckFainted(sourceUnit, targetUnit));
    }
    //�����Ļ�������
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
    //���˺�
    IEnumerator HandleDamage(Unit sourceUnit, Unit targetUnit, Move move, CombatHUD targetHUD, int currentValue,bool isMagic)
    {
        if (move.Base.Power != 0)
        {
            //�����ж�
            bool isCrit = targetUnit.Hero.CritCheck(sourceUnit.Hero.Base.Luck, targetUnit.Hero.Base.Luck);

            //�����˺�
            int damage = targetUnit.Hero.CalculateDamage(move, sourceUnit.Hero, targetUnit.Hero, currentValue,isCrit);

            //�����˺�����
            if (damage != 0 && isCrit)
            {
                yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Critical Hit!")}");
                damage = (int)(damage * 1.5f);
                yield return new WaitForSeconds(0.7f);
            }
            //������Ч������
            if (damage >= targetUnit.Hero.HP&&move.Base.IsNonLethal==true)
            {
                damage = targetUnit.Hero.HP - 1;
                yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} showed mercy")}",sourceUnit .Hero.Base.HeroName));
                yield return new WaitForSeconds(0.7f);
            }

            //����
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
    //�ӳ��˺�
    IEnumerator TakeDelayDamage(Unit source, Unit target, CombatHUD targetHUD)
    {
        //�����ж�
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

    //������Ч��
    IEnumerator HandleMoveEffects(Move move,Unit source, Unit target,int diceValue)
    {
        audioManager.PlayMoveEffectAudio(1, move, false);
        yield return StartCoroutine(HandleDelayDamage(move, source, target)); //�����ӳ��˺�
        yield return StartCoroutine(HandleBoosts(move,source ,target)); //������������
        yield return StartCoroutine(HandleAdaptiveBoosts(move, source, target)); //����������������
        yield return StartCoroutine(HandleStatus(move, source, target)); //��������״̬
        yield return StartCoroutine(HandleHeal(move, source, target, diceValue)); //��������
        yield return StartCoroutine(HandlePercentDamage(move, source, target)); //����ǰ�����ٷֱȵ�Ѫ
    }

    //��ʾ�κα仯��Ҫ������
    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.Hero.StatsChanges.Count > 0)
        {
            var message = unit.Hero.StatsChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
            yield return new WaitForSeconds(0.8f);
        }
    }
    //ƣ�ͼ���
    IEnumerator CheckExhausted(Unit unit,int turnCount)
    {
        if(turnCount > 20)
        {
            unit.Hero.ExhaustedDamage(turnCount);
        }
        yield return null;
    }

    //���غ�
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
    //�жϽ�ɫ�Ƿ�����
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

    //�ж���Ϸ����
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
    //Player turn ���һ�غϵ�����
    IEnumerator Player1Turn()
    {
        //��ͷ�л�
        StartCoroutine(p1HUD.SetArrow());
        StartCoroutine(p2HUD.HideArrow());
        //����
        StartCoroutine(PerformP1Move());
        yield return new WaitForSeconds(1.2f);
    }
    IEnumerator Player2Turn()
    {
        //��ͷ�л�
        StartCoroutine(p2HUD.SetArrow());
        StartCoroutine(p1HUD.HideArrow());
        //����
        StartCoroutine(PerformP2Move());
        yield return new WaitForSeconds(1.2f);
    }

    //EndBattle ��Ϸ����
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
    //���ּ�����Ч
    IEnumerator HandleBoosts(Move move, Unit source, Unit target) //��������
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
    IEnumerator HandleAdaptiveBoosts(Move move, Unit source, Unit target) //������������
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
    IEnumerator HandleStatus(Move move, Unit source, Unit target)//����״̬
    {
        var effect = move.Base.MoveEffects;
        if(effect .Status != ConditionID.none)
        {
            target.Hero.SetStatus(effect.Status);
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }

    IEnumerator HandleHeal(Move move, Unit source, Unit target,int diceValue) //����
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

    IEnumerator HandlePercentDamage(Move move, Unit source, Unit target) //��ǰ�����ٷֱȿ�Ѫ
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
    IEnumerator HandleDelayDamage(Move move, Unit source, Unit target) //�ӳ��˺�
    {
        var effect = move.Base.MoveEffects;
        if (effect.Delay != 0)
        {
            yield return dialogBox.TypeDialog(string.Format($"{Localize.GetInstance().GetTextByKey("{0} sent something to the future")}", source.Hero.Base.HeroName));
            yield return new WaitForSeconds(0.8f);
        }
    }
    //------------------------------�����UI------------------------------------
    //������
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

    //Ӣ����Ϣ
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
