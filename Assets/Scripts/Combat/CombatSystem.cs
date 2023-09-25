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
        p1Unit.Setup();
        p2Unit.Setup();

        //ս��HUD����
        yield return new WaitForSeconds(0.25f);
        p1HUD.SetHUD(p1Unit.Hero);
        p2HUD.SetHUD(p2Unit.Hero);

        //��ս����
        StartCoroutine( dialogBox.TypeDialog($"{p1Unit.Hero.Base.Name} VS {p2Unit.Hero.Base.Name}!"));
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

    //PlayerMovePerform �ż���
    IEnumerator PerformP1Move()
    {
        //������
        int currentValue =  dice.DiceRoll();
        yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } rolled a ......{currentValue+1}!");
        yield return new WaitForSeconds(0.8f);

        //ѡ����
        var move = p1Unit.Hero.Moves[dice.CurrentValue];
        yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } used {move.Base.name}");
        //dialogBox.SetDialog($"{p1Unit.Hero.Base.name } used {move.Base.name}"); //���ã���ֹ����bug
        yield return new WaitForSeconds(0.5f);

        //���ܶ���
        StartCoroutine(p1Unit.PlayAttackAnimation());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(p2Unit.PlayHitAnimation());

        //�����˺�
        int damage = p2Unit.Hero.CalculateDamage(move, p1Unit.Hero, currentValue);

        //�����ж�
        bool isCrit = p2Unit.Hero.CritCheck();
        if(damage != 0 && isCrit)
        {
            yield return dialogBox.TypeDialog($"Critical Hit!");
            damage = (int)(damage * 1.5f);
            yield return new WaitForSeconds(0.7f);
        }

        //����������
        bool isFainted = p2Unit.Hero.TakeDamage(damage);
        yield return p2HUD.ShowDamage(damage, isCrit);
        yield return p2HUD.UpdateHp();
        yield return p2HUD.HideDamage();
        if (damage != 0)
        {
            yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } lose {damage} life");
        }
        yield return new WaitForSeconds(0.7f);
        //Debug.Log("p1����" + p1Unit.Hero.HP + "Ѫ");
        
        //�ж�����
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
        //������
        int currentValue = dice.DiceRoll();
        yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } rolled a ......{currentValue + 1}!");
        yield return new WaitForSeconds(0.8f);

        //ѡ����
        var move = p2Unit.Hero.Moves[dice.CurrentValue];
        yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } used {move.Base.name}");
        //dialogBox.SetDialog($"{p2Unit.Hero.Base.name } used {move.Base.name}");  //���ã���ֹ����bug
        yield return new WaitForSeconds(0.5f);

        //���ܶ���
        StartCoroutine(p2Unit.PlayAttackAnimation());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(p1Unit.PlayHitAnimation());

        //�����˺�
        int damage = p1Unit.Hero.CalculateDamage(move, p2Unit.Hero, currentValue);

        //�����ж�
        bool isCrit = p1Unit.Hero.CritCheck();
        if (damage != 0 && isCrit)
        {
            yield return dialogBox.TypeDialog($"Critical Hit!");
            damage = (int)(damage * 1.5f);
            yield return new WaitForSeconds(0.5f);
        }

        //����������
        bool isFainted = p1Unit.Hero.TakeDamage(damage);
        yield return p1HUD.ShowDamage(damage, isCrit);
        yield return p1HUD.UpdateHp();
        yield return p1HUD.HideDamage();
        if (damage != 0)
        {
            yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } lose {damage} life");
        }
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("p2����" + p2Unit.Hero.HP + "Ѫ");

        //�ж�����
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
