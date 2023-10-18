using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    [SerializeField] HeroBase _base;
    [SerializeField] int level;

    [SerializeField] float enterDelay;
    [SerializeField] bool isPlayer1; //用来检查角色属于哪边

    [Header("Effects")]
    [SerializeField] GameObject smoke;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject statUpEffect;
    [SerializeField] GameObject statDownEffect;
    [SerializeField] AudioManager audioManager;

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
        spRenderer.sprite = Hero.Base.Sprite;
        bullet = Hero.Base.Bullet;

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
    public IEnumerator PlayAttackAnimation(Move move)
    {
        MoveActionType moveActionType = move.Base.MoveActionType;
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
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 50f, 0.2f));
                yield return new WaitForSeconds(0.2f);
                if (bullet != null)
                {
                    Instantiate(bullet, spRenderer.transform);
                }
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 50f, 0.2f));
                yield return new WaitForSeconds(0.2f);
                if (bullet != null)
                {
                    Instantiate(bullet, spRenderer.transform.position ,Quaternion.Euler(0,180,0));
                }
            }
        }
        if (moveActionType == MoveActionType.Heal) //治疗动画
        {
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.05f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.1f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.05f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.1f));
            }
        }
        if (moveActionType == MoveActionType.Special) //特殊动画
        {
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.05f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 20f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 20f, 0.1f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 15f, 0.05f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 15f, 0.1f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 15f, 0.1f));
            }
        }
        audioManager.PlayMovePerformAudio(0,move,false);
        sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.2f)); // 还原
        yield return new WaitForSeconds(0.3f);
    }

    //受击
    public IEnumerator PlayHitAnimation(Move move)
    {
        MoveActionType moveActionType = move.Base.MoveActionType;
        
        if (moveActionType == MoveActionType.Melee|| moveActionType == MoveActionType.Ranged) //被近战远程打
        {
            yield return StartCoroutine(TakenDamage());
        }
        if (moveActionType == MoveActionType.Special)//被特殊技能打
        {
            if (move.Base.MoveEffects.LosePercentLife.max != 0)
            {
                yield return StartCoroutine(TakenDamage());
            }
        }
        audioManager.PlayMoveHitAudio(1, move, false);
        yield return null;
    }
    //受伤害
    public IEnumerator TakenDamage()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spRenderer.DOColor(Color.gray, 0.1f));
        if (isPlayer1)
        {
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 2f, 0.1f));
        }
        else
        {
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 2f, 0.1f));
        }
        sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.1f));
        sequence.Append(spRenderer.DOColor(originalColor, 0.1f));
        yield return null;
    }
    //受到治疗
        public IEnumerator PlayHealAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        yield return new WaitForSeconds(0.4f);
    }

    //受到强化
    public IEnumerator PlayBoostedAnimation(int boostValue)
    {
        var sequence = DOTween.Sequence();
        Debug.Log($"BoostValue = {boostValue}");
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y + 20f, 0.1f));
        sequence.Append(spRenderer.transform.DOLocalMoveY(originalPos.y, 0.1f));
        yield return new WaitForSeconds(0.4f);
        if (boostValue > 0)
        {
            Instantiate(statUpEffect, spRenderer.transform);
        }
        else if (boostValue < 0)
        {
            Instantiate(statDownEffect, spRenderer.transform);
        }
        yield return new WaitForSeconds(0.2f);
    }
    //一个比较泛用的晃悠动画
    public IEnumerator PlayHurtAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer1)
        {
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.02f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
        }
        else
        {
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.02f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
        }
        sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.02f)); // 还原
        yield return new WaitForSeconds(0.2f);
    }

    //异常状态
    public IEnumerator PlayStatusAnimation()
    {
        if(Hero.Status == ConditionsDB.Conditions[ConditionID.psn])
        {
            var sequence = DOTween.Sequence();
            if (isPlayer1)
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.02f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
            }
            else
            {
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.02f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x - 5f, 0.04f));
                sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x + 5f, 0.04f));
            }
            sequence.Append(spRenderer.transform.DOLocalMoveX(originalPos.x, 0.02f)); // 还原
            yield return new WaitForSeconds(0.2f);
        }
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
