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
    [SerializeField] bool isPlayer1; //用来检查角色属于哪边

    public Hero Hero { get; set; }
    SpriteRenderer spRenderer;
    Vector3 originalPos;
    Color originalColor;

    //初始化，存储角色unit的初始状态，便于动画还原
    private void Awake()
    {
        spRenderer = GetComponent<SpriteRenderer>();
        originalPos = spRenderer.transform.localPosition;
        originalColor = spRenderer.color;
    }

    //角色导入
    public void Setup(Hero hero)
    {
        Hero = hero;
        Debug.Log(Hero .Base.HeroName + "的当前数值:" + Hero.HP + " " + Hero.Attack + " " + Hero.Defence + " " + Hero.Magic + " " + Hero.MagicDef + " " + Hero.Luck);
        spRenderer.sprite = Hero.Base.Sprite;

        StartCoroutine (PlayEnterAnimation());
    }

    //-----------------------------动画相关（屎山部分）-----------------------------
    //动画复原
    public IEnumerator AnimationReset()
    {
        spRenderer.transform.localPosition = new Vector3(originalPos.x, 270f);
        spRenderer.DOFade(1, 0.01f);
        yield return new WaitForSeconds(0.1f);
    }

    //入场
    public IEnumerator PlayEnterAnimation()
    {
        spRenderer.transform.DOLocalMoveY(originalPos.y, enterDelay);
        yield return new WaitForSeconds(enterDelay);
        Instantiate(smoke, spRenderer.transform);
    }
    
    //攻击
    public IEnumerator PlayAttackAnimation(MoveActionType moveActionType)
    {
        var sequence = DOTween.Sequence();
        if (moveActionType == MoveActionType.Melee) //近战动画
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
        if (moveActionType == MoveActionType.Ranged) //远程动画
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
        if (moveActionType == MoveActionType.Heal) //治疗动画
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
        if (moveActionType == MoveActionType.Special) //特殊动画
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
        sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.2f)); // 还原
        yield return new WaitForSeconds(0.45f);
    }

    //受击
    public IEnumerator PlayHitAnimation(MoveActionType moveActionType)
    {
        var sequence = DOTween.Sequence();
        if (moveActionType == MoveActionType.Melee|| moveActionType == MoveActionType.Ranged) //受击
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

    //受到强化
    public IEnumerator PlayBoostedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        yield return new WaitForSeconds(0.4f);
    }

    //阵亡
    public IEnumerator PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 10f, 0.2f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y -80f, 0.8f));
        sequence.Join(spRenderer.DOFade(0, 1f));

        yield return null;
    }

}
