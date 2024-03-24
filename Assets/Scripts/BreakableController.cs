using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController : MonoBehaviour
{
    public int maxHealth = 3;  // Set the default maximum health

    private int currentHealth;

    public float deathTimer = 3f;

    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.1f;
    public float shakeSpeed = 10f;
    public List<AudioClip> hitSFXList = new List<AudioClip>();
    public float boxHitVolume = 0.8f;
    private SoundManager sm;
    void Start()
    {
        sm = FindObjectOfType<SoundManager>();
        // StopCoroutine(ShakeObject());
        currentHealth = maxHealth;

        // StartCoroutine(ShakeObject());

        // DecreaseHealth();
        // DecreaseHealth();
        // DecreaseHealth();
    }

    private void triggerSound() {
        sm.EnvironmentalSound(hitSFXList, boxHitVolume);
    }

    // Method to decrease health
    public void DecreaseHealth()
    {
        StopCoroutine(ShakeObject());
        currentHealth--;

        triggerSound();
        if (currentHealth <= 0)
        {
            BreakObject();
        } else {
            StartCoroutine(ShakeObject());
        }
    }

    // Method to handle object breaking
    private void BreakObject()
    {
        // Disable collider for the main object
        GetComponent<Collider2D>().enabled = false;

        // Enable colliders for all children
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D childCollider in childColliders)
        {
            childCollider.enabled = true;

            // Attach Rigidbody to the main object
            Rigidbody2D rb = childCollider.gameObject.AddComponent<Rigidbody2D>();

            // Apply a force to throw it away from the player (change the force vector as needed)
            // Vector3 forceDirection = (transform.position - Camera.main.transform.position).normalized;


            Vector3 playerDirection = (Camera.main.transform.position - transform.position).normalized;
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = 0;
            Vector3 forceDirection = (randomDirection + playerDirection).normalized;

            
            float forceStrength = 10f;  // Change this value as needed
            rb.AddForce(-1 * forceDirection * forceStrength, ForceMode2D.Impulse);
        }
        
        // Disable collider for the main object
        GetComponent<Collider2D>().enabled = false;


        TriggerCutscene();
        Invoke("Cleanup", deathTimer);
    }

    private void TriggerCutscene() {
        CutsceneTriggeredByObject ctbo = GetComponent<CutsceneTriggeredByObject>();
        if (ctbo != null) {
            ctbo.StartScene();
        }
    }

    private void Cleanup() {
        Destroy(gameObject);
    }

    private IEnumerator ShakeObject()
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            if (currentHealth <= 0) {
                StopCoroutine(ShakeObject());
                yield break;
            }
            float x = originalPosition.x + Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;
            float y = originalPosition.y + Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;

            transform.position = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition; // Reset to the original position after shaking
    }
}
