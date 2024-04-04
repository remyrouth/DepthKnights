using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabGFX : MonoBehaviour
{
    CrabController cc;
    private void Start() {
        cc = transform.parent.gameObject.GetComponent<CrabController>();
    }

    private void ResetAfterAttack(float idleDuration) {
        cc.ResetAfterAttack(idleDuration);
    }
}
