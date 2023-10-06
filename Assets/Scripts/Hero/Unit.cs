using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    [SerializeField] HeroBase _base;
    [SerializeField] int level;
    [SerializeField] GameObject smoke;
    [SerializeField] float enterDelay;
    [SerializeField] bool isPlayer1; //��������ɫ�����ı�

    public Hero Hero { get; set; }
    SpriteRenderer spRenderer;
    Vector3 originalPos;
    Color originalColor;

    //��ʼ�����洢��ɫunit�ĳ�ʼ״̬�����ڶ�����ԭ
    private void Awake()
    {
        spRenderer = GetComponent<SpriteRenderer>();
        originalPos = spRenderer.transform.localPosition;
        originalColor = spRenderer.color;
    }

    //��ɫ����
    public void Setup(Hero hero)
    {
        Hero = hero;
        Debug.Log(Hero .Base.HeroName + "�ĵ�ǰ��ֵ:" + Hero.HP + " " + Hero.Attack + " " + Hero.Defence + " " + Hero.Magic + " " + Hero.MagicDef + " " + Hero.Luck);
        spRenderer.sprite = Hero.Base.Sprite;

        StartCoroutine (PlayEnterAnimation());
    }

    //-----------------------------������أ�ʺɽ���֣�-----------------------------
    //������ԭ
    public IEnumerator AnimationReset()
    {
        spRenderer.transform.localPosition = new Vector3(originalPos.x, 270f);
        spRenderer.DOFade(1, 0.01f);
        yield return new WaitForSeconds(0.1f);
    }

    //�볡
    public IEnumerator PlayEnterAnimation()
    {
        spRenderer.transform.DOLocalMoveY(originalPos.y, enterDelay);
        yield return new WaitForSeconds(enterDelay);
        Instantiate(smoke, spRenderer.transform);
    }
    
    //����
    public IEnumerator PlayAttackAnimation(MoveActionType moveActionType)
    {
        var sequence = DOTween.Sequence();
        if (moveActionType == MoveActionType.Melee) //��ս����
        {
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 443f, 0.25f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 443f, 0.25f));
            }
        }
        if (moveActionType == MoveActionType.Ranged) //Զ�̶���
        {
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
            }
        }
        if (moveActionType == MoveActionType.Heal) //���ƶ���
        {
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.2f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.2f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.2f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.2f));
            }
        }
        if (moveActionType == MoveActionType.Special) //���⶯��
        {
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.2f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.2f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.2f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.2f));
            }
        }
        sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.2f)); // ��ԭ
        yield return new WaitForSeconds(0.45f);
    }

    //�ܻ�
    public IEnumerator PlayHitAnimation(MoveActionType moveActionType)
    {
        var sequence = DOTween.Sequence();
        if (moveActionType == MoveActionType.Melee|| moveActionType == MoveActionType.Ranged) //�ܻ�
        {
            sequence.Append(spRenderer.DOColor(Color.gray, 0.1f));
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 2f, 0.1f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 2f, 0.1f));
            }
        }
        sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.1f));
        sequence.Append(spRenderer.DOColor(originalColor, 0.1f));
        yield return null;
    }

    //�ܵ�ǿ��
    public IEnumerator PlayBoostedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        yield return new WaitForSeconds(0.4f);
    }

    //����
    public IEnumerator PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 10f, 0.2f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y -80f, 0.8f));
        sequence.Join(spRenderer.DOFade(0, 1f));

        yield return null;
    }

}
