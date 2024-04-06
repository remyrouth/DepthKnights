using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleController : MonoBehaviour
{
    private Animator ani; 

    public List<int> animationInts = new List<int>();

    public float invokeRateMaximum = 1f;


    private void Start() {
        ani = GetComponentInChildren<Animator>();

        TriggerAnimation();
    }

    private void TriggerAnimation() {
        SetAnimationState(ChooseRandomIdle());
        float randomWaitTime = Random.Range(0, invokeRateMaximum);
        Invoke("Reset", randomWaitTime);

        Invoke("TriggerAnimation", invokeRateMaximum);
    }

    private void Reset() {
        SetAnimationState(0);
    }

    private int ChooseRandomIdle() {
        int randomIndex = Random.Range(0, animationInts.Count - 1);

        return animationInts[randomIndex];
    }

    private void SetAnimationState(int state)
    {
        if (ani.GetInteger("AnimState") != state)
        {
            ani.SetInteger("AnimState", state);
        }
    }
}
