using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGFX : MonoBehaviour
{
    // 1 = death
    // 2 = damaged 
    // 3 = Move
    // 4 = attack
    public int deathAnimInt = 1;
    public int moveAnimInt = 3;
    public int attackAnimInt = 4;


    private Animator animator;
    private EnemyController ec;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponentInParent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerMoveAnim() {
        SetAnimationState(moveAnimInt);
    }

    public void TriggerDeathAnim() {
        SetAnimationState(deathAnimInt);
    }

    public void TriggerAttackAnim() {
        SetAnimationState(attackAnimInt);
    }

    public void ResetAfterBeingDamaged() {
        SetAnimationState(0);
    }

    private void TriggerFrameAttackDamage() {
        ec.EnemyDealsDamageOnFrame();
    } 

   public void SetAnimationState(int state)
    {
        if (animator.GetInteger("AnimState") != state)
        {
            animator.SetInteger("AnimState", state);
        }
    }

    // public void EndDamagedStatus() {
    //     ec.EndDamagedState();
    // }
}
