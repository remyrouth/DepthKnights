using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBlockTrapController : MonoBehaviour
{
    public Transform bottomFallPoint;
    public Transform topRisePoint;
    public GameObject fallBlockStone;
    private SpriteRenderer fbRenderer;
    //distance between gameobject position and its
    // sprite renderer bounds
    private float centerSpriteRendererOffset;
    public SpriteRenderer glowTextRenderer;
    private UnityEngine.Rendering.Universal.Light2D light2D;
    private float maxIntensity; 

    public float initialDelay;

    public float fallRate;
    public float riseRate;
    public float RechargeRate;



    private void Start() {
        fbRenderer = fallBlockStone.GetComponent<SpriteRenderer>();
        light2D = glowTextRenderer.gameObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        maxIntensity = light2D.intensity; 

        Vector3 topBounds = new Vector3(fallBlockStone.transform.position.x,
            fallBlockStone.transform.position.y - fbRenderer.bounds.size.y / 2f,
            fallBlockStone.transform.position.z);

        centerSpriteRendererOffset = Vector3.Distance(fallBlockStone.transform.position,topBounds);
        Invoke("EnableTrapCycle", initialDelay);
    }

    private bool canMove = false;

    private void EnableTrapCycle() {
        canMove = true;
    }

    private void Update() {
        RiseAndFallController();
        Recharge();
    }
    private float currentCharge = 100f;
    private void Recharge() {
        currentCharge += RechargeRate;
        currentCharge = Mathf.Min(currentCharge, 100f);

        float percentage = currentCharge / 100f;
        float alpha = 255f * percentage;
        float alphaNormalized = alpha / 255f;

        light2D.intensity = maxIntensity/percentage;

        if (glowTextRenderer != null) {
            Color originalColor = glowTextRenderer.color;
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaNormalized);
            glowTextRenderer.color = newColor;
        }
        // Debug.Log(alphaPercentage);
    }



    private bool fallingCycle = true;
    private void RiseAndFallController()
    {
        if (!canMove)
        {
            return;
        }

        if (fallingCycle)
        {
            float newYPos = fallBlockStone.transform.position.y - fallRate;
            newYPos = Mathf.Max(newYPos, bottomFallPoint.position.y - centerSpriteRendererOffset);
            fallBlockStone.transform.position = new Vector3(fallBlockStone.transform.position.x, newYPos, fallBlockStone.transform.position.z);

            if (fallBlockStone.transform.position.y <= bottomFallPoint.position.y - centerSpriteRendererOffset)
            {
                fallingCycle = false; // Because now we should start rising
                currentCharge = 0f;
            }
        }
        else
        {
            float newYPos = fallBlockStone.transform.position.y + riseRate;
            newYPos = Mathf.Min(newYPos, topRisePoint.position.y + centerSpriteRendererOffset);
            fallBlockStone.transform.position = new Vector3(fallBlockStone.transform.position.x, newYPos, fallBlockStone.transform.position.z);

            if (fallBlockStone.transform.position.y >= topRisePoint.position.y + centerSpriteRendererOffset
             && currentCharge == 100f)
            {
                fallingCycle = true; // Because now we should start falling
            }
        }
    }


}
