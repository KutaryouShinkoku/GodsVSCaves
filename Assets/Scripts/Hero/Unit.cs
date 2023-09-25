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
    [SerializeField] bool isPlayer1; //备用，用来检查角色属于哪边

    public Hero Hero { get; set; }
    SpriteRenderer spRenderer;
    Vector3 originalPos;
    Color originalColor;

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
        //Debug.Log(Hero .Base.Name + "的初始血量攻防:" + Hero.Base.MaxHP + " " + Hero.Base.Attack + " " + Hero.Base.Defence);
        spRenderer.sprite = Hero.Base.Sprite;


        //Debug.Log("Sprite name:" );
        StartCoroutine (PlayEnterAnimation());
    }

    //--------------------------动画相关---------------------------
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
    public IEnumerator PlayAttackAnimation(int moveActionType)
    {
        var sequence = DOTween.Sequence();
        if (moveActionType == 1) //近战动画
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
        if (moveActionType == 2) //远程动画
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
        if (moveActionType == 3) //治疗动画
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
        if (moveActionType == 4) //特殊动画
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
        yield return null;
    }

    //受击
    public IEnumerator PlayHitAnimation(int moveActionType)
    {
        var sequence = DOTween.Sequence();
        if (moveActionType == 1|| moveActionType == 2) //受击
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
