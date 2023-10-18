using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    List<AudioSource> audios = new List<AudioSource>();

    [Header("AudioClips")]
    [SerializeField] AudioClip diceRoll;
    [SerializeField] AudioClip luckyBoost;
    [SerializeField] AudioClip combatEnd;
    [SerializeField] AudioClip getCoin;
    [SerializeField] AudioClip futureAttack;

    private void Start()
    {
        //轨道说明：
        //0：技能音效 1.受击音效 2：战斗内其它音效
        
        for(int i = 0; i< 3; i++)
        {
           var audio = this.gameObject.AddComponent<AudioSource>();
           audios.Add(audio);
        }
    }
    
    public void Play(int index, string name, bool isLoop)
    {
        var clip = GetAudioClip(name);
        if (clip != null)
        {
            var audio = audios[index];
            audio.clip = clip;
            audio.loop = isLoop;
            audio.Play();
        }
    }

    public void PlayMovePerformAudio(int index,Move move,bool isLoop)
    {
        var clip = GetMovePerformAudio(move);
        if(clip != null)
        {
            var audio = audios[index];
            audio.clip = clip;
            audio.loop = isLoop;
            audio.Play();
        }
    }
    public void PlayMoveHitAudio(int index, Move move, bool isLoop)
    {
        var clip = GetMoveHitAudio(move);
        if (clip != null)
        {
            var audio = audios[index];
            audio.clip = clip;
            audio.loop = isLoop;
            audio.Play();
        }
    }
    public void PlayMoveEffectAudio(int index, Move move, bool isLoop)
    {
        var clip = GetMoveEffectAudio(move);
        if (clip != null)
        {
            var audio = audios[index];
            audio.clip = clip;
            audio.loop = isLoop;
            audio.Play();
        }
    }

    AudioClip GetAudioClip(string name)
    {
        switch (name)
        {
            case "diceRoll":
                return diceRoll;
            case "luckyBoost":
                return luckyBoost;
            case "combatEnd":
                return combatEnd;
            case "getCoin":
                return getCoin;
            case "futureAttack":
                return futureAttack;
        }
        return null;
    }

    AudioClip GetMovePerformAudio(Move move)
    {
        if (move.Base.PerformSE != null)
        {
            return move.Base.PerformSE;
        }
        else return null;
    }
    AudioClip GetMoveHitAudio(Move move)
    {
        if (move.Base.HitSE != null)
        {
            return move.Base.HitSE;
        }
        else return null;
    }
    AudioClip GetMoveEffectAudio(Move move)
    {
        if (move.Base.EffectSE != null)
        {
            return move.Base.EffectSE;
        }
        else return null;
    }
}
