using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovementController : MonoBehaviour
{
    public int startInt;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        // Debug.Log("Executed 2");
        SetAnimationState(startInt);
        animator.SetInteger("AnimState", startInt);
    }

    private void SetAnimationState(int state)
    {
        // Debug.Log("Executed 3");
        if (animator.GetInteger("AnimState") != state)
        {
            // Debug.Log("Executed");
            animator.SetInteger("AnimState", state);
        }
    }
}
