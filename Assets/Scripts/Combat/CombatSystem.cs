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

    //�������Ӣ����Ϣ��ʼ��Ϸ
    public void HandleCombatStart(Hero p1Hero,Hero p2Hero)
    {
        state = CombatState.START;

        this.p1Hero = p1Hero;
        this.p2Hero = p2Hero;
        StartCoroutine(SetupCombat());
        Debug.Log("ս����ʼ");
    }

    //SetupCombat ս����ʼ��
    IEnumerator SetupCombat()
    {
        //����λ�úͶԻ���
        combatStatus.text = "";
        StartCoroutine(p1Unit.AnimationReset());
        StartCoroutine(p2Unit.AnimationReset());

        //С�˴������������������Ч
        p1Unit.Setup(p1Hero);
        p2Unit.Setup(p2Hero);

        //ս��HUD����
        yield return new WaitForSeconds(0.25f);
        p1HUD.SetHUD(p1Unit.Hero);
        p2HUD.SetHUD(p2Unit.Hero);

        //��ս����
        StartCoroutine( dialogBox.TypeDialog($"{p1Unit.Hero.Base.HeroName} {Localize .GetInstance ().GetTextByKey("VS")} {p2Unit.Hero.Base.HeroName}!"));
        yield return new WaitForSeconds(2f);
        yield return dialogBox.TypeDialogSlow($"3......2......1.....GO!");
        yield return new WaitForSeconds(1f);

        Debug.Log("������Ȩ��p" + SpeedCheck() + "����");

        //������Ȩ
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

    //SpeedCheck �Ƚ�˫���ٶȾ�������Ȩ
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

    //-----------------------------һ���غ�-----------------------------
    //PlayerMovePerform �ż���
    IEnumerator PerformP1Move()
    {
        //������
        int currentValue = dice.DiceRoll(p1Unit.Hero.Base.Luck);
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
        int currentValue = dice.DiceRoll(p2Unit.Hero.Base.Luck);
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
        if (value == 6)
        {
            yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Lucky Boost")}!!!");
            yield return new WaitForSeconds(0.5f);
            value = 5;
        }
        yield return dialogBox.TypeDialog($"{sourceUnit.Hero.Base.HeroName} {Localize.GetInstance().GetTextByKey("rolled a")} ......{value + 1}!");
        yield return new WaitForSeconds(0.8f);
    }

    //-----------------------------������-----------------------------

    IEnumerator RunMove(Unit sourceUnit, Unit targetUnit,Move move,CombatHUD sourceHUD,CombatHUD targetHUD,int currentValue)
    {
        //���ܻ�����Ϣ��ʼ��
        MoveActionType moveActionType = move.Base.MoveActionType;
        bool isMagic = move.Base.IsMagic;
        yield return dialogBox.TypeDialog($"{sourceUnit.Hero.Base.HeroName } {Localize.GetInstance().GetTextByKey("used")} {move.Base.MoveName}");
        yield return new WaitForSeconds(0.5f);

        //���ܶ���
        yield return(StartCoroutine(sourceUnit.PlayAttackAnimation(moveActionType)));
        yield return(StartCoroutine(targetUnit.PlayHitAnimation(moveActionType)));

        //�����˺�
        int damage = targetUnit.Hero.CalculateDamage(move, sourceUnit.Hero, currentValue);

        //�����ж�
        bool isCrit = targetUnit.Hero.CritCheck(targetUnit.Hero .Base .Luck);
        if(damage != 0 && isCrit)
        {
            yield return dialogBox.TypeDialog($"{Localize.GetInstance().GetTextByKey("Critical Hit")}!");
            damage = (int)(damage * 1.5f);
            yield return new WaitForSeconds(0.7f);
        }

        //����������
        bool isFainted = targetUnit.Hero.TakeDamage(damage);
        yield return targetHUD.ShowDamage(damage, isCrit, isMagic);
        yield return targetHUD.UpdateHp();
        yield return targetHUD.HideDamage();
        if (damage != 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Hero.Base.HeroName } {Localize.GetInstance().GetTextByKey("lose")} {damage} {Localize.GetInstance().GetTextByKey("life")}");
        }
        yield return new WaitForSeconds(0.7f);

        //״̬�ı� //TODO:����״̬�ı�ֻ��ָ����Ŀ�꣬��ʱ��Ļ��ֿ�д��Ȼ�����һ��ŵ��µĺ�����
        yield return (StartCoroutine(HandleMoveEffects(move, sourceUnit, targetUnit)));

        //�ж�����
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

    //������Ч��
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

    //��ʾ�����������ֺͶ���
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

    //���غ�
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

    //�����ɫ����
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

    //EndBattle ��Ϸ����
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
