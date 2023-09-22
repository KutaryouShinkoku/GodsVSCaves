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

    [SerializeField] CombatDialogBox dialogBox;

    public CombatState state;

    public Dice dice = new Dice(); 
    private void Start()
    {
        state = CombatState.START;
        StartCoroutine(SetupCombat());
    }

    //SetupCombat ս����ʼ��
    IEnumerator SetupCombat()
    {
        p1Unit.Setup();
        p1HUD.SetHUD(p1Unit .Hero);
        p2Unit.Setup();
        p2HUD.SetHUD(p2Unit.Hero);

        StartCoroutine( dialogBox.TypeDialog($"{p1Unit.Hero.Base.Name} VS {p2Unit.Hero.Base.Name}!"));
        yield return new WaitForSeconds(2f);
        yield return dialogBox.TypeDialogSlow($"3......2......1.....GO!");
        yield return new WaitForSeconds(1f);

        Debug.Log("������Ȩ��p" + SpeedCheck() + "����");

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
        yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } rolled a  ...... {currentValue+1}");
        yield return new WaitForSeconds(1.2f);

        //ѡ����
        var move = p1Unit.Hero.Moves[dice.CurrentValue];
        yield return dialogBox.TypeDialog($"{p1Unit.Hero.Base.name } used {move.Base.name}");//������Bug�Ȳ���
        //dialogBox.SetDialog($"{p1Unit.Hero.Base.name } used {move.Base.name}");
        yield return new WaitForSeconds(1.2f);

        //�����˺�
        bool isFainted = p2Unit.Hero.TakeDamage(move, p1Unit.Hero, currentValue);
        p2HUD.UpdateHp();
        p1HUD.ShowDamage();
        //Debug.Log("p1����" + p1Unit.Hero.HP + "Ѫ");
        
        //�ж�����
        if (isFainted)
        {
            state = CombatState.P1WON;
            EndCombat();
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
        yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } rolled a ...... {currentValue + 1}");
        yield return new WaitForSeconds(1.2f);

        //ѡ����
        var move = p2Unit.Hero.Moves[dice.CurrentValue];
        yield return dialogBox.TypeDialog($"{p2Unit.Hero.Base.name } used {move.Base.name}");//������Bug�Ȳ���
        //dialogBox.SetDialog($"{p2Unit.Hero.Base.name } used {move.Base.name}");
        yield return new WaitForSeconds(1.2f);

        //�����˺�
        bool isFainted = p1Unit.Hero.TakeDamage(move, p2Unit.Hero, currentValue);
        p1HUD.UpdateHp();
        p2HUD.ShowDamage();
        //Debug.Log("p2����" + p2Unit.Hero.HP + "Ѫ");

        //�ж�����
        if (isFainted)
        {
            state = CombatState.P2WON;
            EndCombat();
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
        StartCoroutine(PerformP1Move());
        yield return new WaitForSeconds(1.2f);
    }

    IEnumerator Player2Turn()
    {
        StartCoroutine(PerformP2Move());
        yield return new WaitForSeconds(1.2f);
    }

    //EndBattle ��Ϸ����
    void EndCombat()
    {
        if (state == CombatState.P1WON)
        {
            combatStatus.text = p1Unit.Hero.Base.Name + " Won!";
        }
        else if (state == CombatState.P2WON)
        {
            combatStatus.text = p2Unit.Hero.Base.Name + " Won!";
        }
    }


}
