using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrabGFX : MonoBehaviour
{
    CrabController cc;
    private int currentHealth;
    private int maxHealth;
    public Slider healthBarSlider;
    private void Start() {
        cc = transform.parent.gameObject.GetComponent<CrabController>();

        currentHealth = cc.GiveHealthToGFX();
        maxHealth = currentHealth;
    }

    private void Update() {
        currentHealth = cc.GiveHealthToGFX();
        float percentage = (float)currentHealth / maxHealth;

        //Debug.Log("current health: " + currentHealth + "     max health: " + maxHealth + "    percentage: " + percentage);

        if (healthBarSlider != null) {
            healthBarSlider.value = percentage;
        }
        // healthBarSlider.value = percentage;
    }

    private void ResetAfterAttack(float idleDuration) {
        cc.ResetAfterAttack(idleDuration);
    }

    private void PlaySpecificSound(int index) {
        cc.PlaySpecificSound(index);
    }
}
