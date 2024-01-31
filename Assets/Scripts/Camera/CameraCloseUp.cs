using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCloseUp : MonoBehaviour
{
    private Animator animator;
    private bool combatCamera = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    //ÇÐ¾µÍ·µ½ÌØÐ´
    public void HeroCloseup(string camId)
    {
        if (combatCamera)
        {
            animator.Play(camId);
            Debug.Log($"Closeup!{camId}");
        }
        else
        {
            animator.Play("CombatCamera");
        }
        combatCamera = !combatCamera;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) { HeroCloseup("CloseupCameraP1"); }
        if (Input.GetKeyDown(KeyCode.E)) { HeroCloseup("CloseupCameraP2"); }
    }
}
