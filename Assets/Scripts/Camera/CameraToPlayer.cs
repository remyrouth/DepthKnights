using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraToPlayer : MonoBehaviour
{
    public Transform eden;
    public Transform gatBat;
    public Image cutSceneTransition;
    public float yOffset = 2f;
    private Image imageComponent;
    // private Color targetColor;
    private float transitionRateChange = 0f;
    private bool canChangeTransition = false;

    void Start()
    {
        imageComponent = cutSceneTransition.GetComponent<Image>();
        Color initialColor = imageComponent.color;


        // imageComponent.color = new Color(initialColor.r, initialColor.g, initialColor.b, 255f); // Set alpha to 1
        StartTransition(-0.55f);
    }

    public void StartTransition(float newTransitionRate) {
        // Debug.Log("Transition Started");
        transitionRateChange = newTransitionRate;
        canChangeTransition = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (canChangeTransition) {
            // Debug.Log("canChangeTransition is true");
            Color originalColor = imageComponent.color;
            float newAlpha =  originalColor.a + (transitionRateChange * Time.deltaTime);
            newAlpha = Mathf.Min(newAlpha, 100f);
            newAlpha = Mathf.Max(newAlpha, 0f);
            

            imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha); // Set alpha to 1

            if (newAlpha== 1.0f || newAlpha == 0f) {
                // Debug.Log("Transition completed");
                canChangeTransition = false;
            }
        }


        if (gatBat == null && eden != null) {
            transform.position = new Vector3(eden.position.x, eden.position.y + yOffset, transform.position.z);
        } else if (gatBat != null && eden != null) {
            float newX = (eden.position.x + gatBat.position.x) / 2;
            float newY = (eden.position.y + gatBat.position.y) / 2;
            // float newZ = (eden.position.z + gatBat.position.z) / 2;
            transform.position = new Vector3(newX, eden.position.y + yOffset, transform.position.z);
        }
    }
}
